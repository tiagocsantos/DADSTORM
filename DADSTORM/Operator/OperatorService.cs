using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace Operator
{
    class OperatorService
    {
        static void Main(string[] args)
        {
            string id = args[0];//operator id
            string inputs = args[1]; //input operators/files
            string addr = args[2]; //operator addr
            string routing = args[3]; //routing policy
            string logging = args[4]; //logging policy
            string semantics = args[5]; //deliver semantic
            string opSpec = args[6]; //operator mode
            string specInfo = args[7]; //operator mode info
            int rep = Int32.Parse(args[8]); //rep factor
            string[] addrs = new string[rep]; //addrs of every replica

            for (int i = 0; i < rep; i++)
            {
                addrs[i] = args[9 + i];
            }

            int port = Util.getPortFromAddr(addr);

            string routingPolicy = routing.Split('(')[0];
            int routing_field = -1;
            if (String.Compare(routingPolicy, "hashing") == 0)
                routing_field = Int32.Parse(routing.Split('(')[1].Split(')')[0]);

            Receiver op = new Receiver(id, inputs, opSpec, specInfo, routingPolicy, routing_field, addr, logging, semantics, rep, addrs);

            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(op, "op", typeof(Receiver));
            Console.ReadKey();

        }

        /*
        static void Main(string[] args)
        {
            string id = args[0];
            string inputs = args[1];
            string addr = args[2];
            string routing = args[3];
            string logging = args[4];
            string semantics = args[5];
            string opSpec = args[6];
            string specInfo = args[7];
            int rep = Int32.Parse(args[8]);
            string[] addrs = new string[rep-1];

            for (int i = 0; i < rep - 1; i++)
            {
                addrs[i] = args[9 + i];
            }

            Console.WriteLine(routing);

            string port = addr.Split('/')[2].Split(':')[1];
            
            string routingPolicy = routing.Split('(')[0];
            int routing_field = -1;
            if (String.Compare(routingPolicy, "hashing") == 0)
                routing_field = Int32.Parse(routing.Split('(')[1].Split(')')[0]);

            Operator op = new Operator(id, inputs, opSpec, specInfo, routingPolicy, routing_field, addr, logging, semantics, rep, addrs);

            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(op, "op", typeof(Operator));
            Console.ReadKey();
        }
        */
    }
}
