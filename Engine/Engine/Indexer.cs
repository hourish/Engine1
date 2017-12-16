using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;



namespace Engine
{
    class Indexer
    {
        Dictionary<string, Document> Docdictionary = new Dictionary<string,Document>(); //the dictionary of the corpus
        Dictionary<string, StringBuilder> termsToPosting = new Dictionary<string, StringBuilder>();
        int postingNumber = 0;
        int tempPostingFilesCounter;
        public Indexer()
        {
            tempPostingFilesCounter = 0;
        }
        /// <summary>
        /// create a string of terms that will be written to a temporarly posting file when there are enogth terms( enoghth= 10% of the courpes)
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="currentDoc"></param>
        public void PrepareToPosting(Term[] terms, Document currentDoc)
        {
          //  Console.WriteLine("start PrepareToPosting");
            //string str = "";

            // Docdictionary.Add(currentDoc.GetName(), currentDoc);
            for (int i = 0; i < terms.Length; i++)
            {
                //string temp = terms[i].GetName() + "|";

                if (termsToPosting.ContainsKey(terms[i].GetName()))
                {
                    termsToPosting[terms[i].GetName()].Append(terms[i].StringToPosting(currentDoc));
                }
                else
                {
                    termsToPosting.Add(terms[i].GetName(), new StringBuilder(terms[i].StringToPosting(currentDoc)));
                }
                

            }
           // Console.WriteLine("finish PrepareToPosting");
        }
        /// <summary>
        /// sort the list of terms, merge the difrrent parts of the term and write them on the temporarly posting file
        /// </summary>
        /// <param name="path"> the where to write the temporarly posting file</param>
        public void CreateTempPostingFile(string path)
        {
            Console.WriteLine("CreateTempPostingFile");
            List<string> tempTermList = termsToPosting.Keys.ToList();
            StringBuilder str =new StringBuilder("");
            //..termsToPosting=termsToPosting.OrderBy(termsToPosting => termsToPosting.Key);
          /*  for (int i = 0; i < termsToPosting.Count; i++)
            {
                tempTermList.Add(termsToPosting.ElementAt(i).Key);
            }*/
            Console.WriteLine("CreateTempPostingFile before sort");
            tempTermList.Sort();
            Console.WriteLine("CreateTempPostingFile after sort");
            //for (int i= termsToPosting.Count-1; i>=0;i--)
            for (int i = 0; i <tempTermList.Count; i++)
            {
                //str.Append(termsToPosting.ElementAt(i).Key + "|" + termsToPosting.ElementAt(i).Value.ToString() + "\n");
                str.Append(tempTermList[i]).Append("|" + termsToPosting[tempTermList[i]].ToString() + "\n");
                //Console.WriteLine(str.ToString());
            }
            Console.WriteLine("after for");
            termsToPosting.Clear();
            string newPath = path + "\\TempPostingFileNumber_" + tempPostingFilesCounter;
            File.WriteAllText(newPath, str.ToString());
        //    tempTermList.Clear();
            tempPostingFilesCounter++;

        }

