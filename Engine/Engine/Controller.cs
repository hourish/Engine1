using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Engine
{
    class Controller
    {
        public void Engine(string path)
        {
            int currentSize = 0;
           /* long sizeOfCorpus = 0;
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] diArr = di.GetDirectories();
            for(int i = 0; i < diArr.Length; i++)
            {
                FileInfo[] fi = diArr[i].GetFiles();
                for(int j = 0; j < fi.Length; j++)
                {
                    sizeOfCorpus += fi[j].Length;
                }
            }
            */
            ReadFile rf = new ReadFile(path);
            HashSet<string> stopWords = new HashSet<string>();
            stopWords = rf.ReadStopWords(path + "\\stop_words.txt");
            Parser parser = new Parser(stopWords);
            int filesAmount = rf.FilesAmount();//num of files in the given dictionary
            Dictionary<string, Term> terms = new Dictionary<string, Term>();
            Indexer indexer = new Indexer();
            Document currentDoc = null;
            string file = "";
            //Console.WriteLine("10% of corpus is " +sizeOfCorpus / 10);
            for (int i = 0; i < filesAmount; i++)//going through the files in the dictionery and send each to the parser 
            {
                file = rf.ReadText(i);
                //currentSize += ASCIIEncoding.Unicode.GetByteCount(file);
               // currentSize++;
                Match matchTEXT = rf.Seperate(file);// get a sperated files from red file
              /*  if(currentSize >= 100)//more than 10 percent
                {
                    //indexer.CreateTempPostingFile(path);
                    currentSize = 0;
                    string str = "";
                    foreach(string s in terms.Keys)
                    {
                        str += s + "\n";
                    }
                    File.WriteAllText(@"C:\Users\Shani\Desktop\study\first semester\Ihzur\Engine\corpus\bayush", str);
                    terms.Clear();
                }*/
               // if (matchTEXT != null)
             //   {
                    while (matchTEXT.Success)
                    {
                        terms = parser.Parse(matchTEXT.Groups[1].Value);
                       // currentDoc = parser.GetDoc();
                      //  indexer.PrepareToPosting(terms.Values.ToArray(), currentDoc, 5);
                      //  currentDoc.SetMaxTF();
                        matchTEXT = matchTEXT.NextMatch();
                      /*  StringBuilder sb = new StringBuilder();
                        foreach (string s in terms.Keys)
                            sb.Append(s).Append("\n");
                     File.WriteAllText(@"C:\Users\Shani\Desktop\study\first semester\Ihzur\Engine\corpus\bdika", sb.ToString());*/
                    terms.Clear();//להוריד@@@@@@#$%^&^%$#$%^&*&^%$#
                    }
               // }
                /*   if ((i % 2) == 0 && i > 0)
                   {
                       indexer.CreateTempPostingFile(path);
                       terms.Clear();

                   }
                   if (i == 4)
                   {
                       indexer.Merge(@"D:\temporarly posting file" + "\\TempPostingFileNumber_" + "0", @"D:\temporarly posting file" + "\\TempPostingFileNumber_" + "1", @"D:\temporarly posting file", "posting1");
                   }*/
            }
            foreach(string s in parser.getTemp())
            {
                Console.WriteLine(s);
            }
            Console.ReadLine();
        }
    }
}
