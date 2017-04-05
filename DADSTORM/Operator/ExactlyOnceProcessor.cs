using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Operator
{
    public class ExactlyOnceProcessor : AtLeastOnceProcessor
    {
        //tuples that were processed
        private List<string> processedList = new List<string>();

        protected Replicator replicator = null;

        public ExactlyOnceProcessor(string inputs, string opSpec, string specInfo, Messenger msgr, Receiver rcv, int rep, string id, string addr, string[] replicas) : base(inputs, opSpec, specInfo, msgr, rcv)
        {
            if (rep > 1)
            {
                replicator = new Replicator(id, addr, replicas, this);
                Thread t = new Thread(replicator.start);
                t.Start();
            }
        }

        public void registerInput(StormTuple tuple)
        {
            lock (processQueue)
            {
                if (processedList.Contains(tuple.id) | processQueue.ContainsKey(tuple.id))
                    return;

        /*        else if (replicator != null && !replicator.check(tuple.id))
                    processedList.Add(tuple.id);
*/
                else
                {
                    processQueue.Add(tuple.id, tuple);
                    Monitor.Pulse(processQueue);
                }
            }
        }
        
        public override void addInput(StormTuple tuple)
        {
            if (replicator != null)
                replicator.requestAproval(tuple);
            else
                registerInput(tuple);

        }

        public override void markAsProcessed(string tuple, int dir)
        {
            Console.WriteLine("marking as processed " + tuple);
            if (processQueue.ContainsKey(tuple))
                processQueue.Remove(tuple);

            processedList.Add(tuple);
            
            replicator.propagate(tuple, dir);
            
        }

        protected override void process(StormTuple tuple)
        {

            processedList.Add(tuple.id);
            base.process(tuple);

            if (replicator != null)
                replicator.replicate(tuple.id);
        }

        public override bool checkTuple(string id)
        {
            return (processedList.Contains(id) | processQueue.ContainsKey(id)); //true = vou processar/já processei
        }
    }
}
