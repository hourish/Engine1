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
<<<<<<< HEAD
        private Dictionary<string, string[]> DocDictionary = new Dictionary<string, string[]>();
        private Dictionary<string, string[]> words= new Dictionary<string, string[]>(); //dictionery of trerm- total tf, df, posting, position, line
        //private Dictionary<string, string[]> words = new Dictionary<string, string[]>();
=======
        private Dictionary<string, string[]> DocDictionary = new Dictionary<string, string[]>();// dictionery of docName- maxTf, length, date
        private Dictionary<string, string[]> words= new Dictionary<string, string[]>(); //dictionery of trerm- total tf, df, posting, position, line
       // private Dictionary<string, string[]> words = new Dictionary<string, string[]>();
>>>>>>> e8b9bfa310939257e40d899fc5f7cc086bcf2e35
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
<<<<<<< HEAD
            for (int i = 0; i < words.Count; i++) // ~~creat dictionery of trerm- total tf, df, posting, position,line~~
=======
            Dictionary<string, double> rank = new Dictionary<string, double>();
            for (int i = 0; i < words.Count; i++) // creat dictionery of trerm- total tf, df, posting, position,line
>>>>>>> e8b9bfa310939257e40d899fc5f7cc086bcf2e35
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
<<<<<<< HEAD
                
=======
                temp[4] = ReadLine(br); // the line of the term from the dictionery
>>>>>>> e8b9bfa310939257e40d899fc5f7cc086bcf2e35
                words.Add(terms[i], temp);
            }
            HashSet<string> doneDocuments = new HashSet<string>(); // hash of the document we alredy calculate cossin similarity for them.
            StringBuilder termDoc = new StringBuilder();
<<<<<<< HEAD
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
=======
            List<string> docQueryTerms = new List<string>();// list of all the terms from the query that apear in spacific document
            for ( int i=0; i<terms.Count;i++)
            {
                docQueryTerms.Clear();
                termDoc.Clear();
                int index = terms[i].Length+1;// אמור להתחיל מאפס
                string currentTermLine = words[terms[i]][4];
                int count= 0; // count how many times we saw '_'
                while(index<currentTermLine.length) // moving through the term line to get all the documents it apear in
                {
                    
                    while(!currentTermLine[index].Equals(':')) // get some document name
                    {
                        termDoc.Append(currentTermLine[index]);
                        index++;
                    }
                    string currentDoc = termDoc.ToString();
                    if (!doneDocuments.Contains(currentDoc))// check if we alredy calculate cossin similarity to this document
                    {
                        docQueryTerms.Add(terms[i]);
                        for (int j = i + 1; j < terms.Count; j++)// check wich terms of the query also apear in termDoc
                        {
                            if (words[terms[j]][4].Contains(currentDoc) )
                                docQueryTerms.Add(terms[j]);
                        }
                       double currentRank = CosSin(currentDoc, docQueryTerms,path);
                        rank.Add(currentDoc, currentRank);
                        docQueryTerms.Clear();
                    }
                    index = currentTermLine.IndexOf('_', index);
                    index = currentTermLine.IndexOf('_', index + 1);
                    index++;
                }

                
            }
>>>>>>> e8b9bfa310939257e40d899fc5f7cc086bcf2e35

            return rank;
                 
        }
        /// <summary>
        /// calculate cosinsimilarity between spacific doc and the query
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="detail"></param>- array of the terms from the query in the document
        /// <returns></returns>
        public double CosSin(string doc, List<string> terms,string path,bool stm )
        {
            double mone = 0;
            StringBuilder tf = new StringBuilder();
            for (int i = 0; i < terms.Count; i++)
            {
                string currentLine = words[tems[i]][4];
                int index = currentLine.IndexOf(doc);
                index = currentLine.IndexOf('_', index + doc.Length);
                index++;
                tf.Clear();
                while (!currentLine[index].Equals('_'))
                {
                    tf.Append(currentLine[index]);
                    index++;

                }
                int dotIndex = line.IndexOf(':');
                double df = 0;
                while (dotIndex != -1)// count in how many fiels the term apear
                {
                    df++;
                    if (dotIndex + 1 < line.Length)
                        dotIndex = line.IndexOf(':', dotIndex + 1);
                    else
                        break;
                }
                mone += (Int32.Parse(tf.ToString()) / Int32.Parse(DocDictionary[doc][1])) * Math.Log(DocDictionary.Count / df, 2);// (tf/ documentLength)*idf
            }
                string weightDocPath; 
                if(stm)
                {
                    weightDocPath= path+ "//WeightDocsSTM";
                }
                else
                {
                    weightDocPath = path + "//WeightDocs";
                }
                 FileStream fs = new FileStream(path+ queryWords[terms[i]][2], FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                string line= br.BaseStream.Seek(Int64.Parse(DocDictionary[doc][3]), SeekOrigin.Begin);
                string[] temp = line.Split(":");
                double machane = Math.Sqrt(Double.Parse(temp[1]) * words.Count);
            
             
            return mone/machane;
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
