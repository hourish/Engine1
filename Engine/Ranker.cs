using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
     class Ranker 
    {
        private Dictionary<string, string[]> DocDictionary = new Dictionary<string, string[]>();
        private Dictionary<string, string[]> words= new Dictionary<string, string[]>(); //dictionery of trerm- total tf, df, posting, position, line
        //private Dictionary<string, string[]> words = new Dictionary<string, string[]>();
        Dictionary<string, string> cache = new Dictionary<string, string>();
        public Ranker(Dictionary<string, string[]> DocDic, Dictionary<string, string> ocache)
        {
            DocDictionary = DocDic;
            cache = ocache;
        }
        /// <summary>
        /// calculate the weights of every documents the words apear at and return the 50 with the highest weight
        /// </summary>
        /// <param name="stm"></param>// if the query is with stem or not
        /// <param name="words"></param> // 
        /// <returns></returns>
        public Dictionary<string, double> Rank(bool stm, Dictionary<string, string[]> queryWords, string path)
        {
            words.Clear();
            List<string> terms = words.Keys.ToList();
            for (int i = 0; i < words.Count; i++) // ~~creat dictionery of trerm- total tf, df, posting, position,line~~
            {
                string[] temp = new string[5];// detail of one term: 0- total tf, 1- df,2-posting name,3-position, 4-posting line
                if (cache.ContainsKey(terms[i]))
                {
                    temp[4] = cache[terms[i]];
                }
                else
                {
                    FileStream fs = new FileStream(path + queryWords[terms[i]][2], FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    br.BaseStream.Seek(Int64.Parse(queryWords[terms[i]][3]), SeekOrigin.Begin);
                    temp[4] = ReadLine(br);
                }
                // the line of the term from the dictionery
                temp[0] = queryWords[terms[i]][0]; // total tf
                temp[1] = queryWords[terms[i]][1]; // df
                temp[2] = queryWords[terms[i]][2]; // posting name
                temp[3] = queryWords[terms[i]][3];// position 
                
                words.Add(terms[i], temp);
            }
            HashSet<string> doneDocuments = new HashSet<string>(); // hash of the document we alredy calculate cossin similarity for them.
            StringBuilder termDoc = new StringBuilder();
            for ( int i=0; i<words.Count; i++) //~~going through all the words in the query~~ 
            {
                int index = terms[i].Length;
                string currentLine = words[terms[i]][4];
                while(index< currentLine.Length)
                {
                  //  if()
                }
            }
            
           // for ( int i=0; i<)

            return new Dictionary<string, double>();
                 
        }
        /// <summary>
        /// calculate cosinsimilarity between spacific doc and the query
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="detail"></param>- array of the terms from the query in the document
        /// <returns></returns>
        public double CosSin(string doc, string[] detail)
        {
            double res = 0;
             
            return res;
        }
        
        private string ReadLine(BinaryReader reader)
        {
            var result = new StringBuilder();
            bool foundEndOfLine = false;
            char ch;
            while (!foundEndOfLine)
            {
                try
                {
                    ch = reader.ReadChar();
                }
                catch (EndOfStreamException ex)
                {
                    if (result.Length == 0) return null;
                    else break;
                }

                switch (ch)
                {
                    case '\r':
                        if (reader.PeekChar() == '\n') reader.ReadChar();
                        foundEndOfLine = true;
                        break;
                    case '\n':
                        foundEndOfLine = true;
                        break;
                    default:
                        result.Append(ch);
                        break;
                }
            }
            return result.ToString();
        }
    }
}
