using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace mylib
{
    public class OutputOperator
    {
        public IList<IList<string>> Operate(IList<string> l)
        {
            string outputFile = @".\output.txt";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputFile, true))
            {
                foreach (string line in l)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(line);
                }
            }
            IList<IList<string>> result = new List<IList<string>>();
            result.Add(l);
            return result;
        }
    }

    public class QueryFollowersFile
    {
        private static Dictionary<string, List<string>> dictTweeterFollower = new Dictionary<string, List<string>>();
        private static bool dictLoaded = false;

        private static void loadFollowersDict()
        {
            string followersFilepath = @".\followers.dat";

            System.IO.StreamReader followersFile = new System.IO.StreamReader(followersFilepath, true);
            string line = followersFile.ReadLine();
            List<string> followers;
            while (line != null)
            {
                if (line[0] != '%')
                {
                    string[] tokens = line.Split(',');
                    followers = new List<string>();
                    if (dictTweeterFollower.ContainsKey(tokens[0]))
                    {
                        followers = dictTweeterFollower[tokens[0]];
                    }
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        followers.Add(tokens[i]);
                    }
                    if (!dictTweeterFollower.ContainsKey(tokens[0]))
                    {
                        dictTweeterFollower.Add(tokens[0], followers);
                    }
                }
                line = followersFile.ReadLine();
            }
            dictLoaded = true;
        }


        public IList<IList<string>> getFollowers(IList<string> inputTuple)
        {
            IList<IList<string>> outputTuples = new List<IList<string>>();
            List<string> tuple;

            if (!dictLoaded) loadFollowersDict();
            if (dictTweeterFollower.ContainsKey(inputTuple[1]))
            {
                foreach (string follower in dictTweeterFollower[inputTuple[1]])
                {
                    tuple = new List<string>();
                    tuple.Add(follower);
                    outputTuples.Add(tuple);
                }
            }
            return outputTuples;
        }
    }

    public class IdentityOperator
    {
        public IList<IList<string>> CustomOperation(IList<string> l)
        {
            IList<IList<string>> result = new List<IList<string>>();
            result.Add(l);
            return result;
        }
    }

}
