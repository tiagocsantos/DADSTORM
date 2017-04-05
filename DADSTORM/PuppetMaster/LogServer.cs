using SharedTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class LogServer : MarshalByRefObject, IPuppetMaster
    {
        private string logFile;
        private PuppetMasterForm parentForm;
        private string lastMsg;

        delegate void updateFormLog(string msg);

        public LogServer(PuppetMasterForm Form)
        {
            parentForm = Form;
        }

        public void setLogFile(string fileName)
        {
            logFile = Util.LOG_FILES_DIRECTORY + "\\" + fileName;
            FileStream f = File.Open(logFile, FileMode.Create);
            f.SetLength(0);
            f.Close();
        }

        public void logAction(string msg)
        {
            lastMsg = msg;
            Monitor.Enter(this);
            TextWriter tw = File.AppendText(logFile);
            tw.WriteLine(msg);
            tw.Close();
            Monitor.Exit(this);
        }


    }
}
