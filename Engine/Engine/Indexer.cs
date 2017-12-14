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
        //Dictionary<string, Tuple<int, int>> Docdictionary = new Dictionary<string, Tuple<int, int>>(); //the dictionary of the corpus
        Dictionary<string, Document> Docdictionary = new Dictionary<string,Document>(); //the dictionary of the corpus
        //Dictionary<string, Document> tempDocdictionary = new Dictionary<string, Document>();
        Dictionary<string, StringBuilder> termsToPosting = new Dictionary<string, StringBuilder>();
        //Dictionary<string, string> tempTermDictionary = new Dictionary<string, string>();
        List<string> tempTermList = new List<string>();
        //SortedSet<string> stringToPosting;
        //int index;
        //string path;
        int postingNumber = 0;
        int tempPostingFilesCounter;
        public Indexer()
        {
            //stringToPosting = new SortedSet<string>();
            tempPostingFilesCounter = 0;
        }
        /// <summary>
        /// create a string of terms that will be written to a temporarly posting file when there are enogth terms( enoghth= 10% of the courpes)
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="currentDoc"></param>
        public void PrepareToPosting(Term[] terms, Document currentDoc)
        {
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
        }
        /// <summary>
        /// sort the list of terms, merge the difrrent parts of the term and write them on the temporarly posting file
        /// </summary>
        /// <param name="path"> the where to write the temporarly posting file</param>
        public void CreateTempPostingFile(string path)
        {
            
            StringBuilder str =new StringBuilder("");
            //..termsToPosting=termsToPosting.OrderBy(termsToPosting => termsToPosting.Key);
            for (int i = 0; i < termsToPosting.Count; i++)
            {
                tempTermList.Add(termsToPosting.ElementAt(i).Key + "|" + termsToPosting.ElementAt(i).Value.ToString() + "\n");
            }
            tempTermList.Sort();
                //for (int i= termsToPosting.Count-1; i>=0;i--)
            for (int i = 0; i <tempTermList.Count; i++)
            {
                //str.Append(termsToPosting.ElementAt(i).Key + "|" + termsToPosting.ElementAt(i).Value.ToString() + "\n");
                str.Append(tempTermList[i]);
                //Console.WriteLine(str.ToString());
            }
            termsToPosting.Clear();
            string newPath = path + "\\TempPostingFileNumber_" + tempPostingFilesCounter;
            File.WriteAllText(newPath, str.ToString());
            tempTermList.Clear();
            tempPostingFilesCounter++;

        }

        public void Merge(string pathFile1, string pathFile2, string pathMerge)
        {
            string line1 = null;
            string line2 = null;
            // string newPath = path3 + "\\" + name;
            SortedDictionary<string, int> tempDic = new SortedDictionary<string, int>();

            StreamReader file1 = new StreamReader(pathFile1);
            StreamReader file2 = new StreamReader(pathFile2);
            string pathPosting = pathMerge + "/posting" + postingNumber ;
            //FileStream FilePosting = File.Open(pathPosting, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter posting = new StreamWriter(pathPosting);
            postingNumber++;
                // StreamWriter posting = File.CreateText(pathMerge);
                //StreamWriter posting = new StreamWriter(pathMerge+, true);
                for (int i = 0; i < 3; i++)
                {
                    if ((line1 = file1.ReadLine()) != null)
                        tempDic.Add(line1, 1);
                }
                for (int i = 0; i < 3; i++)
                {
                    if ((line2 = file2.ReadLine()) != null)
                    {
                        if (!tempDic.ContainsKey(line2))
                            tempDic.Add(line2, 2);
                        else
                            i = i - 1;
                    }
                }

                while ((!file1.EndOfStream) || (!file2.EndOfStream))// going throgh the files and stop when he got to the end of them 
                {
                    int count = 1;// checking how many lines we write to the file
                    string str = tempDic.ElementAt(0).Key;
                    if (1 < tempDic.Count)// avoid out of boand
                    {

                        // check if the smallest line and the second smallest line are from the same temprarly Postingfile
                        if (tempDic.ElementAt(0).Value == tempDic.ElementAt(1).Value)
                        {
                            str = str + "\n" + tempDic.ElementAt(1).Key;
                            count++;// how many lines we write to the file
                            if (tempDic.Count > 2)
                                if (tempDic.ElementAt(1).Value == tempDic.ElementAt(2).Value)
                                {
                                    str = str + "\n" + tempDic.ElementAt(2).Key;
                                    count++;
                                }
                        }
                    }
                    posting.Write(str);
                    posting.Flush();
                    //ensert new lines to the dic instead the ones we write to the posting
                    if (tempDic.ElementAt(0).Value == 1) // check if we insert elements from temp posting 1 
                    {
                        for (int i = 0; i < count; i++)
                        {
                            tempDic.Remove(tempDic.ElementAt(0).Key);
                            if ((line1 = file1.ReadLine()) != null)
                            {
                                if (!tempDic.ContainsKey(line1))
                                    tempDic.Add(line1, 1);
                                else
                                    i = i - 1;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++) // check if we insert elements from temp posting 2 
                        {
                            tempDic.Remove(tempDic.ElementAt(0).Key);
                            if ((line2 = file2.ReadLine()) != null)
                            {
                                if (!tempDic.ContainsKey(line2))
                                    tempDic.Add(line2, 2);
                                else
                                    i = i - 1;
                            }
                        }
                    }
                }
                while (tempDic.Count > 0) // tempDic is not empty which means there are stel lines to write
                {
                    posting.WriteLine(tempDic.ElementAt(0).Key);
                posting.Flush();
                    tempDic.Remove(tempDic.ElementAt(0).Key);
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

        public void AddDoucToDectionery(Document currentDoc)
        {
            Docdictionary.Add(currentDoc.GetName(), currentDoc);
            //tempDocdictionary.Add(currentDoc.GetName(), currentDoc);
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