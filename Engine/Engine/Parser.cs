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
        //HashSet<Term> terms = new HashSet<Term>();
        int documentLengthCounter = 0;
        public Parser(string path)
        {
           ReadStopWords(path);
        }
        /// <summary>
        /// divide a text into terms according to the engine rulls
        /// </summary>
        /// <param name="str"></param> a text to parse
        public void Parse(string str)
        {
            str = "To Be Or Not To Be";
            documentLengthCounter = 0;
            char[] delimeters = { ' ', '\n', '\r' };
            string[] words = str.Split(delimeters);
            int test= capitalLetterCase(new Document("d1"),words[0],0, words);
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
        /// <summary>
        /// updatind the date field for any document object
        /// </summary>
        /// <param name="currentDoc"></param> a document to update its date
        /// <param name="words"></param> string of the document's data
        /// <param name="i"></param> index of the current place in the document
        /// <returns>the current position in the documment</returns>
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

        /// param- path to the stop words list's folder 
       /// read the stop words list from the given path and save them in Hash set
        private void ReadStopWords(string path)
        {
            StreamReader sr = new StreamReader(path);
            string line = "";
            while((line = sr.ReadLine()) != null)
            {
                stopWords.Add(line);
            }
        }
        // fix terms including numbers according to the rulls
        private void NumberCase(Document currentDoc, string currentWord, int i)
        {
            int index = 0;
            string temp = currentWord.Substring(0, 1);//the first char of the string
            if (double.TryParse(temp, out double d) && !currentWord.Contains("th")) //check if the first char is a number
            {
                if (currentWord.Contains("."))//check if the number is neede to be round
                {
                    if (!(currentWord.Contains("%") || currentWord.Contains("percentage") || (currentWord.Contains("percent"))))// cases like 10.567
                    {
                        d = Double.Parse(currentWord);
                        d = Math.Round(d, 2);
                        addTerm(currentDoc, "" + d, i);
                    }
                    else
                    {
                        if (currentWord.Contains("%"))// cases like 10.675%
                        {
                            index = currentWord.IndexOf("%");
                        }
                        else // cases 10.567precent and 10.567precentege
                        {
                            index = currentWord.IndexOf("p");
                        }
                        currentWord = currentWord.Substring(0, index);
                        d = Double.Parse(currentWord);
                        d = Math.Round(d, 2);
                        addTerm(currentDoc, d + "percent", i);
                    }
                }// if cases need to be round
                else
                {
                    if (currentWord.Contains("%"))
                    {
                        index = currentWord.IndexOf("%");
                    }
                    else if (currentWord.Contains("percentage"))
                    {
                        index = currentWord.IndexOf("p");
                    }
                    currentWord = currentWord.Substring(0, index);
                    currentWord = currentWord + "percent";
                    addTerm(currentDoc, currentWord, i);
                }
            }// if the term contain number
        }

        /// <summary>
        /// add new term to the collection and updating the term and document's details accordingly
        /// </summary>
        /// <param name="currentDoc"></param>
        /// <param name="termName"></param>
        /// <param name="position"></param> the position of the term in the current document
        private void addTerm(Document currentDoc, string termName, int position)
        {
            Term t = new Term(termName);
            t.updateDetails(currentDoc, position);
            currentDoc.addTerm(t);
            //send to indexer
        }

        private int capitalLetterCase(Document currentDoc, string currentWord, int i , string[]words)
        {
            if (!currentWord.ToLower().Equals(currentWord))
            {
                currentWord = currentWord.ToLower();
                if (i + 1 < words.Length)
                {
                    if (words[i + 1].ToLower().Equals(words[i + 1]))
                    {
                        addTerm(currentDoc, currentWord, i);
                        i = i + 1;
                    }
                    else
                    {
                        string temp = "";
                        int position = i;
                        while (!words[i + 1].ToLower().Equals(words[i + 1]))
                        {

                            temp = temp +" "+ currentWord;
                            addTerm(currentDoc, currentWord, i);
                            currentWord = words[i + 1];
                            currentWord = currentWord.ToLower();
                            i = i + 1;
                            if(i+1==words.Length)
                            {
                                break;
                            }
                        }

                        addTerm(currentDoc, temp, position);
                    }
                }
                else
                {
                    addTerm(currentDoc, currentWord, i);
                    i = i + 1;
                }
            }
            return i;
        }

    }// parses class 
}// namespace
