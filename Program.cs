using System;
using System.Management.Automation;

namespace PowershellExecuting
{
    class Program
    {
         static void Main(string[] args)
        {
            RunInlineScript();

            Console.WriteLine("====================");
            string path = @"c:\PowerShell\Full.ps1";
            ExecutePsFile(path);

            Console.ReadKey();
        }

        private static void ExecutePsFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Sorry file does not exists");
                Console.ReadKey();
                return;
            }

            // Open the file to read from.
            var shell = PowerShell.Create();
            shell.AddCommand(path);

            var helper2 = new PowerShellHelper(shell);
            try
            {
                helper2.ExecuteAsynchronously(new TimeSpan(1, 10, 0));
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
            }

            foreach (var item in helper2.GetOutput())
            {
                Console.WriteLine(item);
            }
        }

        private static void RunInlineScript()
        {
            // session configuration information may be found: https://technet.microsoft.com/library/hh847817.aspx
            var script = "Write-Host \"Testing lopping...\"" + Environment.NewLine
                           + "for ($i=1; $i -le 5; $i++)" + Environment.NewLine
                           + "{" + Environment.NewLine
                           + "Write-Output $i" + Environment.NewLine
                           + "Start-Sleep -s 1" + Environment.NewLine
                           + "write-error 'some error $i';" + Environment.NewLine
                           + "}" + Environment.NewLine
                           + "Write-Host \"Done!\"" + Environment.NewLine;
            Console.Write(script);

            var shell = PowerShell.Create();
            shell.AddScript(script);

            var helper = new PowerShellHelper(shell);
            try
            {

                helper.ExecuteAsynchronously(new TimeSpan(0, 10, 0));
            }
            catch (TimeoutException te)
            {
                Console.WriteLine(te.Message);
                Console.WriteLine("\n\nScript took long!");
                shell.Stop();
            }

            foreach (var item in helper.GetOutput())
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("press any key to close");
            Console.ReadKey();
        }
    }
}
