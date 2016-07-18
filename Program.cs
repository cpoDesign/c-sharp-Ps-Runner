using System;
using System.Management.Automation;

namespace PowershellExecuting
{
    class Program
    {
        static void Main(string[] args)
        {
            // session configuration information may be found: https://technet.microsoft.com/library/hh847817.aspx
            string script = "Write-Host \"Testing lopping...\"" + Environment.NewLine
                           + "for ($i=1; $i -le 5; $i++)" + Environment.NewLine
                           + "{" + Environment.NewLine
                           + "Write-Output $i" + Environment.NewLine
                           + "Start-Sleep -s 1" + Environment.NewLine
                           + "write-error 'some error $i';" + Environment.NewLine
                           + "}" + Environment.NewLine
                           + "Write-Host \"Done!\"" + Environment.NewLine;

            Console.Write(script);

            Console.WriteLine();
            Console.WriteLine("====================");
            PowerShell shell = PowerShell.Create();
            shell.AddScript(script);

            PowerShellHelper helper = new PowerShellHelper(shell);
            try
            {
                // the script above should take 15 seconds to execute

                //// do timeout of 10 minutes
                helper.ExecuteAsynchronously(new TimeSpan(0, 10, 0));

                // do a really short timeout - 2 seconds - testing for timeout
                //helper.ExecuteAsynchronously(new TimeSpan(0, 0, 2));
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
    

            //new PowerShellExecutor().ExecuteSynchronouslyNamespaceTest();
            Console.WriteLine("press any key to close");
            Console.ReadKey();
        }
    }
}
