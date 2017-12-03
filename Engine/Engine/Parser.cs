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
        private HashSet<string> stopWords = new HashSet<string>();
        private HashSet<Term> terms = new HashSet<Term>(); // לא בטוח שצריך
        private int documentCurrentPosition = 0;
        public Parser(string path) => ReadStopWords(path);
    HashSet<char> signs=  new HashSet<char>{ '!', '?', ':', ',', '.', '[', ']', '(', ')', '{', '}', '.', '"'};
        /// <summary>
        /// divide a text into terms according to the engine rulls
        /// </summary>
        /// <param name="str"></param> a text to parse
        public void Parse(string str)
        {
           // האם להוריד ', מתי בסטמר
        //   string s = Regex.Replace("[a-1]",  @"[^0-9a-zA-Z. %-$]+", ""); //remove unnecessary chars
            char[] delimeters = { ' ', '\n', '\r','-' };//לזכור לשנות dב-casenumber
            string[] words = str.Split(delimeters);
            Document currentDoc = new Document(words[1]);//because str started after <DOCNO>     
            int startOfText = 0;
            for (int i = 2; i < words.Length; i++)//loop for the documnet's date the to find the begining of the text
            {
                string currentWord = words[i];
                if (currentWord == "" || (stopWords.Contains(currentWord) && Char.IsLower(currentWord[0]))) // if empty line or stopWord
                    continue;
                while (!words[i].Contains("<DATE")) // set the document date
                    i++;
                i = DocDate(currentDoc, words, i);
                if (currentWord == "" || (stopWords.Contains(currentWord) && Char.IsLower(currentWord[0]))) // if empty line or stopWord
                    continue;
                while (!words[i].Contains("<TEXT>"))
                    i++;
                startOfText = ++i; // now its the start of the text
                break;
            }//first for
            for (int i = startOfText; i < words.Length; i++) //loop for the text from <Text> 
            {
                int currentWordLength = words[i].Length;
                if (words[i] == "" || (stopWords.Contains(words[i]) && Char.IsLower(words[i][0]))) // if empty line or stopWord
                    continue;
                if (char.IsNumber(words[i][0])) //if the first char is number
                {
                    //if the number is part of a date 
                    if ((Int32.TryParse(words[i], out int d1) && FindMonth(words[i + 1]) != 0) || words[i].Substring(currentWordLength - 2, 2).Equals("th"))
                    //if (Double.TryParse(words[i].Substring(0, currentWordLength - 1), out double d2) && words[i][currentWordLength - 1].Equals('%'))
                    {
                        //date
                    }//DateCase
                     //if the whole term is a number or ends with %
                    else if ((Double.TryParse(words[i].Substring(0, currentWordLength - 1), out double d2) && words[i][currentWordLength - 1].Equals('%')) || Double.TryParse(words[i], out double d3))
                    {
                        words[i] = Regex.Replace(words[i],  @"[^0-9a-zA-Z. %]+", ""); //remove unnecessary chars
                        i = NumberCase(currentDoc, words, i);
                    }
                }// if first char is number
                else if (FindMonth(words[i]) != 0 && Int32.TryParse(words[i+1], out int d1))// if it is a part of a Date
                {
                    //DATE
                }
                else if (!words[i].ToLower().Equals(words[i]))//check if the word contains at least one capital letter
                {
                    i=CapitalLetterCase(currentDoc,words,i);

                } 
                else  if( stopWords.Contains(words[i])) // if empty line or stopWord
                    continue;
                else 
                    AddTerm(currentDoc, words[i]);
                
            }//second for
            Console.WriteLine("the end");
        }
       

        /// <summary>
        /// updatind the date field for any document object
        /// </summary>
        /// <param name="currentDoc"></param> a document to update its date
        /// <param name="words"></param> string of the document's data
        /// <param name="i"></param> index of the current place in the document
        /// <returns>the current position in the documment</returns>
        private int DocDate(Document currentDoc, string[] words, int i)
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
            return i;
        }

        /// <summary>
        /// convert integers that present day, month and year to string of date according to the rules
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private string DateToString(int day, int month, int year)
        {
            string str = "";
            if (month >= 10)
            {
                if (year == 0)
                {
                    if (day >= 10)
                        str = day + "/" + month;
                    else//day < 10
                        str = "0" + day + "/" + month;
                }
                else
                {
                    if (day >= 10)
                        str = day + "/" + month + "/" + year;
                    else//day < 10
                        str = "0" + day + "/" + month + year;
                }
            }
            else if (month < 10)
            {
                if (year == 0)
                {
                    if (day >= 10)
                        str = day + "/0" + month;
                    else//day < 10
                        str = "0" + day + "/0" + month;
                }
                else
                {
                    if (day >= 10)
                        str = day + "/0" + month + "/" + year;
                    else//day < 10
                        str = "0" + day + "/0" + month + year;
                }
            }
            return str;
        }
        /// <summary>
        /// private method that convert month to its number
        /// </summary>
        /// <param name="currentWord"></param>
        /// <returns></returns>
        private int FindMonth(string currentWord)
        {
            currentWord = currentWord.ToLower();
            if (currentWord.Equals("january") || currentWord.Equals("jan"))
                return 1;
            else if (currentWord.Equals("february") || currentWord.Equals("feb"))
                return 2;
            else if (currentWord.Equals("march") || currentWord.Equals("mar"))
                return 3;
            else if (currentWord.Equals("april") || currentWord.Equals("apr"))
                return 4;
            else if (currentWord.Equals("may"))
                return 5;
            else if (currentWord.Equals("june") || currentWord.Equals("jun"))
                return 6;
            else if (currentWord.Equals("july") || currentWord.Equals("jul"))
                return 7;
            else if (currentWord.Equals("august") || currentWord.Equals("aug"))
                return 8;
            else if (currentWord.Equals("september") || currentWord.Equals("sep"))
                return 9;
            else if (currentWord.Equals("october") || currentWord.Equals("oct"))
                return 10;
            else if (currentWord.Equals("november") || currentWord.Equals("nov"))
                return 11;
            else if (currentWord.Equals("december") || currentWord.Equals("dec"))
                return 12;
            else
                return 0;
        }

        /// <summary>
        /// read the file of the stopwords according to the path and update the hash set
        /// </summary>
        /// <param name="path"></param>
        private void ReadStopWords(string path)
        {
            StreamReader sr = new StreamReader(path);
            string line = "";
            while((line = sr.ReadLine()) != null)
            {
                stopWords.Add(line);
            }
            sr.Close();
        }

        /// <summary>
        /// add terms with capital letters according to the rules
        /// </summary>
        /// <param name="currentDoc"></param>
        /// <param name="currentWord"></param>
        /// <param name="i"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        private int CapitalLetterCase(Document currentDoc, string[] words, int i)
        {
            string currentWord = words[i];
            currentWord = currentWord.ToLower();
            if (i + 1 < words.Length)// to avoid out of bound exception
            {
                if(words[i + 1].ToLower().Equals(words[i + 1])|| signs.Contains(words[i][0])|| signs.Contains(words[i][words[i].Length-1]))//if the next word doesnt contain capital letter
                {
                    AddTerm(currentDoc, currentWord);
                    i = i + 1;
                }
                else //if the next word contains capital letter
                {
                    string temp = currentWord;
                    AddTerm(currentDoc, currentWord);
                    
                    while (!(words[i + 1].ToLower().Equals(words[i + 1])&& signs.Contains(words[i][0]) || signs.Contains(words[i][words[i].Length-1])))//while the next word contains capital letter
                    {
                        currentWord = words[i + 1].ToLower();
                        temp = temp + " " + currentWord;
                        AddTerm(currentDoc, currentWord);
                        i = i + 1;
                        if (i + 1 == words.Length)
                        {
                            break;
                        }
                    }
                    AddTerm(currentDoc, temp);
                }
            }
            else //end of the text
            {
                AddTerm(currentDoc, currentWord);
            }          
            return i;
        }

       

        private void HyphenCase(Document currentDoc, string[] words, int i)
        {
            string currentWord = words[i];
            if (currentWord.Contains("-") && currentWord.Length == 7)//cases like 1992-93
            {
                int indexOfHyphen = currentWord.IndexOf('-');
                if ((currentWord.Substring(0, 2).Equals("19") || currentWord.Substring(0, 2).Equals("20")) && currentWord.Substring(indexOfHyphen).Length == 2)
                {
                    string firstNum = currentWord.Substring(0, indexOfHyphen);
                    string secondNum = currentWord.Substring(0, 2) + currentWord.Substring(indexOfHyphen);
                    if (firstNum.Contains('.'))
                    {
                        double d = Double.Parse(currentWord);
                        d = Math.Round(d, 2);
                        AddTerm(currentDoc, "" + d);
                    }
                    else
                        AddTerm(currentDoc, firstNum);
                    if (secondNum.Contains('.'))
                    {
                        double d = Double.Parse(currentWord);
                        d = Math.Round(d, 2);
                        AddTerm(currentDoc, "" + d);
                    }
                    else
                        AddTerm(currentDoc, secondNum);
                }
            }
        }
        /// <summary>
        /// responsible for the numbers cases, include percets, and fix them according to the rules
        /// </summary>
        /// <param name="currentDoc"></param>
        /// <param name="currentWord"></param>
        /// <param name="i"></param>
        private int NumberCase(Document currentDoc, string[] words, int i)
        {
            int index = 0;
            string currentWord = words[i];
            //try
            // currentWord = Regex.Replace(currentWord, @"[,]", "");//remove unnecessary chars
            if (!currentWord.Contains(".") && !(currentWord.Contains("%") || words[i + 1].Contains("percentage") || (words[i + 1].Contains("percent"))))//regular number (without percent or .)
                AddTerm(currentDoc, currentWord);
            else if (currentWord.Contains("."))//check if the number is neede to be round
            {
                if (!(currentWord.Contains("%") || words[i + 1].Contains("percentage") || (words[i + 1].Contains("percent"))))// cases like 10.567, without percent
                {
                    double d = Double.Parse(currentWord);
                    d = Math.Round(d, 2);
                    AddTerm(currentDoc, "" + d);
                }
                else //is percent
                {
                    if (currentWord.Contains("%"))// cases like 10.675%
                    {
                        index = currentWord.IndexOf("%");
                        currentWord = currentWord.Substring(0, index);
                    }
                    else // cases 10.567 precent and 10.567 precentege
                    {
                        i = i + 2;
                    }
                    double d = Double.Parse(currentWord);
                    d = Math.Round(d, 2);
                    AddTerm(currentDoc, d + " percent");
                }
            }// if cases need to be round
            else // percent without .
            {
                if (currentWord.Contains("%"))
                {
                    index = currentWord.IndexOf("%");
                    currentWord = currentWord.Substring(0, index);
                }
                else if (words[i + 1].Equals("percentage") || words[i + 1].Equals("percent"))
                {
                    i = i + 2;
                }
                currentWord = currentWord + " percent";
                AddTerm(currentDoc, currentWord);
            }
            return i;
        }

        /// <summary>
        /// add new term to the collection and updating the term and document's details accordingly
        /// </summary>
        /// <param name="currentDoc"></param>
        /// <param name="termName"></param>
        /// <param name="position"></param> the position of the term in the current document
        private void AddTerm(Document currentDoc, string termName)
        {
            if(signs.Contains(termName[termName.Length - 1])|| signs.Contains(termName[0]))
                termName = Regex.Replace(termName, @"[^0-9a-zA-Z ]+", ""); //remove unnecessary chars
            Term t = new Term(termName);
            t.updateDetails(currentDoc, this.documentCurrentPosition);
            currentDoc.addTerm(t);
            this.documentCurrentPosition++;
            terms.Add(t);
            //send to indexer
        }

        /*private int DateCase(Document currentDoc, string[] words, int i)
        {
            string currentWord = words[i];
        }*/

    }  
}
