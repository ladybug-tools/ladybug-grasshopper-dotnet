﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace LadybugGrasshopper
{
    public class Ladybug_SortByLayers : GH_Component
    {
        public Ladybug_SortByLayers()
          : base("LB SortByLayers", "SortByLayers",
              "Sort and group Rhino objects by layers.\n\nPlease find the source code from:\nhttps://github.com/ladybug-tools/ladybug-grasshopper-dotnet",
              "Ladybug", "4 :: Extra")
        {
        }

        
        
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("K", "K", "A list of Rhino objects that associated with sortable layers", GH_ParamAccess.list);
            pManager.AddGeometryParameter("A", "A", "Optional object list to sort synchronously", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }

        
        
        
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("K", "K", "Sorted objects by layers", GH_ParamAccess.tree);
            pManager.AddGeometryParameter("A", "A", "Synchronously sorted objects", GH_ParamAccess.tree);
            pManager.AddTextParameter("n", "n", "Grouped layer names", GH_ParamAccess.tree);
        }

        
        
        
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //var RefList = Component.Params.Input[0].VolatileData.AllData(true);
            var refList = new List<IGH_GeometricGoo>();
            var secondList = new List<IGH_GeometricGoo>();
            if (!DA.GetDataList(0, refList))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid Rhino objects!");
                return;
            }
            DA.GetDataList(1, secondList);

            List<string> layerNames = new List<string>();
            var doc = Rhino.RhinoDoc.ActiveDoc;
            var layers = doc.Layers;

            var dic = new Dictionary<string, List<int>>();


            int mark = 0;
            foreach (var item in refList)
            {
                var refID = item.ReferenceID;
                var currentRhinoObj = doc.Objects.Find(refID);
                var atLayerIndex = currentRhinoObj.Attributes.LayerIndex;
                var currentlayerName = layers[atLayerIndex].Name;
                
                //add to layer dictionary
                if (dic.ContainsKey(currentlayerName))
                {
                    dic[currentlayerName].Add(mark);
                }
                else
                {
                    dic[currentlayerName] = new List<int>() { mark };
                }

                mark++;

            }

            var dicKeys = dic.Keys.ToList();
            dicKeys.Sort();

            DataTree<object> treeK = new DataTree<object>();
            DataTree<object> tree_names = new DataTree<object>();
            int i = 0;
            foreach (var layer in dicKeys)
            {
                GH_Path pth = new GH_Path(i);
                foreach (var index in dic[layer])
                {
                    treeK.Add(refList.ElementAt(index), pth);
                }
                tree_names.Add(layer, pth);
                i++;
            }

            DA.SetDataTree(0, treeK);
            DA.SetDataTree(2, tree_names);

            if (secondList.Any())
            {
                DataTree<object> treeA = new DataTree<object>();
                int j = 0;
                foreach (var layer in dicKeys)
                {
                    GH_Path pth = new GH_Path(j);
                    foreach (var index in dic[layer])
                    {
                        treeA.Add(secondList.ElementAt(index), pth);
                    }
                    j++;
                }

                DA.SetDataTree(1,treeA);
                
            }




        }
        protected override System.Drawing.Bitmap Icon => Resources.SortByLayers;

        public override Guid ComponentGuid => new Guid("6B4BA6D8-B458-45BB-9C4F-2E6ED8CD4034");
    }
}