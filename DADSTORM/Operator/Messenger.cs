using SharedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Operator
{
    /**
     * CLASS TO TAKE CARE OF THE DELIVERY PROBLEMS 
     **/
    public class Messenger
    {
        //save channels
        private Dictionary<string, TcpChannel[]> channels = new Dictionary<string, TcpChannel[]>();
        //save proxys
        private Dictionary<string, IOperator[]> OutputOps = new Dictionary<string, IOperator[]>();
        //
        private Dictionary<string, int[]> OpFailures = new Dictionary<string, int[]>();
        //send queue 
        private Dictionary<string, StormTuple> sendQueue = new Dictionary<string, StormTuple>();

        private Dictionary<string, int> confirmations = new Dictionary<string, int>();
        private List<string> deadList = new List<string>();

        private Random randomizer = new Random();
        private IPuppetMaster PuppetMaster;

        private Receiver frontEnd;

        private string routingPolicy;
        private string semantic = Util.SEMANTICS[0];
        private int routingField;
        private string loggingLvl;
        private string opURL;
        private string opStatus = Util.REGULAR_STATUS;
        public int tupleCounter = 0;
        public int confirmedTuples = 0;


        public Messenger(string routingP, int routingF, string semantics, string logginlvl, string url, Receiver rcv)
        {
            routingPolicy = routingP;
            routingField = routingF;
            semantic = semantics;
            loggingLvl = logginlvl;
            opURL = url;
            frontEnd = rcv;
        }

        public void registerPuppetMaster(string ip)
        {
            string url = "tcp://" + ip + ":10000/PuppetMasterService";
            IDictionary propBag = new Hashtable();
            propBag["name"] = opURL + "-PuppetMaster";
            TcpChannel pmch = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(pmch, false);
            PuppetMaster = (IPuppetMaster)Activator.GetObject(typeof(IPuppetMaster), url);

            Console.WriteLine("Connected to PM");
        }

        public void addToQueue(StormTuple t)
        {
            if (OutputOps.Count == 0 && String.Equals(loggingLvl, Util.LOG_POLICIES[1]))
            {
                PuppetMaster.logAction("tuple " + opURL + ", " + t.ToString());
                return;
            }

            lock (sendQueue)
            {
                //trash tuples that are aleady in the send queue 
                if (!sendQueue.ContainsKey(t.id) && !confirmations.ContainsKey(t.id))
                {
                    if (! String.Equals(semantic, Util.SEMANTICS[0])) //with at most once semantic we dont need to care about confirmations
                        confirmations.Add(t.id, 0);
                    sendQueue.Add(t.id, t);
                    Monitor.Pulse(sendQueue);
                }
            }
        }

        //connect to the operators 
        public void connectOperator(string outputOP, string[] addrs)
        {
            IDictionary propBag = new Hashtable();
            int size = addrs.Length;
            TcpChannel[] opchannels = new TcpChannel[size];
            IOperator[] operators = new IOperator[size];
            for (int i = 0; i < size; i++)
            {
                propBag["name"] = outputOP + i;
                propBag["timeout"] = 3000;
                TcpChannel channel = new TcpChannel(propBag, null, null);
                ChannelServices.RegisterChannel(channel, false);
                IOperator op = (IOperator)Activator.GetObject(typeof(IOperator), addrs[i]);
                opchannels[i] = channel;
                operators[i] = op;
            }
            int[] init = new int[size];

            for (int i = 0; i < size; i++)
                init[i] = 0;

            OpFailures[outputOP] = init;

            channels.Add(outputOP, opchannels);
            OutputOps.Add(outputOP, operators);
        }

        private int applyRouting(StormTuple tuple, string opID)
        {
            int replicas = OutputOps[opID].Length;
            switch (routingPolicy)
            {
                case ("primary"):
                    return 0;
                case ("hashing"):
                    MD5 md5 = MD5.Create();
                    string field = tuple.get(routingField);
                    byte[] inputBytes = Encoding.ASCII.GetBytes(field);
                    byte[] hash = md5.ComputeHash(inputBytes);
                    //Get a random byte from the hash and convert it to int
                    int rep = Convert.ToInt32(hash[randomizer.Next(hash.Length)]);

                    while (rep >= 10)
                        rep /= 10;
                    
                    if (rep >= replicas)
                        rep = rep % replicas;

                    Console.WriteLine("Sending to rep: " + rep);

                    return rep;
                case ("random"):
                    return randomizer.Next(replicas);
                default:
                    return -1;
            }
        }

        private void send(StormTuple tuple)
        {
            IOperator op;

            foreach (string  outputOp in OutputOps.Keys.ToList())
            {
                int repIndex;
                do
                {
                    repIndex = applyRouting(tuple, outputOp);

                }
                while (deadList.Contains(outputOp + "-" + repIndex));   

                op = OutputOps[outputOp][repIndex]; //choose replica to send


                if (String.Equals(semantic, Util.SEMANTICS[0])) //at most once 
                {
                    try
                    {
                        op.registerInput(tuple);                //send and thats it 
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(" TIMEOUT ");
                        OpFailures[outputOp][repIndex]++;
                        if (OpFailures[outputOp][repIndex] > Util.MAX_FAILURES)
                            declareDead(outputOp, repIndex);
                    }
                    finally
                    {
                        sendQueue.Remove(tuple.id);
                        tupleCounter++;
                    }
                }
                else if (String.Equals(semantic, Util.SEMANTICS[1]) | String.Equals(semantic, Util.SEMANTICS[2]) ) //at least once or exacly once
                { //sending process is equal only the processing should be different
                    try
                    {
                        op.registerInput(tuple);
                        tupleCounter++;
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(" TIMEOUT ");
                        OpFailures[outputOp][repIndex]++;
                        if (OpFailures[outputOp][repIndex] > Util.MAX_FAILURES)
                            declareDead(outputOp, repIndex);
                    }

                }
            }

            if (String.Compare(loggingLvl, Util.LOG_POLICIES[1]) == 0)
                PuppetMaster.logAction("tuple " + opURL + ", " + tuple.ToString());
            Thread.Sleep(100);

        }

        private void declareDead(string op, int rep)
        {
            Console.WriteLine("Declaring Dead: " + op + "  " + rep);
            deadList.Add(op + "-"+rep);
        }


        public void start()
        {
            while (true)
            {
                lock (this)
                {
                    if (String.Equals(opStatus, Util.FREEZE_STATUS))
                        Monitor.Wait(this);
                }

                while (sendQueue.Count == 0)
                {
                    lock (sendQueue)
                    {
                        Monitor.Wait(sendQueue);
                    }
                }
                send(sendQueue.First().Value);
            }
        }

        //remove a tuple from the send queue when we recive a confirmation from the next operator
        public void confirm(string tuple)
        {
            if (!confirmations.ContainsKey(tuple)) //means that the tuple was already confirmed
                return;
            confirmations[tuple]++;
            if (confirmations[tuple] == OutputOps.Count) //all operators that the tuple was sent have confirmed 
            {
                sendQueue.Remove(tuple);
                confirmedTuples++;
            }

        }

        public void freeze()
        {
            lock (this)
            {
                opStatus = Util.FREEZE_STATUS;
            }
        }

        public void unfreeze()
        {
            lock (this)
            {
                opStatus = Util.REGULAR_STATUS;
                Monitor.PulseAll(this);
            }
        }
    }
}
