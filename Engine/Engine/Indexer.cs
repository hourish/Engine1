using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Engine
{
    class Indexer
    {
        string toWrite = "";
        private static int tempPostingFilesCounter = 0;
        Dictionary<string, Pair<int, int>> dictionary = new Dictionary<string, Pair<int, int>>(); //the dictionary of the corpus: term name, df and position

        public void CreateTempPostingFile(Term[] terms, Document currentDoc)
        {

            Array.Sort<Term>((Engine.Term[])terms);
            for (int i = 0; i < terms.Length; i++)//all terms
            {
                if (!dictionary.ContainsKey(terms[i].GetName()))
                {
                    dictionary.Add(terms[i].GetName(), new Pair<int, int>(terms[i].GetDF(), 0));
                }
                else
                    dictionary[terms[i].GetName()].first = terms[i].GetDF(); //update df

                toWrite += terms[i].StringToPosting();
            }
            Console.WriteLine(toWrite);

        }

        public void CreateTempPostingFile(string path)
        {
            string newPath = path + "\\TempPostingFileNumber_" + tempPostingFilesCounter;
            File.WriteAllText(newPath, toWrite);
            tempPostingFilesCounter++;
            toWrite = "";
        }
    }
}
