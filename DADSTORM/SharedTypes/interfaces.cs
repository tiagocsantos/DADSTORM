using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    public interface IPuppetMaster
    {
        void logAction(string message);
    }

    public interface IProcessCreationService
    {
        void createOperator(string id, string input, string rep_fact, string routing, string[] addrs, string operator_spec, string loggingLvl, string semantics);
        void registerPuppetMaster(string url);
        void UnregisterPuppetMaster();
        void status();
        void kill();
    }

    public interface IOperator
    {
        //stop an operator when the puppet master is stopped
        void crash();
        void registerOutputOP(string opID, string[] addrs);
        void registerInputOP(string opID, string[] adrs);
        void registerPuppetMaster(string url);
        void freeze();
        void unfreeze();
        void start();
        void setInterval(int time);
        void status();
        void registerInput(StormTuple tuple);
        void confirm(string s);
        void markAsProcessed(string s, int d);
        bool checkIfProcessed(string s);
    }
}
