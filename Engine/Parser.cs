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
            HashSet<char> signs = new HashSet<char> { '�', '_', '[', '(', '{', ']', ')', '}', '!', '?', ':', ',', '.', ';', '%', '/', '`', '+', '=', '#', '&', ';' };
            HashSet<char> letters = new HashSet<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            HashSet<char> numbers = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            private Dictionary<string, Term> terms = new Dictionary<string, Term>();
            private int documentCurrentPosition = 0;
            public Parser(HashSet<string> stopWords)
            {
                this.stopWords = stopWords;
            }
            /// <summary>
            /// divide a text into terms according to the engine rulls
            /// </summary>
            /// <param name="str"></param> a text to parse
            public Dictionary<string, Term> Parse(string str)
            {
                documentCurrentPosition = 0;
                terms.Clear();
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
                char[] delimeters = { ' ', '\n', '\r', '-', ']', ')', '}', '\\', '*', '"', '\'', '|' };
                string[] words = str.Split(delimeters);
                int startOfText = 0;
                for (int i = 4; i < words.Length; i++)//loop for the documnet's date the to find the begining of the text. start after the DOCNO
                {
                    string currentWord = words[i];
                    if (currentWord == "" || stopWords.Contains(currentWord)) // if empty line or stopWord
                        continue;
                    while (!words[i].Contains("<DATE")) // set the document date
                        i++;
                    //DocDate
                    currentWord = words[i];
                    int day = 0, year = 0;
                    string month = "";
                    while (!currentWord.Contains("</DATE"))
                    {
                        if (currentWord != "")
                        {
                            if (currentWord.Contains("<DATE") && currentWord.Length >= 10)
                            {
                                if (currentWord.Contains("<DATE1") && currentWord.Length == 13)// for cases like <DATE1>920314
                                {
                                    year = 1900 + (int)Char.GetNumericValue(currentWord[7]) * 10 + (int)Char.GetNumericValue(currentWord[8]);
                                    month += (int)Char.GetNumericValue(currentWord[9]) * 10 + (int)Char.GetNumericValue(currentWord[10]);
                                    day = (int)Char.GetNumericValue(currentWord[11]) * 10 + (int)Char.GetNumericValue(currentWord[12]);
                                }
                                else// for cases like <DATE>920314
                                {
                                    year = 1900 + (int)Char.GetNumericValue(currentWord[6]) * 10 + (int)Char.GetNumericValue(currentWord[7]);
                                    month += (int)Char.GetNumericValue(currentWord[8]) * 10 + (int)Char.GetNumericValue(currentWord[9]);
                                    day = (int)Char.GetNumericValue(currentWord[10]) * 10 + (int)Char.GetNumericValue(currentWord[11]);
                                }
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
                                else if (months.ContainsKey(currentWord))
                                    month = months[currentWord];
                                if (day >= 1 && day <= 31 && year > 100 && months.ContainsValue(month))
                                    break;
                            }
                        }
                        currentWord = words[++i];
                        int tmpStart = 0;
                        int tmpEnd = currentWord.Length;
                        if(!currentWord.Equals(""))
                        {
                            while (signs.Contains(currentWord[tmpStart]))
                            {
                                tmpStart++;
                                if (tmpStart == currentWord.Length)
                                    break;
                            }
                            while(signs.Contains(currentWord[tmpEnd - 1]))
                            {
                                tmpEnd--;
                                if(tmpEnd == 0)
                                    break;
                            }
                        }
                        if(tmpStart != 0)
                            currentWord = currentWord.Substring(tmpStart);
                        if(tmpEnd != currentWord.Length)
                            currentWord = currentWord.Substring(0, tmpEnd);
                    }
                    currentDoc.SetDate(DateToString(day, month, year));
                    if (currentWord == "" || stopWords.Contains(currentWord)) // if empty line or stopWord
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
                    if (words[i].Equals("") || stopWords.Contains(words[i]) || words[i][0].Equals('<') || words[i].Contains('&') || !IsLegal(words[i]) || words[i].Contains('�') || (words[i].Length == 1 && !words[i].Equals("A") && !words[i].Equals("I"))) // if stopWord or illegal word
                        continue;
                    int tmp = 0;
                    while (signs.Contains(words[i][tmp]))
                    {
                        tmp++;
                        if (tmp == words[i].Length)
                            break;
                    }
                    if (tmp == words[i].Length)
                        continue;
                    else if (tmp > 0)
                    {
                        words[i] = words[i].Substring(tmp);
                    }
                    if (!Char.IsUpper(words[i][0]) && signs.Contains(words[i][words[i].Length - 1]))
                        words[i] = words[i].Substring(0, words[i].Length - 1);
                    string currentWord = words[i];//for debug
                                                  /*if (i + 1 < words.Length - 1) // cases like The Aug. 24
                                                  {
                                                          int indexOfCurrentWord = i;
                                                          while (words[i + 1].Equals(""))
                                                          {
                                                              i++;
                                                              if (i == words.Length)
                                                                  break;
                                                          }
                                                          if(i + 1 < words.Length - 1)
                                                              i++;
                                                          if (stopWords.Contains(words[indexOfCurrentWord].ToLower()))
                                                          {
                                                              if ((months.ContainsKey(words[i + 1]) || months.ContainsKey(words[i + 1].Substring(0, words[i + 1].Length - 1))))
                                                              {
                                                                  words[i] = Regex.Replace(words[i], @"[^0-9a-zA-Z ]+", ""); //remove unnecessary chars
                                                                  string month = words[i];
                                                                  i = DateCaseBeginWithMonth(currentDoc, words, i, month);
                                                                  break;
                                                              }
                                                          }
                                                  }*/
                    if (Char.IsNumber(words[i][0])) //if the first char is number
                    {
                        int index = i;
                        bool end = false;
                        bool isDate = true;
                        if (i + 1 < words.Length)
                        {
                            if (words[index + 1].Equals(""))
                            {
                                index++;
                                if (index + 1 < words.Length)
                                {
                                    if (words[index + 1].Equals(""))
                                    {
                                        index++;
                                        if (index + 1 < words.Length)
                                        {
                                            if (words[index + 1].Equals(""))
                                            {
                                                isDate = false;
                                            }
                                        }
                                        else
                                            end = true;
                                    }
                                }
                                else
                                    end = true;
                            }
                            //month case
                            if (!end && isDate)
                            {
                                int day = 0;
                                if (signs.Contains(words[i][words[i].Length - 1]))
                                {
                                    words[i] = words[i].Substring(0, words[i].Length - 1);
                                }
                                if (!Int32.TryParse(words[i], out day) && words[i].Length > 2)//th cases
                                {
                                    if (words[i][words[i].Length - 2].Equals('t') && words[i][words[i].Length - 1].Equals('h'))
                                    {
                                        Int32.TryParse(words[i].Substring(0, words[i].Length - 1), out day);
                                    }
                                }
                                if ((months.ContainsKey(words[index + 1]) || (months.ContainsKey(words[index + 1].Substring(0, words[index + 1].Length - 1)) && signs.Contains(words[index + 1][words[index + 1].Length - 1]))))
                                {
                                    // DateCaseBeginWithNumber
                                    int year = 0;
                                    if (signs.Contains(words[index + 1][words[index + 1].Length - 1]))//remove sign
                                    {
                                        words[index + 1] = words[index + 1].Substring(0, words[index + 1].Length - 1);
                                    }
                                    string month = months[words[index + 1]];
                                    if (currentWord.Contains(','))
                                    {
                                        currentWord.Replace(',', '\0');
                                        //  currentWord = Regex.Replace(currentWord, @"[^0-9]+", ""); //remove unnecessary chars
                                    }

                                    if (i + 2 < words.Length)
                                    {
                                        int index2 = i;
                                        bool end2 = false;

                                        if (words[index2 + 2].Equals(""))
                                        {
                                            index2++;
                                            i++;
                                            if (index2 + 2 < words.Length)//find if there is a year
                                            {
                                                if (words[index2 + 2].Equals(""))
                                                {
                                                    index2++;
                                                    i++;
                                                    if (index2 + 2 < words.Length)
                                                    {
                                                        if (words[index2 + 2].Equals(""))
                                                        {
                                                            AddTerm(DateToString(day, month, year));
                                                            while (words[i + 1].Equals(""))
                                                            {
                                                                i++;
                                                                if (i + 1 == words.Length)
                                                                    break;
                                                            }
                                                            continue;
                                                        }
                                                    }
                                                    else
                                                        end2 = true;
                                                }
                                            }
                                            else
                                                end2 = true;
                                        }
                                        if (!end2)
                                        {
                                            if (signs.Contains(words[index2 + 2][words[index2 + 2].Length - 1]))
                                            {
                                                words[index2 + 2] = words[index2 + 2].Substring(0, words[index2 + 2].Length - 1);
                                            }
                                            if (!int.TryParse(words[index2 + 2], out year))//no year
                                            {
                                                AddTerm(DateToString(day, month, year));
                                                i++;
                                                continue;
                                            }
                                            else
                                            {
                                                if (year < 100) //cases like 12 MAY 91
                                                {
                                                    year += 1900;
                                                }
                                                AddTerm(DateToString(day, month, year));
                                                i = i + 2;
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }//DateCaseBeginWithNumber
                        if (!end && Double.TryParse(words[i], out double d3))
                        {
                            //NumberCase
                            currentWord = words[i];
                            //try
                            // currentWord = Regex.Replace(currentWord, @"[,]", "");//remove unnecessary chars
                            if (!currentWord.Contains(".") && !(currentWord.Contains("%") || words[index + 1].Contains("percentage") || (words[index + 1].Contains("percent"))))//regular number (without percent or .)
                            {
                                if (currentWord.Contains(','))
                                {
                                    currentWord = Regex.Replace(currentWord, @"[^0-9]+", ""); //remove unnecessary chars
                                }
                                AddTerm(currentWord);
                                continue;
                            }
                            else if (currentWord.Contains("."))//check if the number is neede to be round
                            {
                                if (currentWord.Contains(','))
                                {
                                    currentWord = Regex.Replace(currentWord, @"[^0-9]+. %", ""); //remove unnecessary chars
                                }
                                if (!(currentWord.Contains("%") || words[index + 1].Contains("percentage") || (words[index + 1].Contains("percent"))))// cases like 10.567, without percent
                                {
                                    if (double.TryParse(currentWord, out double d))
                                    {
                                        d = Math.Round(d, 2);
                                        AddTerm("" + d);
                                        continue;
                                    }
                                }
                                else //is percent
                                {
                                    if (currentWord[currentWord.Length - 1].Equals('%'))// cases like 10.675%
                                    {
                                        currentWord = currentWord.Substring(0, currentWord.Length - 1);
                                    }
                                    else // cases 10.567 precent and 10.567 precentege
                                    {
                                        i = i + 1;
                                    }
                                    double d = Double.Parse(currentWord);
                                    d = Math.Round(d, 2);
                                    AddTerm(d + " percent");
                                    continue;
                                }
                            }// if cases need to be round
                            else // percent without .
                            {
                                if (currentWord[currentWord.Length - 1].Equals('%'))
                                {
                                    currentWord = currentWord.Substring(0, currentWord.Length - 1);
                                }
                                else if (words[index + 1].Equals("percentage") || words[index + 1].Equals("percent"))
                                {
                                    i = i + 1;
                                }
                                currentWord = currentWord + " percent";
                                AddTerm(currentWord);
                                continue;
                            }
                        }
                    }// if first char is number
                    else if (Char.IsLetter(words[i][0]))
                    {
                        if (i + 1 < words.Length)
                        {
                            int index = i;
                            bool end = false;
                            while (words[index + 1].Equals(""))
                            {
                                index++;
                                if (index + 1 == words.Length)
                                {
                                    end = true;
                                    break;
                                }
                            }
                            // if date start with month
                            if (!end)
                            {
                                if ((months.ContainsKey(words[i]) || (months.ContainsKey(words[i].Substring(0, words[i].Length - 1)) && signs.Contains(words[i][words[i].Length - 1])) && (Int32.TryParse(words[index + 1].Substring(0, words[index + 1].Length - 1), out int d1) || (Int32.TryParse(words[index + 1], out int d2)))))
                                {
                                    if (signs.Contains(words[i][words[i].Length - 1]))
                                        words[i] = words[i].Substring(0, words[i].Length - 1);
                                    string month = words[i];
                                    //date case begin with month
                                    int day = 0, year = 0;
                                    int firstAfterMonthValue = 0;
                                    int secondAfterMonthValue = 0;
                                    if (i + 1 < words.Length)
                                    {
                                        while (words[i + 1].Equals(""))
                                        {
                                            i++;
                                            if (i + 1 == words.Length)
                                                break;
                                        }
                                        if (i + 1 >= words.Length)
                                            break;
                                        string firstAfterMonth = words[i + 1];
                                        if (signs.Contains(firstAfterMonth[firstAfterMonth.Length - 1]))// remove last char if is sign
                                        {
                                            firstAfterMonth = firstAfterMonth.Substring(0, firstAfterMonth.Length - 1);
                                        }
                                        if (i + 2 < words.Length)
                                        {
                                            bool end1 = false;
                                            while (words[i + 2].Equals(""))
                                            {
                                                i++;
                                                if (i + 2 == words.Length)
                                                {
                                                    end1 = true;
                                                    break;
                                                }
                                            }
                                            string secondtAfterMonth = "";
                                            if (!end1)
                                            {
                                                secondtAfterMonth = words[i + 2];
                                                if (signs.Contains(secondtAfterMonth[secondtAfterMonth.Length - 1]))// remove last char if is sign
                                                {
                                                    secondtAfterMonth = secondtAfterMonth.Substring(0, secondtAfterMonth.Length - 1);
                                                }
                                            }
                                            if (Int32.TryParse(firstAfterMonth, out firstAfterMonthValue))//if the next word after the month is number
                                            {
                                                if (!end1 && Int32.TryParse(secondtAfterMonth, out secondAfterMonthValue))//two words after month are numbers
                                                {
                                                    day = firstAfterMonthValue;
                                                    year = secondAfterMonthValue;
                                                    i = i + 2;
                                                }
                                                else//only the first word after month is number
                                                {
                                                    i++;
                                                    if (firstAfterMonth.Length <= 2)// cases like  MAY 4
                                                    {
                                                        day = firstAfterMonthValue;
                                                        year = 0;
                                                    }
                                                    else if (firstAfterMonth.Length == 4)// cases like  MAY 1993
                                                    {
                                                        year = firstAfterMonthValue;
                                                        day = 0;
                                                    }
                                                }
                                            }
                                            else//if the next word after the month is not a number
                                            {
                                                AddTerm(month);
                                                continue;
                                            }
                                        }
                                    }
                                    AddTerm(DateToString(day, months[month], year));
                                    continue;
                                }
                            }
                        }
                        if (Char.IsUpper(words[i][0]))
                        {
                            //CapitalLetterCase
                            currentWord = words[i];
                            currentWord = currentWord.ToLower();
                            if (i + 1 < words.Length)// to avoid out of bound exception
                            {
                                int index = i + 1;
                                bool end = false;
                                while (words[index].Equals(""))
                                {
                                    index++;
                                    if (index == words.Length)
                                    {
                                        end = true;
                                        break;
                                    }
                                }
                                // if date start with month
                                if (!end)
                                {
                                    //no concatenate if the next word doesnt contain capital letter or that the word contain sign at the beginning or at the end or it not legal
                                    if (words[index][0].Equals('<') || !IsLegal(words[index]) || !Char.IsUpper(words[index][0]) ||
                                        signs.Contains(words[index][0]) || signs.Contains(words[i][words[i].Length - 1]))
                                    {
                                        AddTerm(currentWord);
                                        continue;
                                    }
                                    else //if the next word contains capital letter
                                    {
                                        StringBuilder sb = new StringBuilder(currentWord);
                                        AddTerm(currentWord);
                                        i++;
                                        end = false;
                                        if (words[i].Equals(""))
                                        {
                                            i++;
                                            if (words[i].Equals(""))
                                            {
                                                i++;
                                                while (words[i].Equals(""))
                                                {
                                                    i++;
                                                    if (i >= words.Length)
                                                    {
                                                        end = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (!end)
                                        {
                                            int counter = 0;
                                            //check if the next word contains capital letter and doesnt contains signs at the end or the beginning
                                            while (!words[i][0].Equals('<') && IsLegal(words[i]) && Char.IsUpper(words[i][0]) &&
                                            !signs.Contains(words[i][0]))
                                            {
                                                bool finish = false;
                                                if (signs.Contains(words[i][words[i].Length - 1]))
                                                {
                                                    finish = true;
                                                    words[i] = words[i].Substring(0, words[i].Length - 1);
                                                }
                                                currentWord = words[i].ToLower();
                                                sb.Append(" " + currentWord);//לוודא
                                                counter++;//num of concatenate
                                                AddTerm(currentWord);
                                                if (finish)
                                                    break;
                                                i++;
                                                if (i == words.Length)
                                                {
                                                    break;
                                                }
                                                if (words[i].Equals(""))
                                                {
                                                    i++;
                                                    if (i == words.Length)
                                                    {
                                                        break;
                                                    }
                                                    if (words[i].Equals(""))
                                                    {
                                                        break;
                                                    }

                                                }
                                            }
                                            this.documentCurrentPosition = this.documentCurrentPosition - counter; //in order to set the position of the expression as the position of the first term
                                            AddTerm(sb.ToString());
                                            this.documentCurrentPosition = this.documentCurrentPosition + counter;
                                            sb.Clear();
                                        }
                                    }
                                }
                            }
                            else //end of the text
                            {
                                AddTerm(currentWord);
                            }
                        }
                    }
                    else if (words[i][0].Equals('$'))//dollar case
                    {
                        currentWord = currentWord.Substring(1);
                        if (currentWord.Contains(','))
                        {
                            currentWord = Regex.Replace(currentWord, @"[^0-9]+.", ""); //remove unnecessary chars
                        }
                        if (Double.TryParse(currentWord, out double d))
                        {
                            d = Math.Round(d, 2);
                            AddTerm(d + " dollars");
                        }
                    }
                }//second for
            return terms;
            }
            
            /// <summary>
            /// check if currentWord is legal: not conatains letters and number together or not contain nothing to them
            /// </summary>
            /// <param name="currenWord"></param>
            /// <returns></returns>
            private bool IsLegal(string currentWord)
            {
                bool containLetters = false;
                bool containNumbers = false;
                bool res = false;
                //check if the word contains letters and numbers
                for (int i = 0; i < currentWord.Length; i++)
                {
                    if (numbers.Contains(currentWord[i]))
                    {
                        containNumbers = true;
                    }
                    if (letters.Contains(currentWord[i]))
                    {
                        containLetters = true;
                    }
                    if (containLetters && containNumbers)//if the word contains letters and numbers
                        return false;
                }
                if ((containLetters && !containNumbers) || (!containLetters && containNumbers)) //if Legal
                    res = true;
                return res;
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
                        str = "0" + day + "/" + month + "/" + year;
                }
                return str;
            }

            /// <summary>
            /// add new term to the collection and updating the term and document's details accordingly
            /// </summary>
            /// <param name="currentDoc"></param>
            /// <param name="termName"></param>
            /// <param name="position"></param> the position of the term in the current document
            private void AddTerm(string termName)
            {
                if (signs.Contains(termName[termName.Length - 1]))
                {
                    termName = termName.Substring(0, termName.Length - 1); //remove unnecessary chars
                }
                if (termName == "" || stopWords.Contains(termName)) // if empty line or stopWord
                    return;
                if(Char.IsUpper(termName[0]))
                {
                    termName = termName.ToLower();
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
                this.documentCurrentPosition++;
            }
      
            /// <summary>
            /// return the current document
            /// </summary>
            /// <returns></returns>
            public Document GetDoc()
            {
                return currentDoc;
            }
        }
    }