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
        Indexer indexer = new Indexer();
        string path;
        Parser parser;
        int sizeCounter;
        string[] filesPaths;
        int currentIndex;
        public ReadFile(string path)
        {
            this.path = path;
            parser = new Parser(path + "\\stop_words.txt");
            filesPaths = System.IO.Directory.GetFiles(path, ".", System.IO.SearchOption.AllDirectories);
            currentIndex = 0;
            sizeCounter = 0;
        }

        public void Seperate()
        {

            for (int i = 0; i < filesPaths.Length; i++)
            {
                string fileText = File.ReadAllText(filesPaths[i]);
                //if pass corpus size
                Match matchTEXT = Regex.Match(fileText, Regex.Escape("<DOCNO>") + "(.*?)" + Regex.Escape("</TEXT>"), RegexOptions.Singleline, Regex.InfiniteMatchTimeout);
                while (matchTEXT.Success)
                {
                    parser.Parse(matchTEXT.Groups[1].Value);
                    matchTEXT = matchTEXT.NextMatch();
                }
                //else
                indexer.CreateTempPostingFile(path);
                sizeCounter = 0;
            }
        }
    }
}
