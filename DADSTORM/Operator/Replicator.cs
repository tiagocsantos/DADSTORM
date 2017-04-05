using SharedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Operator
{
    /**
     *Class responsible for coordination among same operator replicas
     */
    public class Replicator
    {
        private List<TcpChannel> channels = new List<TcpChannel>();
        //save proxys
        private Dictionary<int, IOperator> replicas = new Dictionary<int, IOperator>();
        private List<string> replicatedTuples = new List<string>();
        private Dictionary<int , int > failures = new Dictionary<int, int>();

        private Dictionary<string, StormTuple> confirmBuffer = new Dictionary<string, StormTuple>();

        private string opID;
        private int myIndex;
        private List<string> replicasAddr;

        private ExactlyOnceProcessor processor;

        public Replicator(string opid, string myAddr, string[] replicas, ExactlyOnceProcessor p)
        {
            opID = opid;
            replicasAddr = new List<string>(replicas);
            processor = p;

            myIndex = replicasAddr.IndexOf(myAddr);
            Console.WriteLine("i am " + myIndex);
            //connect to the replica before in the "chain"
            if (myIndex > 0)
                connectToReplica(myIndex - 1, replicas[myIndex - 1]);
            //connext to the replica after in the "chain"
            if (myIndex < replicas.Length - 1)
                connectToReplica(myIndex + 1, replicas[myIndex + 1]);
        }

        private void connectToReplica(int i, string addr)
        {
            Console.WriteLine("connecting to " + i);
            IDictionary propBag = new Hashtable();
            propBag["name"] = opID + "replica" + i;
            propBag["timeout"] = 1000;
            TcpChannel channel = new TcpChannel(propBag, null, null);
            ChannelServices.RegisterChannel(channel, false);
            IOperator op = (IOperator)Activator.GetObject(typeof(IOperator), addr);

            channels.Add(channel);
            replicas.Add(i, op);
            failures.Add(i, 0);
        }

        public void replicate(string tuple)
        {

            replicatedTuples.Add(tuple);
            
            foreach (int replica in replicas.Keys)
            {
                if (failures[replica] > Util.MAX_REP_FAILURES)
                    return;
                try
                {
                    replicas[replica].markAsProcessed(tuple, myIndex < replica ? 1 : -1);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Exception on Replicating");
                    newFail(replica);
                }
            }
        }

        public void propagate(string tuple, int direction)
        {
            int index = -1;
            if ((direction == 1) && (myIndex < replicas.Count - 1)) //up in the chain 
                index = myIndex + 1;

            else if ((direction == -1) && (myIndex > 0)) //down in the chain
                index = myIndex - 1;


            try
            {   if (index !=-1 && failures[index]< Util.MAX_REP_FAILURES)
                    replicas[index].markAsProcessed(tuple, direction);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Exception on Propagating");
                newFail(index);
            }
        }

        public bool check(string tuple)
        {         
            foreach(int replica in replicas.Keys)
            {
                if (failures[replica] > Util.MAX_REP_FAILURES)
                    return true;

                Console.WriteLine("asking replica " + replica);
                try
                {
                    if (replicas[replica].checkIfProcessed(tuple)) //se já o processsou
                        return false;       //esta replica nao o pode processar
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Replica not Responding, we need to move on");
                    newFail(replica);
                    return true; //caso constrario pode avançar
                }
            }
            return true; //caso constrario pode avançar
        }

        private void newFail(int rep)
        {
            failures[rep]++;
        }

        public void requestAproval(StormTuple t)
        {
            lock (confirmBuffer)
            {
                if (!confirmBuffer.ContainsKey(t.id))
                {
                    confirmBuffer.Add(t.id, t);
                    Monitor.Pulse(confirmBuffer);
                }
            }
        }

        public void start()
        {
            while (true)
            {
                lock (confirmBuffer)
                {
                    Monitor.Wait(confirmBuffer);
                }

                StormTuple t = confirmBuffer.Last().Value;
                if (check(t.id))
                {
                    processor.registerInput(t);
                }
            }
        }
    }
}