        public void Merge(string pathFile1, string pathFile2, string pathMerge)
        {
            string line1 = null;
            string line2 = null;
            SortedDictionary<string, StringBuilder> tempDic = new SortedDictionary<string, StringBuilder>();//holds 3 of each posting file (1 or 2)
            StreamReader file1 = new StreamReader(pathFile1);
            StreamReader file2 = new StreamReader(pathFile2);
            string pathPosting = pathMerge + "/posting" + postingNumber ;
            StreamWriter posting = new StreamWriter(pathPosting);
            postingNumber++;
            StringBuilder sb = new StringBuilder();
            string last1 = "";
            string last2 = "";
            bool shirshur = false;
            if ((!file1.EndOfStream))
            {
                    if ((line1 = file1.ReadLine()) != null)
                    {
                        sb.Clear();
                        int index = 0;
                        while (!line1[index].Equals('\0'))
                        {
                            if (!line1[index].Equals('|'))
                            {
                                sb.Append(line1[index]);
                                index++;
                            }
                            else
                                break;
                        }
                        if (!tempDic.ContainsKey(sb.ToString()))
                        {
                            tempDic.Add(sb.ToString(), new StringBuilder(line1));
                        }
                        else
                        {
                            tempDic[sb.ToString()].Append(line1.Replace(sb.ToString() + "|", ""));
                        }
                         last1 = sb.ToString();
                    }  
            }
            if ((!file2.EndOfStream))
            {
                    if ((line2 = file2.ReadLine()) != null)
                    {
                        sb.Clear();
                        int index = 0;
                        while (!line2[index].Equals('\0'))
                        {
                            if (!line2[index].Equals('|'))
                            {
                                sb.Append(line2[index]);
                                index++;
                            }
                            else
                                break;
                        }
                        if (!tempDic.ContainsKey(sb.ToString()))
                        {
                            tempDic.Add(sb.ToString(), new StringBuilder(line2));
                        }
                        else
                        {
                            tempDic[sb.ToString()].Append(line2.Replace(sb.ToString() + "|", ""));
                            shirshur = true;
                        }
                        last2 = sb.ToString();
                }
            }
            while ((!file1.EndOfStream) && (!file2.EndOfStream))// going throgh the files and stop when he got to the end of them 
            {
               // sb.Clear();
              //  sb.Append(tempDic.ElementAt(0).Value).Append("\n");
                posting.Write(tempDic.ElementAt(0).Value + "\n");
                posting.Flush();
                tempDic.Remove(tempDic.ElementAt(0).Key);
                if (!shirshur)
                {
                    if (String.Compare(last1, last2) < 0)
                    {
                        if ((!file1.EndOfStream))
                        {
                            if ((line1 = file1.ReadLine()) != null)
                            {
                                sb.Clear();
                                int index = 0;
                                while (!line1[index].Equals('\0'))
                                {
                                    if (!line1[index].Equals('|'))
                                    {
                                        sb.Append(line1[index]);
                                        index++;
                                    }
                                    else
                                        break;
                                }
                                if (!tempDic.ContainsKey(sb.ToString()))
                                {
                                    tempDic.Add(sb.ToString(), new StringBuilder(line1));
                                    shirshur = false;
                                }
                                else
                                {
                                    tempDic[sb.ToString()].Append(line1.Replace(sb.ToString() + "|", ""));
                                    shirshur = true;
                                }
                                last1 = sb.ToString();
                            }
                        }
                    }
                    else
                    {
                        if ((!file2.EndOfStream))
                        {
                            if ((line2 = file2.ReadLine()) != null)
                            {
                                sb.Clear();
                                int index = 0;
                                while (!line2[index].Equals('\0'))
                                {
                                    if (!line2[index].Equals('|'))
                                    {
                                        sb.Append(line2[index]);
                                        index++;
                                    }
                                    else
                                        break;
                                }
                                if (!tempDic.ContainsKey(sb.ToString()))
                                {
                                    tempDic.Add(sb.ToString(), new StringBuilder(line2));
                                    shirshur = false;
                                }
                                else
                                {
                                    tempDic[sb.ToString()].Append(line2.Replace(sb.ToString() + "|", ""));
                                    shirshur = true;
                                }
                                last2 = sb.ToString();
                            }
                        }
                    }
                }
                else
                {
                    if ((!file1.EndOfStream))
                    {
                        if ((line1 = file1.ReadLine()) != null)
                        {
                            sb.Clear();
                            int index = 0;
                            while (!line1[index].Equals('\0'))
                            {
                                if (!line1[index].Equals('|'))
                                {
                                    sb.Append(line1[index]);
                                    index++;
                                }
                                else
                                    break;
                            }
                            if (!tempDic.ContainsKey(sb.ToString()))
                            {
                                tempDic.Add(sb.ToString(), new StringBuilder(line1));
                                shirshur = true;
                            }
                            else
                            {
                                tempDic[sb.ToString()].Append(line1.Replace(sb.ToString() + "|", ""));
                                shirshur = false;
                            }
                            last1 = sb.ToString();
                        }
                    }
                    if ((!file2.EndOfStream))
                    {
                        if ((line2 = file2.ReadLine()) != null)
                        {
                            sb.Clear();
                            int index = 0;
                            while (!line2[index].Equals('\0'))
                            {
                                if (!line2[index].Equals('|'))
                                {
                                    sb.Append(line2[index]);
                                    index++;
                                }
                                else
                                    break;
                            }
                            if (!tempDic.ContainsKey(sb.ToString()))
                            {
                                tempDic.Add(sb.ToString(), new StringBuilder(line2));
                                shirshur = false;
                            }
                            else
                            {
                                tempDic[sb.ToString()].Append(line2.Replace(sb.ToString() + "|", ""));
                                shirshur = true;
                            }
                            last2 = sb.ToString();
                        }
                    }
                }
            }
            if(!file1.EndOfStream)
            {
                while (!file1.EndOfStream)
                {
                    if ((line1 = file1.ReadLine()) != null)
                    {
                        posting.WriteLine(line1);
                        posting.Flush();
                    }
                }  
            }
            else if(!file2.EndOfStream)
            {
                while (!file2.EndOfStream)
                {
                    if ((line2 = file2.ReadLine()) != null)
                    {
                        posting.WriteLine(line1);
                        posting.Flush();
                    }
                }
            }
       
            file1.Close();
            file2.Close();
            posting.Close();
        }
                
        public int GetPostingNumber()
        {
            return postingNumber;
        }

        public void SetPostingNumber(int num)
        {
            postingNumber = num;
        }

        public void AddDoucToDictionary(Document currentDoc)
        {
            Docdictionary.Add(currentDoc.GetName(), currentDoc);
        }           
    }
}

/*
         string toWrite = "";
        private static int tempPostingFilesCounter = 0;

        public void PrepareToPosting(Term[] terms, Document currentDoc)
        {
            Array.Sort<Term>((Engine.Term[])terms);
            for (int i = 0; i < terms.Length; i++)//all terms
            {
                toWrite += terms[i].StringToPosting();
            }
        }

        public void CreateTempPostingFile(string path)
        {
            string newPath = path + "\\TempPostingFileNumber_" + tempPostingFilesCounter;
            File.WriteAllText(newPath, toWrite);
            tempPostingFilesCounter++;
            toWrite = "";
        }
*/