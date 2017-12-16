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
        SortedDictionary<string, string[]> finalDic = new SortedDictionary<string, string[]>();
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
                
                string temp;
                int sum;
                if (termsToPosting.ContainsKey(terms[i].GetName()))
                {
                    temp = terms[i].StringToPosting(currentDoc);
                    string temp2 = temp.Substring(temp.IndexOf("=")+1);
                    sum = Int32.Parse(temp2);
                    int length = termsToPosting[terms[i].GetName()].ToString().Length - termsToPosting[terms[i].GetName()].ToString().IndexOf("=")-1;//
                    string currentTerm = termsToPosting[terms[i].GetName()].ToString(termsToPosting[terms[i].GetName()].ToString().IndexOf("=")+1, length);
                    sum = sum + Int32.Parse(currentTerm);
                    currentTerm = "=" + currentTerm;
                    termsToPosting[terms[i].GetName()].Replace(currentTerm,"");
                    termsToPosting[terms[i].GetName()].Append(temp.Substring(0, temp.IndexOf("=") ));
                    termsToPosting[terms[i].GetName()].Append("=").Append(sum);


                }
                else
                {
                    string termString = terms[i].StringToPosting(currentDoc);
                    termsToPosting.Add(terms[i].GetName(), new StringBuilder(termString));
                   
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
            string pathPosting = pathMerge + "/posting" + postingNumber;
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
            if (!file1.EndOfStream)
            {
                while (!file1.EndOfStream)
                {
                    if ((line1 = file1.ReadLine()) != null)
                    {
                        posting.WriteLine(line1.Substring(line1.IndexOf("|")));
                        posting.Flush();
                    }
                }
            }
            else if (!file2.EndOfStream)
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
        
        public void FinalMerge(string pathFile1, string pathFile2, string finalPath)
        {


            string line1 = null;
            string line2 = null;
            List<string> read1 = new List< string >();
            List<string> read2 = new List<string>();

            StringBuilder sb = new StringBuilder();
            StreamReader file1 = new StreamReader(pathFile1);
            StreamReader file2 = new StreamReader(pathFile2);
            String name = "Number posting";
            string pathPosting= finalPath+"/Numbers posting";
            SortedDictionary<string, StringBuilder> tempDic = new SortedDictionary<string, StringBuilder>();//key= term value= name of the file, position, number of files
            
            StreamWriter final_posting = new StreamWriter(pathPosting);
            StringBuilder term1 = new StringBuilder();
            StringBuilder term2 = new StringBuilder();
           List<char> letters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            Posting(final_posting, file1, file2, '0', "Number Posting");
            final_posting.Close();
            for(int i=0; i<27; i++)
            {
                pathPosting = finalPath + "/" + letters[i]+ "posting";
                final_posting = new StreamWriter(pathPosting);
                Posting(final_posting, file1, file2, char.ToLower(letters[i]) , letters[i]+ "Posting");
                final_posting.Close();
            }
            /*
            pathPosting = finalPath + "/A posting";
            final_posting = new StreamWriter(pathPosting);
                     Posting(final_posting, file1, file2, 'a', "A Posting");
            final_posting.Close();
            pathPosting = finalPath + "/B posting";
            final_posting = new StreamWriter(pathPosting);
                     Posting(final_posting, file1, file2, 'b', "B Posting");
            final_posting.Close();
            pathPosting = finalPath + "/C posting";
            final_posting = new StreamWriter(pathPosting);
            Posting(final_posting, file1, file2, 'c', "C Posting");
            final_posting.Close();
            pathPosting = finalPath + "/D posting";
            final_posting = new StreamWriter(pathPosting);
            Posting(final_posting, file1, file2, 'd', "D Posting");
            final_posting.Close();
            pathPosting = finalPath + "/E posting";
            Posting(final_posting, file1, file2, 'e', "E Posting");
            final_posting.Close();
            pathPosting = finalPath + "/F posting";
            Posting(final_posting, file1, file2, 'f', "F Posting");
            final_posting.Close();
            pathPosting = finalPath + "/H posting";
            Posting(final_posting, file1, file2, 'h', "H Posting");
            final_posting.Close();
            pathPosting = finalPath + "/I posting";
            Posting(final_posting, file1, file2, 'i', "I Posting");
            final_posting.Close();
            pathPosting = finalPath + "/J posting";
            Posting(final_posting, file1, file2, 'j', "J Posting");
            final_posting.Close();
            pathPosting = finalPath + "/K posting";
            Posting(final_posting, file1, file2, 'k', "K Posting");
            final_posting.Close();
            pathPosting = finalPath + "/L posting";
            Posting(final_posting, file1, file2, 'l', "L Posting");
            final_posting.Close();
            pathPosting = finalPath + "/M posting";
            Posting(final_posting, file1, file2, 'm', "M Posting");
            final_posting.Close();
            pathPosting = finalPath + "/N posting";
            Posting(final_posting, file1, file2, 'n', "N Posting");
            */
            Console.WriteLine("F");

        }

        public void Posting(StreamWriter final_posting, StreamReader file1, StreamReader file2, char topic, string name )
        {
            string line1;
            string line2;
            string[] termDetails = new string[3];// termDetails[0]= df, termDetails[1]= name of final postnig file,  termDetails[2]= position in the posting file
            StringBuilder term1 = new StringBuilder();
            StringBuilder term2 = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            if ((line1 = file1.ReadLine()) != null)
            {
                term1.Clear();
                int index = 0;
                while (!line1[index].Equals('\0'))
                {
                    if (!line1[index].Equals('|'))
                    {
                        term1.Append(line1[index]);
                        index++;
                    }
                    else
                        break;
                }
            }
            if ((line2 = file2.ReadLine()) != null)
            {
                term2.Clear();
                int index = 0;
                while (!line2[index].Equals('\0'))
                {
                    if (!line2[index].Equals('|'))
                    {
                        term2.Append(line2[index]);
                        index++;
                    }
                    else
                        break;
                }
            }
            StringBuilder lineToWrite = new StringBuilder();
            bool cTerm1 = Condition(term1, topic);
            bool cTerm2 = Condition(term2, topic);
            while (!(file1.EndOfStream) && !(file2.EndOfStream) && Condition(term1, topic) && Condition(term2, topic))
            {
                lineToWrite.Clear();
                int later = 0;
                
                if (term1.ToString().CompareTo(term2.ToString()) < 0)
                {

                    later = 1;
                    lineToWrite.Append(line1.Substring(line1.IndexOf("|")+1));
                    sb = new StringBuilder(term1.ToString()) ;
                    if ((line1 = file1.ReadLine()) != null)
                    {
                        term1.Clear();
                        int index = 0;
                        while (!line1[index].Equals('\0'))
                        {
                            if (!line1[index].Equals('|'))
                            {
                                term1.Append(line1[index]);
                                index++;
                            }
                            else
                                break;
                        }
                    }
                    while (sb.Equals(term1) && !(file1.EndOfStream))
                    {

                        int sum;
                        string temp;
                        temp = line1.Substring(line1.IndexOf("|") + 1); // the position of new term
                        string temp2 = temp.Substring(temp.IndexOf("=") + 1);// take the appernce summerize of the new term 
                        sum = Int32.Parse(temp2);
                        int length = lineToWrite.ToString().Length - lineToWrite.ToString().IndexOf("=") - 1;// the length of first term without the summerize
                        string currentTerm = lineToWrite.ToString(lineToWrite.ToString().IndexOf("=") + 1, length); // the  first term without the summerize
                        sum = sum + Int32.Parse(currentTerm);
                        currentTerm = "=" + currentTerm;
                        lineToWrite.Replace(currentTerm, "");
                        lineToWrite.Append(temp.Substring(0, temp.IndexOf("=")));
                        lineToWrite.Append("=").Append(sum);
                        if ((line1 = file1.ReadLine()) != null)
                        {
                            term1.Clear();
                            int index = 0;
                            while (!line1[index].Equals('\0'))
                            {
                                if (!line1[index].Equals('|'))
                                {
                                    term1.Append(line1[index]);
                                    index++;
                                }
                                else
                                    break;
                            }

                        }

                    }
                }
                ///////////////
                else if (term1.ToString().CompareTo(term2.ToString()) > 0)
                {
                    later = 2;
                    lineToWrite.Append(line2.Substring(line2.IndexOf("|")+1));
                    sb = new StringBuilder(term2.ToString());
                    if ((line2 = file2.ReadLine()) != null)
                    {
                        term2.Clear();
                        int index = 0;
                        while (!line2[index].Equals('\0'))
                        {
                            if (!line2[index].Equals('|'))
                            {
                                term2.Append(line2[index]);
                                index++;
                            }
                            else
                                break;
                        }
                    }
                    while (sb.Equals(term1) && !(file1.EndOfStream))
                    {

                        int sum;
                        string temp;
                        temp = line2.Substring(line2.IndexOf("|") + 1); // the position of new term
                        string temp2 = temp.Substring(temp.IndexOf("=") + 1);// take the appernce summerize of the new term 
                        sum = Int32.Parse(temp2);
                        int length = lineToWrite.ToString().Length - lineToWrite.ToString().IndexOf("=") - 1;// the length of first term without the summerize
                        string currentTerm = lineToWrite.ToString(lineToWrite.ToString().IndexOf("=") + 1, length); // the  first term without the summerize
                        sum = sum + Int32.Parse(currentTerm);
                        currentTerm = "=" + currentTerm;
                        lineToWrite.Replace(currentTerm, "");
                        lineToWrite.Append(temp.Substring(0, temp.IndexOf("=")));
                        lineToWrite.Append("=").Append(sum);
                        if ((line2 = file2.ReadLine()) != null)
                        {
                            term2.Clear();
                            int index = 0;
                            while (!line2[index].Equals('\0'))
                            {
                                if (!line2[index].Equals('|'))
                                {
                                    term2.Append(line2[index]);
                                    index++;
                                }
                                else
                                    break;
                            }
                        }


                    }

                }
                else
                {
                    later = 0;
                    sb = new StringBuilder(term1.ToString());
                    lineToWrite.Append(line2.Substring(line1.IndexOf("|")));
                    int sum;
                    string temp;
                    temp = line1.Substring(line1.IndexOf("|")+1); // the position of new term
                    string temp2 = temp.Substring(temp.IndexOf("=") + 1);// take the appernce summerize of the new term 
                    sum = Int32.Parse(temp2);
                    int length = lineToWrite.ToString().Length - lineToWrite.ToString().IndexOf("=") - 1;// the length of first term without the summerize
                    string currentTerm = lineToWrite.ToString(lineToWrite.ToString().IndexOf("=") + 1, length); // the  first term without the summerize
                    sum = sum + Int32.Parse(currentTerm);
                    currentTerm = "=" + currentTerm;
                    lineToWrite.Replace(currentTerm, "");
                    lineToWrite.Append(temp.Substring(0, temp.IndexOf("=")));
                    lineToWrite.Append("=").Append(sum);
                    if ((line1 = file1.ReadLine()) != null)
                    {
                        term1.Clear();
                        int index = 0;
                        while (!line1[index].Equals('\0'))
                        {
                            if (!line1[index].Equals('|'))
                            {
                                term1.Append(line1[index]);
                                index++;
                            }
                            else
                                break;
                        }

                    }
                    if ((line2 = file2.ReadLine()) != null)
                    {
                        term2.Clear();
                        int index = 0;
                        while (!line2[index].Equals('\0'))
                        {
                            if (!line2[index].Equals('|'))
                            {
                                term2.Append(line2[index]);
                                index++;
                            }
                            else
                                break;
                        }
                    }


                }

                int filesAmount = 0;// the amount of files the term apper in
                for (int i = 0; i < lineToWrite.Length; i++)
                {
                    if (lineToWrite[i].Equals('?'))
                        filesAmount++;
                }
                termDetails[0] = filesAmount.ToString();
                termDetails[1] = name;
                termDetails[2] = final_posting.BaseStream.Length.ToString();
                
                if (filesAmount > 2)
                {
                    final_posting.WriteLine(lineToWrite.ToString() + "/n");
                    if(!finalDic.ContainsKey(sb.ToString()))
                    finalDic.Add(sb.ToString(), termDetails);
                }

            }//while 
            
            if (!(file1.EndOfStream) && !(file2.EndOfStream))
            {
               
                while (Condition(term1, topic) && !Condition(term2, topic))
                {
                    int filesAmount = 0;// the amount of files the term apper in
                    for (int i = 0; i < line1.Length; i++)
                    {
                        if (line1[i].Equals('?'))
                            filesAmount++;
                    }
                    termDetails[0] = filesAmount.ToString();
                    termDetails[1] = name;
                    termDetails[2] = final_posting.BaseStream.Length.ToString();
                    final_posting.WriteLine(line1.Substring(line1.IndexOf("|")+1)+"/n");
                    if ((line1 = file1.ReadLine()) != null)
                    {
                        term1.Clear();
                        int index = 0;
                        while (!line1[index].Equals('\0'))
                        {
                            if (!line1[index].Equals('|'))
                            {
                                term1.Append(line1[index]);
                                index++;
                            }
                            else
                                break;
                        }

                    }
                    else
                        break;
                    

                }
                while (!Condition(term1, topic) && Condition(term2, topic))
                {
                    int filesAmount = 0;// the amount of files the term apper in
                    for (int i = 0; i < line2.Length; i++)
                    {
                        if (line2[i].Equals('?'))
                            filesAmount++;
                    }
                    termDetails[0] = filesAmount.ToString();
                    termDetails[1] = name;
                    termDetails[2] = final_posting.BaseStream.Length.ToString();
                    if (filesAmount > 2)
                    {
                        final_posting.WriteLine(line2.Substring(line2.IndexOf("|") + 1) + "/n");
                        finalDic.Add(term2.ToString(), termDetails);
                    }
                    if ((line2 = file2.ReadLine()) != null)
                    {
                        term2.Clear();
                        int index = 0;
                        while (!line2[index].Equals('\0'))
                        {
                            if (!line2[index].Equals('|'))
                            {
                                term2.Append(line2[index]);
                                index++;
                            }
                            else
                                break;
                        }
                    }
                    else
                        break;
                   
                }

            }
            
            else if (!(file1.EndOfStream))
             {
                while (!(file1.EndOfStream))
                {
                    int filesAmount = 0;// the amount of files the term apper in
                    for (int i = 0; i < line1.Length; i++)
                    {
                        if (line1[i].Equals('?'))
                            filesAmount++;
                    }
                    termDetails[0] = filesAmount.ToString();
                    termDetails[1] = name;
                    termDetails[2] = final_posting.BaseStream.Length.ToString();
                    if (filesAmount > 2)
                    {
                        final_posting.WriteLine(line1.Substring(line1.IndexOf("|") + 1));
                        finalDic.Add(sb.ToString(), termDetails);
                    }
                        if ((line1 = file1.ReadLine()) != null)
                    {
                        term1.Clear();
                        int index = 0;
                        while (!line1[index].Equals('\0'))
                        {
                            if (!line1[index].Equals('|'))
                            {
                                term1.Append(line1[index]);
                                index++;
                            }
                            else
                                break;
                        }
                    }
                }
               
            }
            else
            {
                
                while (!file2.EndOfStream)
                {
                    int filesAmount = 0;// the amount of files the term apper in
                    for (int i = 0; i < line1.Length; i++)
                    {
                        if (line1[i].Equals('?'))
                            filesAmount++;
                    }
                    termDetails[0] = filesAmount.ToString();
                    termDetails[1] = name;
                    termDetails[2] = final_posting.BaseStream.Length.ToString();
                    if (filesAmount > 2)
                    {
                        final_posting.WriteLine(line1.Substring(line2.IndexOf("|") + 1));
                        finalDic.Add(sb.ToString(), termDetails);
                    }
                        if ((line1 = file1.ReadLine()) != null)
                    {
                        term1.Clear();
                        int index = 0;
                        while (!line1[index].Equals('\0'))
                        {
                            if (!line1[index].Equals('|'))
                            {
                                term1.Append(line1[index]);
                                index++;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            }
        

            public bool Condition ( StringBuilder term1,  Char topic)
        {
            bool ans= false;
            string temp = term1.ToString();
            if (topic.Equals('0'))
            {
                ans = char.IsNumber(temp[0]) ; 
            }
            else
            {
                temp = temp.ToLower();
                if (temp[0].Equals(topic))
                ans =true ;
            }
            return ans;
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
    }//class
}//nameSpace

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