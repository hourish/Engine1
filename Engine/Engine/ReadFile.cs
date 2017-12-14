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
        private Regex CompiledRegex;
        private string[] filesPaths;

       
        public ReadFile(string path)
        {
            this.path = path;
            CompiledRegex = new Regex(Regex.Escape("<DOCNO>") + "(.*?)" + Regex.Escape("</TEXT>"), RegexOptions.Singleline);
            filesPaths = System.IO.Directory.GetFiles(path, ".", System.IO.SearchOption.AllDirectories);///להתייעץ עם שני אם זה צריך להיות פה או בפונקציה
        }
        /// <summary>
        /// sperate directory into files
        /// </summary>
        /// <returns></returns>
        public string ReadText(int index)
        {
            return File.ReadAllText(filesPaths[index]);
        }

        public Match Seperate(string str)
        {
            return CompiledRegex.Match(str);
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
