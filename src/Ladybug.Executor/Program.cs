using System;
using System.Linq;

namespace Ladybug.Executor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {

                if (args == null || args.Length == 0)
                {
                    Console.WriteLine($"Invalid argument!");
                    return;
                }

                var cleaned = args.Select(x => x.Trim()).ToArray();
                var program = cleaned.FirstOrDefault();

                if (string.IsNullOrEmpty(program))
                {
                    Console.WriteLine($"Invalid program!");
                    return;
                }
                var arguments = string.Join(" ", cleaned.Skip(1));


                var sInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = false,
                    //FileName = @"C:\Program Files\ladybug_tools\python\python.exe",
                    //Arguments = @"-m pip install ladybug-rhino",
                    FileName = program,
                    Arguments = arguments,
                    //Arguments = @"-m ladybug-rhino change-installed-version [OPTIONS]",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    //CreateNoWindow = true,  

                };

                Console.WriteLine($"{sInfo.FileName} {sInfo.Arguments}");

                var p = new System.Diagnostics.Process();
                p.StartInfo = sInfo;


                p.ErrorDataReceived += P_ErrorDataReceived;
                p.OutputDataReceived += P_OutputDataReceived;
                p.Start();

                // To avoid deadlocks, use an asynchronous read operation on at least one of the streams.  
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();

                p.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Press any key to close!");
                Console.ReadKey();
            }
            
        }

        private static void P_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Console.WriteLine(e?.Data);
        }

        private static void P_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Console.WriteLine(e?.Data);
        }
    }
}
