using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;


namespace LadybugGrasshopper
{
    public abstract class RadianceBaseCommand
    {
        private static string _radDir;

        protected static string RadDir
        {
            get 
            {
                if (string.IsNullOrEmpty(_radDir))
                    _radDir = GetRadianceDir();
                return _radDir; 
            }
        }

        protected static string RadlibPath => Path.Combine(RadDir, "lib");

        protected static string RadbinPath => Path.Combine(RadDir, "bin");

        private string exeName;
        protected string ExePath => Path.Combine(RadbinPath, exeName);

        protected RadianceBaseCommand(string executableName)
        {
            exeName = executableName;
        }

        protected abstract string ToRadString();

        public virtual string Execute()
        {
            var envArg = GetEnvArgForRadiance();
            var args = $"{this.ToRadString()}";
            Process cmd = new Process()
            {
                
                StartInfo = new ProcessStartInfo()
                {
                    FileName = this.ExePath,
                    Arguments = args,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput =true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
                
            };

            cmd.Start();
            string outputs = cmd.StandardOutput.ReadToEnd();
            string err = cmd.StandardError.ReadToEnd();

            cmd.WaitForExit();
            

            cmd.Close();

            if (!string.IsNullOrEmpty(err))
            {
                throw new ArgumentException($"{ err }\n{this.ToRadString()}");
            }
            
            return outputs;

            
        }


        private static bool isNTSystem()
        {
            return Environment.OSVersion.ToString().ToUpper().Contains("NT");
        }

        public static string GetRadianceDir()
        {
            var pyRun = Rhino.Runtime.PythonScript.Create();
            var pyScript = @"
from honeybee_radiance.config import folders as radiance_folders
rad_install = radiance_folders.radiance_path
print(rad_install)";
            pyRun.ExecuteScript(pyScript);
            var radiance = pyRun.GetVariable("rad_install").ToString();
            if (!Directory.Exists(radiance))
                throw new ArgumentException("Failed to find the Radiance on your machine, please install it first!");
            return radiance;
        }

        public static string GetEnvArgForRadiance()
        {

            var radBin = RadbinPath;
            var radLib = RadlibPath;

            if (Directory.Exists(radBin) && Directory.Exists(radLib))
            {
                var envArg = $" --env PATH=\"{radBin}\"";
                envArg += $" --env RAYPATH=\"{radLib}\"";
                return envArg;
            }
            return string.Empty;

        }
    }
}
