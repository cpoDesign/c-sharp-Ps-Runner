/*
In order to get following using we can use new package 

using System.Management.Automation;
you have 2 choises

Install-Package System.Management.Automation
or
C:\windows\assembly\GAC_MSIL\System.Management.Automation\1.0.0.0__31bf3856ad364e35\System.Management.Automation.dll

*/

using System;
using System.Collections.ObjectModel;
using System.Management.Automation;


namespace PowershellExecuting
{
     public class PowerShellHelper
    {
        private PowerShell _shell;
        private Collection<string> _collection;

        public PowerShellHelper(PowerShell shell)
        {
            _shell = shell;
            _collection = new Collection<string>();
        }

        public Collection<string> GetOutput()
        {
            return _collection;
        }

        public void ExecuteAsynchronously(TimeSpan timeout)
        {
            _shell.InvocationStateChanged += delegate (object sender, PSInvocationStateChangedEventArgs e)
            {
                RecordSystemAudit(e.InvocationStateInfo.State.ToString());
            };

            // prepare a new collection to store output stream objects
            PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
            outputCollection.DataAdded += outputCollection_DataAdded;

            // begin invoke execution on the pipeline
            // use this overload to specify an output stream buffer
            IAsyncResult result = _shell.BeginInvoke<PSObject, PSObject>(null, outputCollection);

            result.AsyncWaitHandle.WaitOne(timeout);

            foreach (PSObject outputItem in outputCollection)
            {
                if (outputItem != null)
                {
                    Console.WriteLine(outputItem.BaseObject.ToString());
                }
            }
        }

        /// <summary>
        /// Event handler for when data is added to the output stream.
        /// </summary>
        /// <param name="sender">Contains the complete PSDataCollection of all output items.</param>
        /// <param name="e">Contains the index ID of the added collection item and the ID of the PowerShell instance this event belongs to.</param>
        private void outputCollection_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<PSObject> myp = (PSDataCollection<PSObject>)sender;
            Collection<PSObject> results = myp.ReadAll();
            foreach (PSObject result in results)
            {
                RecordAudit(result.ToString());
            }
        }

        private void RecordSystemAudit(string message)
        {
            _collection.Add(string.Format("System: {0} {1}", DateTime.UtcNow, message));
        }

        private void RecordAudit(string message)
        {
            _collection.Add(string.Format("{0} {1}", DateTime.UtcNow, message));
        }
    }
}
