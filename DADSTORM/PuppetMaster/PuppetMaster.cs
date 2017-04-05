using SharedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
using System.Threading;
using System.Linq;

namespace PuppetMaster
{
    class PuppetMaster : MarshalByRefObject
    {
        //delegate para chamadas assincronas
        public delegate void RemoteAsyncDelegate();
        public delegate void RemoveAsyncDelegateInt(int x);

        public delegate void AddOperatorDel(string s);
     
        //save PCS for each machine
        private  Dictionary<string, IProcessCreationService> pcsList = new Dictionary<string, IProcessCreationService>();
        //save channels in use  for PCS
        private  Dictionary<string, TcpChannel> channelList = new Dictionary<string, TcpChannel>();
        //save operator addresses 
        private  Dictionary<string, string[]> opList = new Dictionary<string, string[]>();
        //save operator channels
        private  Dictionary<string, TcpChannel[]> opChannel = new Dictionary<string, TcpChannel[]>();
        //save operator objects
        private  Dictionary<string, IOperator[]> opObj = new Dictionary<string, IOperator[]>();
        
        private string puppetMasterIP;

        private LogServer logServer;
        private TcpChannel LogServerCh;

        private int instructionIndex;

        private string[] fileLines;

        private PuppetMasterForm parentForm;

        public PuppetMaster(PuppetMasterForm Form)
        {
            parentForm = Form;
            logServer = new LogServer(parentForm);
            LogServerCh = new TcpChannel(Util.PUPPET_MASTER_PORT);
            ChannelServices.RegisterChannel(LogServerCh, false);
            RemotingServices.Marshal(logServer, Util.PUPPET_MASTER_SERVICE, typeof(LogServer));
        }

        internal void crashAllOperarors()
        {
            while (opObj.Count != 0)
            {
                crashOperator(opObj.First().Key, -1);
            }

            foreach (TcpChannel[] chs in opChannel.Values)
            {
                foreach (TcpChannel ch in chs)
                    ChannelServices.UnregisterChannel(ch);
            }
            opObj.Clear();
            opChannel.Clear();
            opList.Clear();
        }

        public void startConfigFile(string filename)
        {  
            logServer.setLogFile(filename);

            //get localhost ip
            puppetMasterIP = Util.getLocalHostIP();

            logServer.logAction("       NewDebug     -------------------------");
            instructionIndex = readConfigFile(Util.CONFIG_FILES_DIRECTORY+"\\"+filename);
            logServer.logAction("PuppetMaster: Configuration File Read");
        }

        public int readConfigFile(string path)
        {

            fileLines = File.ReadAllLines(path);
            string[] info;
            string semantics = null;
            string logging = "light";
            int i;
            for (i = 0; i < fileLines.Length; i++)
            {
                if (fileLines[i].StartsWith(Util.COMMENT_TOKEN) | fileLines[i] == "") //comment or empty lines
                    continue;

                else if (fileLines[i].StartsWith(Util.SEMANTICS_TOKEN))
                    semantics = fileLines[i].Split(null)[1];

                else if (fileLines[i].StartsWith(Util.LOGGING_TOKEN))
                    logging = fileLines[i].Split(null)[1];

                else if (fileLines[i].StartsWith("OP"))
                {
                    info = fileLines[i].Trim(new char[] { ' ' }).Split(Util.TOKENS, StringSplitOptions.None);

                    for (int j = 0; j < info.Length; j++)
                        info[j] = info[j].Trim();

                    createOperator(info, semantics, logging);

                }
                else break;
            }
            return i;
        }

