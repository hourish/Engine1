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
        public ReadFile(string path)
        {
            this.path = path;
        }

        public void Seperate()
        {
            List<string> docTEXT = new List<string>();
            List<string> docNum = new List<string>();

            string[] filesPaths = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            string[] fileText = new string[filesPaths.Length];
            for (int i = 0; i < filesPaths.Length; i++)
            {
                fileText[i] = File.ReadAllText(filesPaths[i]);
                Match matchTEXT = Regex.Match(fileText[i], Regex.Escape("<TEXT>") + "(.*?)" + Regex.Escape("</TEXT>"), RegexOptions.Singleline, Regex.InfiniteMatchTimeout);
                while (matchTEXT.Success)
                {
                    docTEXT.Add(matchTEXT.Groups[1].Value);
                    matchTEXT = matchTEXT.NextMatch();
                }

                Match matchNo = Regex.Match(fileText[i], Regex.Escape("<DOCNO>") + "(.*?)" + Regex.Escape("</DOCNO>"), RegexOptions.Singleline, Regex.InfiniteMatchTimeout);
                while(matchNo.Success)
                {
                    docNum.Add(matchNo.Groups[1].Value);
                    matchNo = matchNo.NextMatch();
                }
            }

        }
    }
}
