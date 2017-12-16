using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace Engine
{
    class ReadFile
    {
        string path;
        //Parser parser;
        private  Regex CompiledRegex;
        private string[] filesPaths;
        //int index;
     // private static readonly  Regex CompiledRegex = new Regex(Regex.Escape("<DOCNO>") + "(.*?)" + Regex.Escape("</TEXT>"), RegexOptions.Singleline);
        public ReadFile(string path)
        {
            this.path = path;
            //parser = new Parser(path + "\\stop_words.txt");
            CompiledRegex = new Regex(Regex.Escape("<DOCNO>") + "(.*?)" + Regex.Escape("</TEXT>"),RegexOptions.Singleline );
            filesPaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
               
            
        }
        
        /// <summary>
        /// sperate directory into files
        /// </summary>
        /// <returns></returns>
        public Match Seperate(int index)
        {
            string fileText = File.ReadAllText(filesPaths[index]);
            return CompiledRegex.Match(fileText);
        }
  
        /// <summary>
        /// return the amount of files in specific path 
        /// </summary>
        /// <returns></returns>
        public int FilesAmount()
        {
            return filesPaths.Length;
        }

        /// <summary>
        /// read the file of the stopwords according to the path and update the hash set
        /// </summary>
        /// <param name="path"></param>
        public HashSet<string> ReadStopWords(string path)
        {
          HashSet<string> stopWords = new HashSet<string>();
        StreamReader sr = new StreamReader(path);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                stopWords.Add(line);
            }
            sr.Close();
            return stopWords;
        }
    }
}
