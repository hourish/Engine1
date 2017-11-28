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
            Regex rDOCNO = new Regex(Regex.Escape("<DOCNO>") + "(.*?)" + Regex.Escape("</DOCNO>"));
            Regex rTEXT = new Regex(Regex.Escape("<TEXT>") + "(.*?)" + Regex.Escape("</TEXT>"));
            for (int i = 0; i < filesPaths.Length; i++)
            {
                fileText[i] = File.ReadAllText(filesPaths[i]);
                MatchCollection matchesDOCNO = rDOCNO.Matches(fileText[i]);
                MatchCollection matchesTEXT = rTEXT.Matches(fileText[i]);
               // foreach (Match match in matchesDOCNO)
               //     docNum.Add(match.Groups[1].Value);
                foreach (Match match in matchesTEXT)
                    docTEXT.Add(match.Groups[1].Value);

                Match matchNo = rDOCNO.Match(fileText[i]);
                while(matchNo.Success)
                {
                    docNum.Add(matchNo.Groups[1].Value);
                    matchNo = matchNo.NextMatch();
                }
            }
            //    string del = @"\<DOCNO\>(.*?)\<\/TEXT\>";
            //string del = @"\b\<DOCNO\>+\s\<\/TEXT\>+\b"
            //string del = Regex.Escape("<DOCNO>") + "(.*?)" + Regex.Escape("</TEXT>");
          /*  string docNo = "<DOCNO>(.*?)</DOCNO>";
            string doc = "<DOC>(.*?)</DOC>";
            docNum.AddRange(Regex.Split(fileText[0], docNo));
            docs.AddRange(Regex.Split(fileText[0], doc));*/
        }
    }
}
