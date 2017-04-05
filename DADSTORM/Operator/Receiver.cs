using SharedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Operator
{
    //frontend class of the operator
    //receives all remote calls and tuples
    public class Receiver : MarshalByRefObject, IOperator
    {
        private string OpStatus = Util.REGULAR_STATUS;
        private string OpID;

        private Processor processor;
        private Messenger messenger;

        private Dictionary<string, IOperator[]> inputOPS = new Dictionary<string, IOperator[]>();
        private Dictionary<string, TcpChannel[]> channels = new Dictionary<string, TcpChannel[]>();

        private Dictionary<string, int> failures = new Dictionary<string, int>();
        private List<string> deadList = new List<string>();

        public Receiver(string id, string inputs, string opSpec, string specInfo, string routingPolicy, 
            int routing_field, string addr, string logging, string semantics, int rep, string[] addrs)
        {
            OpID = id;
            messenger = new Messenger(routingPolicy, routing_field, semantics, logging, addr, this);
            if (String.Equals(semantics, Util.SEMANTICS[0]))
                processor = new Processor(inputs, opSpec, specInfo, messenger, this);
            else if (String.Equals(semantics, Util.SEMANTICS[1]))
                processor = new AtLeastOnceProcessor(inputs, opSpec, specInfo, messenger, this);
            else if (String.Equals(semantics, Util.SEMANTICS[2]))
                processor = new ExactlyOnceProcessor(inputs, opSpec, specInfo, messenger, this, rep, id, addr, addrs);

        }

        public void crash()
        {
            Process.GetCurrentProcess().Kill();
        }

        public void freeze()
        {
            lock (this)
            {
                OpStatus = Util.FREEZE_STATUS;
            }
            messenger.freeze();
        }

        public void unfreeze()
        {
            lock (this)
            {
                OpStatus = Util.REGULAR_STATUS;
                Monitor.PulseAll(this);
            }

            messenger.unfreeze();
        }

        public void registerInput(StormTuple tuple)
        {
            lock (this)
            {
                while (String.Equals(OpStatus, Util.FREEZE_STATUS))
                {
                    Monitor.Wait(this);
                }
            }
            processor.addInput(tuple);
            
        }

        public void registerOutputOP(string opID, string[] addrs)
        {
            messenger.connectOperator(opID, addrs);
        }

        public void registerInputOP(string inputOP, string[] addrs)
        {
            IDictionary propBag = new Hashtable();
            int size = addrs.Length;
            TcpChannel[] opchannels = new TcpChannel[size];
            IOperator[] operators = new IOperator[size];

            for (int i = 0; i < size; i++)
            {
                propBag["name"] = inputOP + i + " input";
                propBag["timeout"] = 3000;
                TcpChannel channel = new TcpChannel(propBag, null, null);
                ChannelServices.RegisterChannel(channel, false);
                IOperator op = (IOperator)Activator.GetObject(typeof(IOperator), addrs[i]);
                opchannels[i] = channel;
                operators[i] = op;

                failures.Add(inputOP + "-" + i, 0);
                
            }

            channels.Add(inputOP, opchannels);
            inputOPS.Add(inputOP, operators);
        }

        public void registerPuppetMaster(string url)
        {
            messenger.registerPuppetMaster(url);
        }

        public void setInterval(int time)
        {
            processor.setInterval(time);
        }

        public void start()
        {
            Thread t1 =  new Thread(processor.start);
            Thread t2 = new Thread(messenger.start);
            t1.Start();
            t2.Start();
        }

        public void status()
        {
            Console.WriteLine("\r\nReporting Status:\r\nOperator Status: " + OpStatus + "\r\nProcessed Tuples: " + processor.tupleCounter[0]
                + "\r\nForwarded Tuples: " + processor.tupleCounter[1] + "\r\nSent Tuples: " + messenger.tupleCounter);
        }

        public void confirm(string s)
        {
            messenger.confirm(s);

        }

        public void sendConfirmation(string s)
        {
           foreach (string ops in inputOPS.Keys)
           {
            for(int i = 0; i<inputOPS[ops].Length; i++)
                {
                    string id = ops + "-" + i;
                    if (deadList.Contains(id))
                        return;
                    try
                    {
                        inputOPS[ops][i].confirm(s);
                    }catch(SocketException e)
                    {
                        Console.WriteLine("Exception on Confirming");
                        failures[id]++;
                        if (failures[id] > 3)
                            deadList.Add(id);
                    }
                }
            }
        }
        
        public void markAsProcessed(string s, int d)
        {
            processor.markAsProcessed(s, d);
        }

        public bool checkIfProcessed(string s)
        {
            return processor.checkTuple(s);
        }
        
    }
}
