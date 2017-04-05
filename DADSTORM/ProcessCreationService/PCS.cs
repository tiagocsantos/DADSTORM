using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Diagnostics;
using System.Collections;

namespace ProcessCreationService 
{
    class PCSRunner
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(10001);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(PCS),"ProcessCreationService", WellKnownObjectMode.Singleton);
            Console.WriteLine("PCS is running\r\nPress < enter > to exit...");
            Console.ReadLine();
        }         
    }

    class PCS : MarshalByRefObject, IProcessCreationService
    {
        private IPuppetMaster PuppetMaster;
        private TcpChannel PuppetMasterCh;

        public void createOperator(string id, string input, string rep_fact, string routing, string[] addrs, string operator_spec, string loggingLvl, string semantics)
        {
            PuppetMaster.logAction("PCS : Request to Create Operator: "+id);

            Console.WriteLine("operator " + id + " be created");
            for (int i = 0; i < Int32.Parse(rep_fact); i++)
            {
                ProcessStartInfo startinfo = new ProcessStartInfo();
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\Operator";
                startinfo.FileName = @"bin\Debug\Operator.exe";

                if (input == "")
                    input = "none";


                string[] opinfo = new string[2];
                opinfo[0] = operator_spec.Split(' ')[0];

                if (operator_spec.Split(' ').Count() == 1)
                    opinfo[1] = "none";
                else 
                    opinfo[1] = operator_spec.Split(' ')[1];

                string RepAdr;

                if (Int32.Parse(rep_fact) > 1)
                    RepAdr = String.Join(" ", addrs);
                else
                    RepAdr = "none";


                startinfo.Arguments = id+" "+input+" "+addrs[i]+" "+routing+" "+loggingLvl+" "+semantics+" "+opinfo[0]+" "+opinfo[1]+" "+rep_fact+" "+RepAdr;
                startinfo.WindowStyle = ProcessWindowStyle.Normal;
                Process pid = new Process();
                pid.StartInfo = startinfo;
                pid.Start();
            }
        }

        public void registerPuppetMaster(string ip)
        {
            IDictionary propBag = new Hashtable();
            propBag["name"] = "PCS-PuppetMaster";
            PuppetMasterCh = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(PuppetMasterCh, false);
            PuppetMaster = (IPuppetMaster)Activator.GetObject(typeof(IPuppetMaster), 
                Util.createURL(ip, Util.PUPPET_MASTER_PORT, Util.PUPPET_MASTER_SERVICE));

            PuppetMaster.logAction("PCS : PuppetMaster Connected");
        }

        public void status()
        {
           // throw new NotImplementedException();
        }

        public void UnregisterPuppetMaster()
        {
            ChannelServices.UnregisterChannel(PuppetMasterCh);
            PuppetMaster = null;
            PuppetMasterCh = null;
        }

        public void kill()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
