using SharedTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Operator
{
    //class responsible for the processing of tuples
    //receives a tuple from the receiver processes it and forwards the result to the messenger
    public class Processor
    {
        protected Messenger messenger;
        protected Receiver frontEnd;

        protected string[] inputFiles = null;

        protected string operatorMode;
        protected string modeInfo;
        protected int timeInterval = 0;

        private int aux = 0;

        public int[] tupleCounter = { 0, 0 };

        //when working with the al-least-once we need to use list so repeated tuples can be acepted do processing (duplicate ids)
        protected Dictionary<string, StormTuple> processQueue = new Dictionary<string, StormTuple>(); 

        public Processor(string inputs, string opSpec, string specInfo, Messenger msgr, Receiver rcv)
        {
            operatorMode = opSpec;
            modeInfo = specInfo;
            messenger = msgr;
            frontEnd = rcv;

            if (String.Compare(inputs, "none") != 0)
                inputFiles = inputs.Split(',');
        }


        public void setInterval(int time)
        {
            timeInterval = time;
        }

        public virtual void addInput(StormTuple tuple)
        {
            lock (processQueue)
            {
                processQueue.Add(Util.getID(), tuple);
                Monitor.Pulse(processQueue);
            }
        }

        public virtual void markAsProcessed(string tuple, int direction)
        {
            //only for exactly once
        }

        public virtual bool checkTuple(string id)
        {
            //only for exactly once
            return true;
        }

        protected void process(string line)
        {
            StormTuple tuple = new StormTuple(line);
            process(tuple);
        }

        protected virtual void process(StormTuple tuple)
        {
            Console.WriteLine("Processing " + tuple.id);
            StormTuple result = null;
            if (string.Compare(operatorMode, Util.MODES[0]) == 0)
                result = unique(tuple);
            else if (string.Compare(operatorMode, Util.MODES[1]) == 0)
                result = count(tuple);
            else if (string.Compare(operatorMode, Util.MODES[2]) == 0)
                result = dup(tuple);
            else if (string.Compare(operatorMode, Util.MODES[3]) == 0)
                result = filter(tuple);
            else if (string.Compare(operatorMode, Util.MODES[4]) == 0)
                result = costum(tuple);

            if (result != null)
            {
                messenger.addToQueue(result);
                tupleCounter[1]++;
            }
        
            tupleCounter[0]++;
        }

        private StormTuple costum(StormTuple tuple)
        {
            string[] param = modeInfo.Split(',');
            byte[] code = File.ReadAllBytes(Util.MYLIB_DIRECTORY + param[0]);
            Assembly assembly = Assembly.Load(code);

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == true)
                {
                    if (type.FullName.EndsWith("." + param[1]))
                    {
                        object classObj = Activator.CreateInstance(type);
                        List<string> l = new List<string>();
                        foreach (string value in tuple.getAll())
                            l.Add(value);

                        object[] args = new object[] { l };

                        object resultObject = type.InvokeMember(param[2],BindingFlags.Default|BindingFlags.InvokeMethod, null,classObj,args);

                        IList<IList<string>> result = (IList<IList<string>>)resultObject;
                        return new StormTuple(result);
                    }
                }
            }
            return null;
        }

        private StormTuple filter(StormTuple tuple)
        {
            int field = Int32.Parse(modeInfo.Split(',')[0]);
            string comparer = modeInfo.Split(',')[1];
            string filter = modeInfo.Split(',')[2];

            if (tuple.compare(filter, field, comparer))
                return tuple;
            else
                return null;
        }

        private StormTuple dup(StormTuple tuple)
        {
            return tuple;
        }

        private StormTuple count(StormTuple tuple)
        {
            return new StormTuple(tupleCounter[1]);
        }

        private StormTuple unique(StormTuple tuple)
        {
            int index = Int32.Parse(modeInfo);
            if (!tuple.getAll().Skip(index).Contains(tuple.get(index)))
                return tuple;
            else return null;
        }

        public void start()
        {
            //lê primeiro todos os ficheiros indicados
            if (inputFiles != null)
            {
                foreach (string input in inputFiles)
                {
                    StreamReader fileReader = new StreamReader(input);
                    string line;
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        process(line);
                        Thread.Sleep(timeInterval);
                    }
                }
            }
            //só depois passa a processar os tuplos na queue
            while (true)
            {
                while(processQueue.Count == 0)
                {
                    lock (processQueue)
                    {
                        Monitor.Wait(processQueue);
                    }
                }
                process(processQueue.First().Value);
                processQueue.Remove(processQueue.First().Key);

                Thread.Sleep(timeInterval);
            }
        }
    }
}
