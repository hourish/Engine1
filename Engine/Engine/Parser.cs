using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Engine
{
    class Parser
    {
        HashSet<string> stopWords = new HashSet<string>();
        HashSet<Term> terms = new HashSet<Term>();
        public Parser(string path)
        {
           ReadStopWords(path);
        }
        public void Parse(string str)
        {
            char[] delimeters = { ' ', '\n', '\r' };
            string[] words = str.Split(delimeters);
            Document currentDoc = new Document(words[1]);//because str started after <DOCNO>
            for (int i = 2; i < words.Length; i++)
            {
                string currentWord = words[i];
                if (currentWord == "" || (stopWords.Contains(currentWord) && Char.IsLower(currentWord[0]))) // if empty line or stopWord
                    continue;
                if (currentWord.Contains("<DATE"))
                {
                    i = DateCase(currentDoc, words, i);                  
                }
            }
        }

        private int DateCase(Document currentDoc, string[] words, int i)
        {
            string currentWord = words[i];
            int day = 0, month = 0, year = 0;
            while (!currentWord.Contains("</DATE"))
            {
                if (currentWord != "")
                {
                    if (currentWord.IndexOf('>') != currentWord.Length - 1 && currentWord.Length == 12) // for cases like <DATE>920314
                    {
                        year = 1900 + (int)Char.GetNumericValue(currentWord[6]) * 10 + (int)Char.GetNumericValue(currentWord[7]);
                        month = (int)Char.GetNumericValue(currentWord[8]) * 10 + (int)Char.GetNumericValue(currentWord[9]);
                        day = (int)Char.GetNumericValue(currentWord[10]) * 10 + (int)Char.GetNumericValue(currentWord[11]);
                    }
                    else
                    {
                        if (int.TryParse(currentWord, out int x))
                        {
                            if (x >= 1 && x <= 31)
                                day = x;
                            else if (x > 1000)
                                year = x;
                        }
                        else if (month == 0)
                            month = FindMonth(currentWord);
                    }
                }
                currentWord = words[++i];
            }
            currentDoc.SetDate(new DateTime(year, month, day));
            Term t;
            if(month >= 10 && day >=10)
                t = new Term(day + "/" + month + "/" + year);
            else if(month < 10 && day >= 10)
                t = new Term(day + "/0" + month + "/" + year);
            else if(month < 10 && day <10)
                t = new Term(day + "/0" + month + "/0" + year);
            else if (month >=10 && day < 10)
                t = new Term(day + "/" + month + "/0" + year);
            return i;
        }

        private int FindMonth(string currentWord)
        {
            if (currentWord.Equals("January") || currentWord.Equals("Jan"))
                return 1;
            else if (currentWord.Equals("February") || currentWord.Equals("Feb"))
                return 2;
            else if (currentWord.Equals("March") || currentWord.Equals("Mar"))
                return 3;
            else if (currentWord.Equals("April") || currentWord.Equals("Apr"))
                return 4;
            else if (currentWord.Equals("May"))
                return 5;
            else if (currentWord.Equals("June") || currentWord.Equals("Jun"))
                return 6;
            else if (currentWord.Equals("July") || currentWord.Equals("Jul"))
                return 7;
            else if (currentWord.Equals("August") || currentWord.Equals("Aug"))
                return 8;
            else if (currentWord.Equals("September") || currentWord.Equals("Sep"))
                return 9;
            else if (currentWord.Equals("October") || currentWord.Equals("Oct"))
                return 10;
            else if (currentWord.Equals("November") || currentWord.Equals("Nov"))
                return 11;
            else if (currentWord.Equals("December") || currentWord.Equals("Dec"))
                return 12;
            else
                return 0;
        }


        private void ReadStopWords(string path)
        {
            StreamReader sr = new StreamReader(path);
            string line = "";
            while((line = sr.ReadLine()) != null)
            {
                stopWords.Add(line);
            }
        }

        private string NumberCase(string term)
        {
            int index;
            double d = 0;
            string temp = term.Substring(0, 1);//the first char of the string
            if (double.TryParse(temp, out d) && !term.Contains("th")) //check if the first char is a number
            {
                if (term.Contains("."))//check if the number is neede to be round
                {
                    if (!(term.Contains("%") || term.Contains("percentage") || (term.Contains("percent"))))// cases like 10.567
                    {
                        d = Double.Parse(term);
                        d = Math.Round(d, 2);
                        return "" + d;
                    }
                    if (term.Contains("%"))// cases like 10.675%
                    {
                        index = term.IndexOf("%");
                        term = term.Substring(0, index);
                        d = Double.Parse(term);
                        d = Math.Round(d, 2);
                        return d + "percent";
                    }
                    // cases 10.567precent and 10.567precentege
                    index = term.IndexOf("p");
                    term = term.Substring(0, index);
                    d = Double.Parse(term);
                    d = Math.Round(d, 2);
                    return d + "percent";


                }// if cases need to be round

                if (term.Contains("%"))
                {
                    index = term.IndexOf("%");
                    term = term.Substring(0, index);
                    term = term + "percent";
                    return term;
                }

                if (term.Contains("percentage"))
                {
                    index = term.IndexOf("p");
                    term = term.Substring(0, index);
                    term = term + "percent";
                    return term;

                }
            }// if the term contain number

            return term;
        }
    }  
}
