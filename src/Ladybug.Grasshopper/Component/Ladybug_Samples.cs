using Grasshopper.Kernel;
using System;
using GH = Grasshopper;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Rhino;

namespace LadybugGrasshopper
{
    public class Ladybug_Samples : GH_Component
    {
        static Rhino.Runtime.PythonScript _script;
       
        List<string> folderList = new List<string>();
        List<List<string>> filesList = new List<List<string>>();
        public Ladybug_Samples()
          : base("LB Samples", "Samples",
                "Load sample files\n\nPlease find the source code from:\nhttps://github.com/ladybug-tools/ladybug-grasshopper-dotnet",
                "Ladybug", "4 :: Extra")
        {
        }
        public override Guid ComponentGuid => new Guid("BF8CF596-EAAF-460A-8B51-97E53B0197FD");
        protected override System.Drawing.Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Directory", "_dir", "Additional folder path to load the sample files.", GH_ParamAccess.list);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Templates", "out", "Sample files found from folders", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.folderList = new List<string>();
            this.filesList = new List<List<string>>();
            var sampleDirs = GetSampleFolder();
            var dirs = sampleDirs.ToList();
            DA.GetDataList(0, dirs);

            dirs =  dirs.Distinct().Where(_ => Directory.Exists(_)).ToList();
            if (dirs.Count == 0)
                throw new ArgumentException("No template folder was found!");

            foreach (var dir in dirs)
            {
                var fs = Directory.GetFiles(dir, "*.gh*", SearchOption.AllDirectories).ToList();
                if (fs.Any())
                {
                    this.folderList.Add(dir);
                    this.filesList.Add(fs);
                }
            }
            DA.SetDataList(0, this.filesList.SelectMany(_=>_));

            this.templateMenu = GetMenu();
            
        }

        private static List<string> GetSampleFolder()
        {
            var folders = new List<string>();
            try
            {
                _script = _script ?? Rhino.Runtime.PythonScript.Create();
                _script.ScriptContextDoc = RhinoDoc.ActiveDoc;
                var pyScript =
    @"
from ladybug.config import folders as lb_folders
epw_folder = lb_folders.default_epw_folder
lbt_folder = lb_folders.ladybug_tools_folder
";
                _script.ExecuteScript(pyScript);
                //C:\Users\mingo\AppData\Roaming\ladybug_tools\weather
                var epw = _script?.GetVariable("epw_folder")?.ToString();
                if (Directory.Exists(epw))
                {
                    var dir = Path.Combine(System.IO.Path.GetDirectoryName(epw), "samples");
                    folders.Add(dir);

                }

                //check the ladybug_tools installation folder
                var lbt = _script?.GetVariable("lbt_folder")?.ToString();
                if (Directory.Exists(lbt))
                {
                    var dir = Path.Combine(lbt, "resources", "samples");
                    folders.Add(dir);
                }
            }
            catch (Exception) { }
            
            return folders;
        }

        private Size GetMoveVector(PointF FromLocation)
        {
            var moveX = this.Attributes.Bounds.Left - 80 - FromLocation.X;
            var moveY = this.Attributes.Bounds.Y + 180 - FromLocation.Y;
            var loc = new System.Drawing.Point(Convert.ToInt32(moveX), Convert.ToInt32(moveY));
            
            return new Size(loc);
        }

        private void CreateTemplateFromFilePath(string filePath, ref bool run)
        {
            var canvasCurrent = GH.Instances.ActiveCanvas;
            var f = canvasCurrent.Focused;
            var isFileExist = File.Exists(filePath);

            if (run && f && isFileExist)
            {
                var io = new GH_DocumentIO();
                var success = io.Open(filePath);

                if (!success)
                {
                    MessageBox.Show($"Failed to open sample file from:{filePath}");
                    return;
                }
                var docTemp = io.Document;

                docTemp.SelectAll();
                docTemp.MutateAllIds();

                //move to where this component is...
                var box = docTemp.BoundingBox(false);
                var vec = GetMoveVector(box.Location);
                docTemp.TranslateObjects(vec ,true);

                docTemp.ExpireSolution();
                var docCurrent = canvasCurrent.Document;
                docCurrent.DeselectAll();
                docCurrent.MergeDocument(docTemp);
            }
        }

        private ToolStripDropDownMenu GetMenu()
        {
            var menu = new ToolStripDropDownMenu();

            var rootFolder = this.folderList.FirstOrDefault();
            var topDirs = Directory.GetDirectories(rootFolder);

            foreach (var item in topDirs)
            {
                var menuItem = addFromFolder(item);
                if (menuItem == null)
                    continue;
                menu.Items.Add(menuItem);
            }

            return menu;
        }

        ToolStripDropDownMenu templateMenu = new ToolStripDropDownMenu();
        private ToolStripMenuItem addFromFolder(string rootFolder)
        {
            var allFiles = Directory.GetFiles(rootFolder, "*.gh*", SearchOption.AllDirectories);
            if (!allFiles.Any()) return null;

            var topDirs = Directory.GetDirectories(rootFolder);
            var topFiles = Directory.GetFiles(rootFolder, "*.gh*", SearchOption.TopDirectoryOnly);


            // create menu item
            var folderName = new DirectoryInfo(rootFolder).Name;
            var t = new ToolStripMenuItem(folderName);

            foreach (var item in topDirs)
            {
                var menuItem = addFromFolder(item);
                if (menuItem == null)
                    continue;
                t.DropDownItems.Add(menuItem);
            }


            foreach (var item in topFiles)
            {
                //var p = Path.GetDirectoryName(item);
                var name = Path.GetFileNameWithoutExtension(item);
                //var showName = p.Length > rootFolder.Length ? p.Replace(rootFolder+"\\", "") + "\\" + name : name;

                EventHandler ev = (object sender, EventArgs e) =>
                {
                    var a = sender as ToolStripDropDownItem;
                    var r = true;
                    CreateTemplateFromFilePath(a.Tag.ToString(), ref r);
                    this.ExpireSolution(true);

                };

                Menu_AppendItem(t.DropDown, name, ev, null, item);
            }
           
            return t;
        }

        public override void CreateAttributes()
        {
            var att = new ComponentButtonAttributes(this);
            att.ButtonText = "Load a sample";

            att.MouseDownEvent += (object loc) => this.templateMenu.Show((GH.GUI.Canvas.GH_Canvas)loc,(loc as GH.GUI.Canvas.GH_Canvas).CursorControlPosition);
            this.Attributes = att;

        }
        
    }


}
