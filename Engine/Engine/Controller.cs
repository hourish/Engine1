using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;



namespace Engine
{
    class Controller
    {
        Indexer indexer = new Indexer();
        /// <summary>
        /// run the engine, control all the classes
        /// </summary>
        /// <param name="path"></param>
        public void Engine(string path)
        {
            ReadFile rf = new ReadFile(path);
            SortedDictionary<string, string[]> theDictionery= new SortedDictionary<string, string[]>();//term to (total tf, df, postingFileName, position in posting file)
            Parser parser = new Parser(rf.ReadStopWords(path + "\\stop_words.txt"));
            Dictionary<string, Document> DocDictionary = new Dictionary<string, Document>();
            int filesAmount = rf.FilesAmount();
            Document currentDoc = null;
            string tempPath = @"./temp Posting Files";
            Directory.CreateDirectory(tempPath);
            string finalPath = @"./finalPosting";
            Directory.CreateDirectory(finalPath);
            DirectoryInfo di = new DirectoryInfo(path);
            long size = di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            long avgFilesSize = size / filesAmount;
            long tenPrecent = (size * 9) / 100;
            long numFiles = tenPrecent / avgFilesSize;
            int count = 0;
            //numFiles = 20;           
            for (int i = 0; i < filesAmount; i++)//going through the files in the dictionery and send each to the parser 
            {
                Match matchTEXT = rf.Seperate(i);// get a sperated files from red file
                while (matchTEXT.Success)
                {
                    Term[] terms = parser.Parse(matchTEXT.Groups[1].Value).Values.ToArray();
                     int max = -1;
                     indexer.PrepareToPosting(terms, currentDoc = parser.GetDoc());
                     for (int j = 0; j < terms.Length; j++)
                     {
                         int currentTF = terms[j].GetTF(currentDoc);
                         if (currentTF > max)
                         {
                             max = currentTF;
                         }
                     }
                    currentDoc.SetMaxTF(max);
                    currentDoc.SetLength(terms.Length);
                    DocDictionary.Add(currentDoc.GetName(), currentDoc);
                    matchTEXT = matchTEXT.NextMatch();
                }          
                count++;
                if (count == numFiles)
                {
                    indexer.CreateTempPostingFile(tempPath);
                    count = 0;
                }
            }//for
            if(count > 0)// if we finished the for and there are still terms in the hash
            {
                indexer.CreateTempPostingFile(tempPath);

            }
            int finalFolder = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories).Length;
            int temporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories).Length;
            //continue until there is only two files
            while (!(temporarlyPostingFolder == 2 && finalFolder == 0) || !(temporarlyPostingFolder == 0 && finalFolder == 2))
            {
                indexer.SetPostingNumber(0);
                Merge(tempPath, finalPath);
                temporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories).Length;
                finalFolder = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories).Length;
                if (temporarlyPostingFolder == 0 && finalFolder == 2)
                {
                    string[] filesAtFinalPath = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories);
                    indexer.FinalMerge(filesAtFinalPath[0], filesAtFinalPath[1], finalPath);
                    File.Delete(filesAtFinalPath[0]);
                    File.Delete(filesAtFinalPath[1]);
                    break;
                }
                indexer.SetPostingNumber(0);
                Merge(finalPath, tempPath);
                temporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories).Length;
                finalFolder = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories).Length;
                if (temporarlyPostingFolder == 2 && finalFolder == 0)
                {
                    string[] filesAtTemporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                    indexer.FinalMerge(filesAtTemporarlyPostingFolder[0], filesAtTemporarlyPostingFolder[1], finalPath);
                    File.Delete(filesAtTemporarlyPostingFolder[0]);
                    File.Delete(filesAtTemporarlyPostingFolder[1]);
                    break;
                }
            }
            theDictionery = indexer.GetFinalDic();
            //cach
            Dictionary<string, string> cache = new Dictionary<string, string>();
            List<string> tempTermList = theDictionery.Keys.ToList();
            List<int> totalTF = new List<int>();
            for(int i = 0; i < theDictionery.Values.Count; i++)
            {
                totalTF.Add(Int32.Parse(theDictionery[tempTermList[i]][0]));
            }
            totalTF.Sort((a, b) => -1 * a.CompareTo(b)); //descending sort
            HashSet<string> maxTF = new HashSet<string>();
            for(int i = 0; i < 10000; i++)
            {
                maxTF.Add(totalTF[i].ToString());
            }
            string s = totalTF[9999].ToString();
            int counter9999 = 0;
            string s1 = totalTF[9998].ToString();
            totalTF.Clear();
            for (int i = 0; i < theDictionery.Count; i++)
            {
                if(maxTF.Contains(theDictionery[tempTermList[i]][0]))
                {
                    if (theDictionery[tempTermList[i]][0].Equals(s))
                    {
                        counter9999++;
                        if(counter9999 > 6)
                        {
                            continue;
                        }
                    }
                    string pathtToPosting = Path.Combine(finalPath, theDictionery[tempTermList[i]][2]);
                    FileStream file = new FileStream(pathtToPosting, FileMode.Open, FileAccess.Read);
                    file.Seek(Int64.Parse(theDictionery[tempTermList[i]][3]), SeekOrigin.Begin);
                    BufferedStream bs = new BufferedStream(file);
                    StreamReader sr = new StreamReader(bs);
                    cache.Add(tempTermList[i], sr.ReadLine());
                }
            }
            tempTermList.Clear();
            List<string> cacheKeys = cache.Keys.ToList();
            List<string> cacheList = cache.Values.ToList();
            for(int i = 0; i < 100; i ++)
            {
                Console.WriteLine(cacheKeys[i] + " " + cacheList[i]);
            }
            theDictionery.Clear();
            cache.Clear();
            }//engine
             /// <summary>
             /// take every two files from source dictionery merge and save  the new file in the dest dictionery;
             /// </summary>
             /// <param name="source"></param>
             /// <param name="dest"></param>
            public void Merge(string source, string dest)
            {
                string[] temporarlyPostingFolder = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
                int index = 0;//if even number of files
                              // if there id odd number of files in the source it move one file to the dest folder
                if (temporarlyPostingFolder.Length % 2 != 0)
                {
                    string fileName = Path.GetFileName(temporarlyPostingFolder[0]);//take a file
                    string destFile = Path.Combine(dest, fileName);//find new path to destination file
                    File.Copy(temporarlyPostingFolder[0], destFile, true);//copy the file to the new path
                    File.Delete(temporarlyPostingFolder[0]);//delete the file from the old path
                    index = 1;
                    indexer.SetPostingNumber(1);
                }
                for (int i = index; i < temporarlyPostingFolder.Length; i = i + 2)
                {
                    indexer.Merge(temporarlyPostingFolder[i], temporarlyPostingFolder[i + 1], dest);//merge two file to destination direcory
                    File.Delete(temporarlyPostingFolder[i]);
                    File.Delete(temporarlyPostingFolder[i + 1]);
                }
            }
        }
    }



