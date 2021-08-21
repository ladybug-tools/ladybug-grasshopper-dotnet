using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LadybugGrasshopper
{
    public class RaTiff : RadianceBaseCommand
    {
        private string InputHdrFile { get; set; }
        private string OutputTiffFile { get; set; }

        public RaTiff(string inputHdrFile, string outputTiffFile):base("ra_tiff")
        {
            this.InputHdrFile = inputHdrFile;
            this.OutputTiffFile = outputTiffFile;
        }
        
        protected override string ToRadString()
        {
            //string cmdName = this.ExePath;
            string inputFile = this.InputHdrFile;
            string outputFile = this.OutputTiffFile;
            string radString = $"\"{inputFile}\" \"{outputFile}\"";

            return radString;
        }

        public override string ToString()
        {
            return this.ToRadString();
        }
    }
}
