using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Engine
{
    class Parser
    {
        HashSet<string> stopWords = new HashSet<string>();

        public Parser(string path)
        {
           ReadStopWords(path);
        }
        public void Parse(string str)
        {
            string[] words = str.Split(' ');
            Document currentDoc = new Document(words[1]);//because str started after <DOCNO>
            for (int i = 2; i < words.Length; i++)
            {
                if (words[i] == "" || stopWords.Contains(words[i]))
                    continue;
                if()
            }
        }

        private void ReadStopWords(string path)
        {
            StreamReader sr = new StreamReader(path);
            string line = "";
            while((line = sr.ReadLine()) != null)
            {
                stopWords.Add(line);
            }
        }

        private string NumberCase (string term)
        {
            double d;
            if (double.TryParse(term[0], d))
            {
                if (term[term.Length].Equals("%"))
                {
                    term = term.Substring(0, term.Length - 1);
                    term = term + " percent";
                    return term;
                }





            }





            }






        }
    }
}