        private void createOperator(string[] info, string semantics, string logginglvl)
        {
            string id = info[0];
            string input = info[1];
            string rep_fact = info[2];
            string routing = info[3];
            string[] addrs = info[4].Replace(" ", "").Split(',');
            string operator_spec = info[5];
            string[] tokens = { "://", ":" };
            //get machine ip
            string machineIP = addrs[0].Split(tokens, StringSplitOptions.None)[1];

            //connect to PSC
            IProcessCreationService pcs;
            if (!pcsList.TryGetValue(machineIP, out pcs))
                conectPCS(machineIP);

            pcs = pcsList[machineIP];
            string inputFiles = "";
            
            foreach (string inputSource in input.Split(','))
            {
                if (!opList.ContainsKey(inputSource))
                    inputFiles +=inputSource +",";         
            }
            if (inputFiles!="")
                inputFiles = inputFiles.Remove(inputFiles.Length - 1);
             
            //send creation request
            pcs.createOperator(id, inputFiles, rep_fact, routing, addrs, operator_spec, logginglvl, semantics);
            opList.Add(id, addrs);

            //connect to the new operator
            connectOp(id);

            //connect operators for input passing
            IOperator[] ops;
            foreach (string inputID in input.Split(','))
            {
                if (opList.ContainsKey(inputID))
                {
                    ops = opObj[inputID];

                    foreach (IOperator op in ops)
                        op.registerOutputOP(id, addrs);

                    foreach (IOperator replica in opObj[id])
                    {
                        replica.registerInputOP(inputID, opList[inputID]);
                    }
                }
            }

            parentForm.Invoke(new AddOperatorDel(parentForm.AddOperator), id);

        }

        private void connectOp(string opID)
        {

            string[] addrs;
            opList.TryGetValue(opID, out addrs);
            int size = addrs.Length;
            string url;
            IOperator[] ops = new IOperator[size];
            TcpChannel[] channels = new TcpChannel[size];
            for(int i=0; i<size; i++)  //connect to all replicas
            {
                IDictionary propBag = new Hashtable();
                propBag["name"] = opID+"-"+i;
                propBag["timeout"] = 2000;
                TcpChannel channel = new TcpChannel(propBag, null, null);
                ChannelServices.RegisterChannel(channel, false);

                url = addrs[i];

                IOperator op = (IOperator)Activator.GetObject(typeof(IOperator), url);

                ops[i] = op;
                channels[i] = channel;

                op.registerPuppetMaster(puppetMasterIP);

            }
            opChannel.Add(opID, channels);
            opObj.Add(opID, ops);
        }

        private void conectPCS(string machineIP)
        {
           
            IDictionary propBag = new Hashtable();
            propBag["name"] = machineIP;
            TcpChannel channel = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(channel, false);
            IProcessCreationService obj = (IProcessCreationService)Activator.GetObject(
                typeof(IProcessCreationService), Util.createURL(machineIP, Util.PCS_PORT, Util.PCS_SERVICE_NAME));

            pcsList.Add(machineIP, obj);
            channelList.Add(machineIP, channel);

            obj.registerPuppetMaster(puppetMasterIP);
        }


        //execute the next instruction of the file
        //at line = instructionindex  clear empty lines (?)
        public void executeNext()
        {
            if (instructionIndex >= fileLines.Length)
            {
                logServer.logAction("No more Instructions");
                return;
            }
            while (fileLines[instructionIndex] == "" | fileLines[instructionIndex].StartsWith("%"))
            {
                instructionIndex++;
            }

            string instruction =  fileLines[instructionIndex];
            parseInstruction(instruction);
            instructionIndex++;
        }

        public void parseInstruction(string instruction)
        {
            string op;
            int replica;
            int time;
            if (instruction.StartsWith(Util.START_COMMAND))
            {
                op = instruction.Split(null)[1];
                startOperator(op);
            }

            else if (instruction.StartsWith(Util.STATUS_COMMAND))
            {
                requestStatus();
            }
            else if (instruction.StartsWith(Util.FREEZE_COMMAND))
            {
                op = instruction.Split(null)[1];
                replica = Int32.Parse(instruction.Split(null)[2]);
                freezeOperator(op, replica);
            }
            else if (instruction.StartsWith(Util.UNFREEZE_COMMAND))
            {
                op = instruction.Split(null)[1];
                replica = Int32.Parse(instruction.Split(null)[2]);
                unfreezeOperator(op, replica);
            }
            else if (instruction.StartsWith(Util.CRASH_COMMAND))
            {
                op = instruction.Split(null)[1];
                replica = Int32.Parse(instruction.Split(null)[2]);
                crashOperator(op, replica);
            }
            else if (instruction.StartsWith(Util.WAIT_COMMAND))
            {
                time = Int32.Parse(instruction.Split(null)[1]);
                Thread.Sleep(time);
            }
            else if (instruction.StartsWith(Util.INTERVAL_COMMAND))
            {
                op = instruction.Split(null)[1];
                time = Int32.Parse(instruction.Split(null)[2]);
                setOperatorInterval(op, time);
            }
        }

