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
        Parser parser;
        public ReadFile(string path)
        {
            this.path = path;
            parser = new Parser(path + "\\stop_words.txt");
        }

        public void Seperate()
        {
           string[] filesPaths = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);

            for (int i = 0; i < filesPaths.Length; i++)
            {
                string fileText = File.ReadAllText(filesPaths[i]);
                Match matchTEXT = Regex.Match(fileText, Regex.Escape("<DOCNO>") + "(.*?)" + Regex.Escape("</TEXT>"), RegexOptions.Singleline, Regex.InfiniteMatchTimeout);
                while (matchTEXT.Success)
                {
                    parser.Parse(matchTEXT.Groups[1].Value);
                    matchTEXT = matchTEXT.NextMatch();
                }
            }
        }
    }
}
