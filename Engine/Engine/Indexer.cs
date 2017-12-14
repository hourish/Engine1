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
        Dictionary<string, Document> Docdictionary = new Dictionary<string, Document>(); //the dictionary of the corpus
        //Dictionary<string, string> tempTermDictionary = new Dictionary<string, string>();
        List<string> tempTermList = new List<string>();
        //SortedSet<string> stringToPosting;
        //string path;
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
        public void PrepareToPosting(Term[] terms, Document currentDoc, int size)
        {
            string str = "";
            //StringBuilder
            Docdictionary.Add(currentDoc.GetName(), currentDoc);
            for (int i = 0; i < terms.Length; i++)
            {
                string temp = terms[i].StringToPosting(currentDoc);
                if (temp != "")
                {
                    str = str + temp;
                    tempTermList.Add(str);
                }
                str = "";

            }

        }
        /// <summary>
        /// sort the list of terms, merge the difrrent parts of the term and write them on the temporarly posting file
        /// </summary>
        /// <param name="path"> the where to write the temporarly posting file</param>
        public void CreateTempPostingFile(string path)
        {
            //tempTermDictionary.OrderByDescending(x=>x.Key);
            string str = "";
            tempTermList.Sort();
            for (int i = 0; i < tempTermList.Count; i++)
            {
                str = str + tempTermList[i];
                if (i + 1 != tempTermList.Count)
                {
                    string current = tempTermList[i];
                    string next = tempTermList[i + 1];
                    while (current.Substring(0, current.IndexOf('|')).Equals(next.Substring(0, next.IndexOf('|'))))
                    {
                        str = str + next.Substring(next.IndexOf('|') + 1);
                        i++;
                        if (i + 1 == tempTermList.Count)
                            break;
                        current = tempTermList[i];
                        next = tempTermList[i + 1];
                    }

                }
                str = str + "\n";
            }
            string newPath = path + "\\TempPostingFileNumber_" + tempPostingFilesCounter;
            File.WriteAllText(newPath, str);
            tempPostingFilesCounter++;
            tempTermList.Clear();
        }

        public void Merge(string path1, string path2, string path3, string name)
        {
            string line1, line2;
            string newPath = path3 + "\\" + name;
            SortedDictionary<string, int> tempDic = new SortedDictionary<string, int>();
            StreamReader file1 = new StreamReader(path1);
            StreamReader file2 = new StreamReader(path2);
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
            while (((line1 = file1.ReadLine()) != null) || ((line2 = file2.ReadLine()) != null))
            {
                int count = 1;// checking how many lines we write to the file
                string str = tempDic.ElementAt(0).Key;
                if (tempDic.Count < 2)
                    continue;
                // check if the smallest line and the second smallest line are from the same temprarly Postingfile
                if (tempDic.ElementAt(0).Value == tempDic.ElementAt(1).Value)
                {
                    str = str + "\n" + tempDic.ElementAt(1).Key;
                    count++;
                    if (tempDic.Count > 3)
                        if (tempDic.ElementAt(1).Value == tempDic.ElementAt(2).Value)
                        {
                            str = str + "\n" + tempDic.ElementAt(2).Key;
                            count++;
                        }
                }

                File.WriteAllText(newPath, str);
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
            file1.Close();
            file2.Close();
        }
    }
}