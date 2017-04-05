using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTypes;

namespace Operator
{
    public class AtLeastOnceProcessor : Processor
    {

        public AtLeastOnceProcessor(string inputs, string opSpec, string specInfo, Messenger msgr, Receiver rcv) : base(inputs, opSpec, specInfo, msgr, rcv)
        {
        }

        protected override void process(StormTuple tuple)
        {
            base.process(tuple);
            frontEnd.sendConfirmation(tuple.id);
        }

    }
}
