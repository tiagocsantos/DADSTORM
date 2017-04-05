using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    [Serializable()]
    public class StormTuple
    {
        private string[] fields;
        public string id = Util.getID();

        public StormTuple(string s)
        {
            fields =  s.Replace("\"", "").Replace(" ", "").Split(',');
        }
        public StormTuple(string[] s)
        {
            fields = s;
        }

        public StormTuple(int tupleCounter)
        {
            fields = new string[] { tupleCounter.ToString() };
        }

        public StormTuple(IList<IList<string>> result)
        {
            int i = 0;
            fields = new string[result.Count()];
            foreach (IList<string> l in result)
            {
                foreach (string s in l)
                {
                    fields[i] = s;
                }
                i++;
            }
        }

        public string get(int index)
        {
            return fields[index-1];
        }

        override
        public string ToString()
        {
            string s = "<";
            foreach(string field in fields)
            {
                s += field + "-";
            }
            s = s.Remove(s.Length - 1);
            s += ">";

            return s;
        }

        public string[] getAll()
        {
            return fields;
        }

        public bool compare(string filter, int field, string mode)
        {
            switch (mode) {
                case ("="):
                    return String.Compare(this.get(field), filter) == 0;

                case ("<"):
                    return String.Compare(this.get(field), filter) < 0;

                case (">"):
                    return String.Compare(this.get(field), filter) > 0;
                default:
                    return false;

            }
        }




    }
}