        public void crashOperator(string op, int replica)
        {
            logServer.logAction(Util.CRASH_COMMAND +" "+ op + " " + replica);
            IOperator[] callOps = opObj[op];
            if (replica == -1)
            {
                foreach (IOperator callOp in callOps) //terminate all replicas
                {
                    RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(callOp.crash); //chamada assincrona de terminação 
                    IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
                    opList.Remove(op);
                    opObj.Remove(op);
                    parentForm.Invoke(new AddOperatorDel(parentForm.removeOperator), op);
                }
            }
            else
            {
                IOperator callOp = callOps[replica];
                RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(callOp.crash); //chamada assincrona de terminação 
                IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
                //FIXME dont remove operator from form only the crashed replica
              //  parentForm.Invoke(new AddOperatorDel(parentForm.removeOperator), op);
            }
        }

        public void unfreezeOperator(string op, int replica)
        {
            logServer.logAction(Util.UNFREEZE_COMMAND + " " + op+ " " +replica);
            IOperator[] callOps = opObj[op];
            IOperator callOp = callOps[replica];
            RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(callOp.unfreeze);
            IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
        }

        public void freezeOperator(string op, int replica)
        {
            logServer.logAction(Util.FREEZE_COMMAND + " " + op+ " "+replica);
            IOperator[] callOps = opObj[op];
            IOperator callOp = callOps[replica];
            RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(callOp.freeze);
            IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);

        }

        public void requestStatus()
        {
            logServer.logAction(Util.STATUS_COMMAND);
            foreach (IOperator[] callOps in opObj.Values)
            {
                foreach (IOperator callOp in callOps)
                {
                    RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(callOp.status);
                    IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
                }
            }

            foreach (IProcessCreationService pcs in pcsList.Values)
                pcs.status();
        }

        private void setOperatorInterval(string op, int time)
        {
            logServer.logAction(Util.INTERVAL_COMMAND + " " + op+ " "+time);
            IOperator[] callOps = opObj[op];
            foreach (IOperator callOp in callOps)
            {
                RemoveAsyncDelegateInt RemoteDel = new RemoveAsyncDelegateInt(callOp.setInterval);
                IAsyncResult RemAr = RemoteDel.BeginInvoke(time, null, null);
            }
        }

        public void startOperator(string op)
        {
            logServer.logAction(Util.START_COMMAND + " " + op);
            IOperator[] callOps = opObj[op];
            foreach (IOperator callOp in callOps)
            {
                RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(callOp.start);
                IAsyncResult RemAr = RemoteDel.BeginInvoke(null, null);
            }
        }

        //execute all the instructions in the file
        public void executeAll()
        {
            while (instructionIndex < fileLines.Length)
                executeNext();
        }
        //kill all operators and exit
        public void terminate()
        {
            crashAllOperarors();

            //also kill pcs for testing simplicity 
            IProcessCreationService ipcs;
            foreach(string pcs in pcsList.Keys)
            {
                ipcs = pcsList[pcs];
                RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(ipcs.kill);
                RemoteDel.BeginInvoke(null, null);
            }

            opObj.Clear();
            opChannel.Clear();
            opList.Clear();
            pcsList.Clear();
            channelList.Clear();
        }

        public int getReplicas(string opID)
        {
            return opObj[opID].Length;
        }

    }
}
