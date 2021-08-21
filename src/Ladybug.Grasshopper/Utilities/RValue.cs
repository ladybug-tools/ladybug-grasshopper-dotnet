using System;
using System.Collections.Generic;
using System.Linq;

namespace LadybugGrasshopper
{

    public class PValue : RadianceBaseCommand
    {

        //private int X = 0;
        //private int Y = 0;

        private string InputHdrFile { get; set; }

        //private string OutputTiffFile { get; set; }

        public PValue(string inputHdrFile):base("pvalue")
        {
            this.InputHdrFile = inputHdrFile;
        }
        
        protected override string ToRadString()
        {
            //string cmdName = this.ExePath;
            string cmdParams = "-o -d -h -b";
            string inputFile = this.InputHdrFile;
            string radString = $"{cmdParams} \"{inputFile}\"";

            return radString;
        }

        public new IEnumerable<double> Execute()
        {
            var outputStr = base.Execute().Trim();
            var outputlist = outputStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (outputlist.Length==0)
            {
                new Exception("Failed to extract HDR image values!");
                return new List<double>();
            }
            else
            {
                //var dim = outputlist[0].Split(' ');
                //this.X = Convert.ToInt16(dim[3]);
                //this.Y = Convert.ToInt16(dim[1]);
                var output = outputlist.Skip(1).Select(_ => double.Parse(_.Trim()));
                return output;
            }

            
        }



        public override string ToString() => this.ToRadString();
    }
}
