
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SharedTypes
{
    public class Util
    {
        public static int PUPPET_MASTER_PORT = 10000;
        public static int PCS_PORT = 10001;

        public static string PCS_SERVICE_NAME = "ProcessCreationService";
        public static string PUPPET_MASTER_SERVICE = "PuppetMasterService";
        public static string OPERATOR_SERVICE = "op";

        public static string SEMANTICS_TOKEN = "Semantics";
        public static string LOGGING_TOKEN = "LoggingLevel";
        public static string ROUTING_TOKEN = "routing";
        public static string REP_TOKEN = "rep_fact";
        public static string INPUT_TOKEN = "input_ops";
        public static string ADDRESS_TOKEN = "address";
        public static string SPEC_TOKEN = "operator_spec";
        public static string COMMENT_TOKEN = "%";

        public static string[] TOKENS = { INPUT_TOKEN, REP_TOKEN, ROUTING_TOKEN, ADDRESS_TOKEN, SPEC_TOKEN };
        public static string[] ROUTING_POLICIES =  { "primary", "hashing", "random"}; 
        public static string[] MODES = { "UNIQ", "COUNT", "DUP", "FILTER", "CUSTOM" } ;
        public static string[] LOG_POLICIES = { "light", "full" };
        public static string[] SEMANTICS = { "at-most-once", "at-least-once", "exactly-once" };

        public static string START_COMMAND = "Start";
        public static string FREEZE_COMMAND = "Freeze";
        public static string UNFREEZE_COMMAND = "Unfreeze";
        public static string INTERVAL_COMMAND = "Interval";
        public static string STATUS_COMMAND = "Status";
        public static string CRASH_COMMAND = "Crash";
        public static string WAIT_COMMAND = "Wait";

        public static string FREEZE_STATUS = "FREEZED";
        public static string REGULAR_STATUS = "REGULAR";

        public static int MAX_FAILURES = 3;
        public static int MAX_REP_FAILURES = 5;

        public static string CONFIG_FILES_DIRECTORY = @"..\..\..\ConfigFiles";
        public static string LOG_FILES_DIRECTORY =    @"..\..\..\LogFiles";
        //OUR LIB
        //public static string MYLIB_DIRECTORY = @"..\mylib\bin\Debug\";
        //PROFS LIB
        public static string MYLIB_DIRECTORY = @"..\";

        public static string getID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string createURL(string ip, int port, string servicename)
        {
            return "tcp://" + ip + ":" + port + "/" + servicename;
        }

        public static string getLocalHostIP()
        {
            string localHostIP = null;
            foreach (var addr in Dns.GetHostEntry(string.Empty).AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    localHostIP = addr.ToString();
                    break;
                }
            }
            return localHostIP;
        }

        public static int getPortFromAddr(string addr)
        {
            return Int32.Parse(addr.Split('/')[2].Split(':')[1]);
        }


    }
}