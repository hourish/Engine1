using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace Engine
{
    class Parser
    {
        Indexer indexer = new Indexer();
        private HashSet<string> stopWords = new HashSet<string>();
        Document currentDoc;
        Dictionary<string, string> months = new Dictionary<string, string>() { { "January", "01" }, { "Jan", "01" }, { "February", "02" }, { "Feb", "02" }, { "March", "03" }, { "Mar", "03" }, { "April", "04" }, { "Apr", "04" }, { "May", "05" }, { "June", "06" }, { "Jun", "06" }, { "July", "07" }, { "Jul", "07" }, { "August", "08" }, { "Aug", "08" }, { "September", "09" }, { "Sep", "09" }, { "October", "10" }, { "Oct", "10" }, { "November", "11" }, { "Nov", "11" }, { "December", "12" }, { "Dec", "12" } };
        HashSet<char> signs = new HashSet<char> { '!', '?', ':', ',', '.', '[', ']', '(', ')', '{', '}', '.', '"', '\\', '/' , '*'};
        private static Dictionary<string, Term> terms = new Dictionary<string, Term>();
        private int documentCurrentPosition = 0;
        public Parser(string path) => ReadStopWords(path);
        /// <summary>
        /// divide a text into terms according to the engine rulls
        /// </summary>
        /// <param name="str"></param> a text to parse
        public void Parse(string str)
        {
            // האם להוריד ', מתי בסטמר
            //   string s = Regex.Replace("[a-1]",  @"[^0-9a-zA-Z. %-$]+", ""); //remove unnecessary chars
            int startOfDONCON = 0;
            while (str[startOfDONCON].Equals(' '))
                startOfDONCON++;
            string DOCNO = "";
            while (Char.IsLetterOrDigit(str[startOfDONCON]) || str[startOfDONCON].Equals('-'))
            {
                DOCNO += str[startOfDONCON];
                startOfDONCON++;
            }
            currentDoc = new Document(DOCNO);//because str started after <DOCNO>
            char[] delimeters = { ' ', '\n', '\r', '-' };
            string[] words = str.Split(delimeters);
            int startOfText = 0;
            for (int i = 4; i < words.Length; i++)//loop for the documnet's date the to find the begining of the text. start after the DOCNO
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
                {
                    i++;
                    if (i == words.Length)
                        break;
                }
                startOfText = ++i; // now its the start of the text
                break;
            }//first for
            for (int i = startOfText; i < words.Length; i++) //loop for the text from <Text> 
            {
                string currentWord = words[i];//for debug
                int currentWordLength = words[i].Length;
                if (words[i] == "" || (stopWords.Contains(words[i]) && Char.IsLower(words[i][0])) || (AllSign(words[i]))) // if empty line or stopWord
                    continue;
                if (currentWordLength > 1)
                {
                    if (words[i][currentWordLength - 2].Equals('\'') && words[i][currentWordLength - 1].Equals('s')) //if word ends with 's suffix
                    {
                        words[i] = words[i].Substring(0, words[i].IndexOf("'s"));
                        if (words[i].Equals(""))//cases like only 's
                            continue;
                        currentWordLength = words[i].Length;

                    }
                }
                if (char.IsNumber(words[i][0])) //if the first char is number
                {
                    //if the number is part of a date: int and than month or int with th suffix th
                    if ((Int32.TryParse(words[i], out int d1)  || words[i].Contains("th")) && months.ContainsKey(words[i + 1])) 
                    {
                        i = DateCaseBeginWithNumber(currentDoc, words, i);
                    }//DateCaseBeginWithNumber
                     //if the whole term is a number or ends with %
                    else if ((Double.TryParse(words[i].Substring(0, currentWordLength - 1), out double d2) && words[i][currentWordLength - 1].Equals('%')) || Double.TryParse(words[i], out double d3))
                    {
                        words[i] = Regex.Replace(words[i], @"[^0-9a-zA-Z. %]+", ""); //remove unnecessary chars
                        i = NumberCase(currentDoc, words, i);
                    }
                }// if first char is number
                else if (months.ContainsKey(words[i]))// if date start with month
                {
                    string month = words[i];
                    while (i + 1 < words.Length)
                    {
                        if (words[i + 1].Equals(""))
                            i++;
                        else break;
                    }
                    if (i + 1 < words.Length)
                    {
                        int length = words[i + 1].Length;
                        if (signs.Contains(words[i + 1][length - 1]))
                        {
                            words[i + 1] = words[i + 1].Substring(0, length - 1);
                            length = words[i + 1].Length;
                        }
                        while (i + 1 < words.Length)
                        {
                            if (words[i + 1].Equals(""))
                                i++;
                            else break;
                        }
                        if (Int32.TryParse(words[i + 1], out int d1)) //check if the next word after the month is a number
                            i = DateCaseBeginWithMonth(currentDoc, words, i, month);
                        //check if the next word is number with th
                        else if (length > 1)// 
                        {
                            if ((words[i + 1][length - 1].Equals('h')) && (words[i + 1][length - 2].Equals('t')) && (Int32.TryParse(words[i + 1].Substring(0, length - 2), out int d2)))
                            {
                                i = DateCaseBeginWithMonth(currentDoc, words, i, month);
                            }
                        }
                    }
                }
                else if (!words[i].ToLower().Equals(words[i]))//check if the word contains at least one capital letter
                {
                    i = CapitalLetterCase(currentDoc, words, i);

                }
                else if (stopWords.Contains(words[i])) // if empty line or stopWord
                    continue;
                else
                    AddTerm(currentDoc, words[i]);
            }//second for
            currentDoc.SetMaxTF();
            indexer.CreateTempPostingFile(terms.Values.ToArray(),currentDoc);
            terms.Clear();
        }

        /// <summary>
        /// check if term composed of  just signs for example the term: /"
        /// </summary>
        /// <param name="termName"></param>
        /// <returns>true- if the term made of signs
        /// false- if the term is not just signs</returns>
        private bool AllSign(string termName)
        {
            for (int j = 0; j < termName.Length; j++)
            {
                if (!signs.Contains(termName[j]))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// responsible for the case when the words[i] is part of a date: int and than month or int with th suffix
        /// </summary>
        /// <param name="currentDoc"></param>
        /// <param name="words"></param>
        /// <param name="i"></param> index
        /// <returns></returns>
        private int DateCaseBeginWithNumber(Document currentDoc, string[] words, int i)
        {
            string currentWord = words[i];
            int day = 0, year = 0;
            string month = months[words[i + 1]];
            currentWord = Regex.Replace(currentWord, @"[^0-9a-zA-Z ]+", ""); //remove unnecessary chars
            if (currentWord.Contains("th"))//cases like 12th MAY 1991
            {
                int.TryParse(currentWord.Substring(0, currentWord.IndexOf('t')), out day);
                if (i + 2 < words.Length)
                {
                    int.TryParse(words[i + 2], out year);
                    i = i + 2;
                }
                else
                    year = 0;
            }
            else// cases like 12 MAY 1991 / 12 MAY 91 / 14 MAY
            {
                int.TryParse(currentWord, out day);
                if (words[i + 2].Length == 4)
                {
                    int.TryParse(words[i + 2], out year);
                    i = i + 2;
                }
                else if (words[i + 2].Length == 2)
                {
                    int.TryParse(words[i + 2], out int tmp);
                    year = 1900 + tmp;
                    i = i + 2;
                }
                else
                {
                    i = i + 1;
                }
            }
            AddTerm(currentDoc, DateToString(day, month, year));
            return i;
        }
        /// <summary>
        /// responsible for the case when the words[i] is month at the beginning of a date
        /// </summary>
        /// <param name="currentDoc"></param>
        /// <param name="words"></param>
        /// <param name="i"></param> index
        /// <returns></returns>
        private int DateCaseBeginWithMonth(Document currentDoc, string[] words, int i, string month)
        {
            while (words[i].Equals(""))
                i++;
            int day = 0, year = 0;

            while (i + 1 < words.Length)
            {
                if (words[i + 1].Equals(""))
                    i++;
                else break;
            }
            if (i + 1 < words.Length)
            {
                if (words[i + 1][words[i + 1].Length - 1].Equals(',') && i + 2 < words.Length)
                {
                    int.TryParse(words[i + 1].Substring(0, words[i + 1].IndexOf(',')), out day);//cases like MAY 12, 1990
                    int.TryParse(words[i + 2], out year);
                    i = i + 2;
                }
                else if (words[i + 1].Length == 2 || words[i + 1].Length == 1) // cases like  MAY 4
                {
                    int.TryParse(words[i + 1], out day);
                    i = i + 1;
                }
                else if (words[i + 1].Length == 4) // cases like  MAY 1993
                {
                    int.TryParse(words[i + 1], out year);
                    i = i + 1;
                }
            }
            AddTerm(currentDoc, DateToString(day, month, year));
            return i;
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
            int day = 0, year = 0;
            string month = "";
            while (!currentWord.Contains("</DATE"))
            {
                if (currentWord != "")
                {
                    currentWord = Regex.Replace(currentWord, @"[^0-9a-zA-Z ]+", ""); //remove unnecessary chars
                    if (currentWord.Contains("DATE") && currentWord.Length == 10) // for cases like <DATE>920314
                    {
                        year = 1900 + (int)Char.GetNumericValue(currentWord[4]) * 10 + (int)Char.GetNumericValue(currentWord[5]);
                        month += (int)Char.GetNumericValue(currentWord[6]) * 10 + (int)Char.GetNumericValue(currentWord[7]);
                        day = (int)Char.GetNumericValue(currentWord[8]) * 10 + (int)Char.GetNumericValue(currentWord[9]);
                        break;
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
                        else if (months.ContainsKey(month))
                            month = months[currentWord];
                        if (day >= 1 && day <= 31 && year > 100 && months.ContainsKey(month))
                            break;
                    }
                }
                currentWord = words[++i];
            }
            currentDoc.SetDate(DateToString(day, month, year));
            return i;
        }

        /// <summary>
        /// convert integers that present day, month and year to string of date according to the rules
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private string DateToString(int day, string month, int year)
        {
            string str = "";
            if (day == 0) //no day
            {
              str = month + "/" + year;
              return str;
            }
            if (year == 0) // no year
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
            return str;
        }
        /// <summary>
        /// private method that convert month to its number
        /// </summary>
        /// <param name="currentWord"></param>
        /// <returns></returns>

        /// <summary>
        /// read the file of the stopwords according to the path and update the hash set
        /// </summary>
        /// <param name="path"></param>
        private void ReadStopWords(string path)
        {
            StreamReader sr = new StreamReader(path);
            string line = "";
            while ((line = sr.ReadLine()) != null)
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
                //no concatenate if the next word doesnt contain capital letter or that the word contain sign at the beginning or at the end
                if (words[i + 1].ToLower().Equals(words[i + 1])|| signs.Contains(words[i][0])|| signs.Contains(words[i][words[i].Length-1]))
                {
                    if (currentWord == "" || (stopWords.Contains(currentWord) && Char.IsLower(currentWord[0]))) // if empty line or stopWord
                        return i;
                    AddTerm(currentDoc, currentWord);
                }
                else //if the next word contains capital letter
                {
                    string temp = currentWord;
                    AddTerm(currentDoc, currentWord);
                    int counter = 0;
                    //check if the next word contains capital letter and doesnt contains signs at the end or the beginning
                    while (!(words[i + 1].ToLower().Equals(words[i + 1])) && !signs.Contains(words[i][0]) && !signs.Contains(words[i][words[i].Length - 1]))
                    {
                        if (words[i + 1].Equals(""))
                        {
                            i++;
                            continue;
                        }
                        currentWord = words[i + 1].ToLower();
                        temp = temp + " " + currentWord;
                        counter++;//num of concatenate
                        AddTerm(currentDoc, currentWord);
                        i = i + 1;
                        if (i + 1 == words.Length)
                        {
                            break;
                        }
                    }
                    this.documentCurrentPosition = this.documentCurrentPosition - counter; //in order to set the position of the expression as the position of the first term
                    AddTerm(currentDoc, temp);
                    this.documentCurrentPosition = this.documentCurrentPosition + counter;
                }
            }
            else //end of the text
            {
                AddTerm(currentDoc, currentWord);
            }
            return i;
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
                        i = i + 1;
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
                    i = i + 1;
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
            if (signs.Contains(termName[termName.Length - 1]) || signs.Contains(termName[0]))
            {
                termName = Regex.Replace(termName, @"[^0-9a-zA-Z ]+", ""); //remove unnecessary chars
                if (termName == "" || (stopWords.Contains(termName) && Char.IsLower(termName[0]))) // if empty line or stopWord
                    return;
            }
            Term t;
            if (terms.ContainsKey(termName))
                t = terms[termName];
            else
            {
                t = new Term(termName);
                terms.Add(termName, t);
            }
            t.UpdateDetails(currentDoc, this.documentCurrentPosition);
            currentDoc.AddTerm(t);
            this.documentCurrentPosition++;
        }
    }
}
