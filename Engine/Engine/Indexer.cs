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
        Dictionary<string, StringBuilder> termsToPosting = new Dictionary<string, StringBuilder>();
        int postingNumber;//inner counter to the posting files during each merge from source file to destination file
        int tempPostingFilesCounter;//counter to the posting files during the creation of the first posting files
        public Indexer()
        {
            postingNumber = 0;
            tempPostingFilesCounter = 0;
        }
        /// <summary>
        /// create a string of terms that will be written to a temporarly posting file when there are enogth terms( enoghth= 10% of the courpes)
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="currentDoc"></param>
        public void PrepareToPosting(Term[] terms, Document currentDoc)
        {
            for (int i = 0; i < terms.Length; i++)
            {
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
            List<string> tempTermList = termsToPosting.Keys.ToList();
            StringBuilder str =new StringBuilder("");
            tempTermList.Sort();
            for (int i = 0; i <tempTermList.Count; i++)
            {
                str.Append(tempTermList[i]).Append("|" + termsToPosting[tempTermList[i]].ToString() + "\n");
            }
            termsToPosting.Clear();
            string newPath = path + "\\TempPostingFileNumber_" + tempPostingFilesCounter;
            File.WriteAllText(newPath, str.ToString());
            tempPostingFilesCounter++;
        }

        /// <summary>
        /// merge two files, from pathFile1 and pathFile2, to one file in pathMerge
        /// </summary>
        /// <param name="pathFile1"></param>  path of file1
        /// <param name="pathFile2"></param> path of file2
        /// <param name="pathMerge"></param> path of destination merged file
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

        public void SetPostingNumber(int num)
        {
            postingNumber = num;
        }        
    }
}