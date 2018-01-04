using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Engine
{
    class Controller
    {
        Dictionary<string, string> cache = new Dictionary<string, string>();
        SortedDictionary<string, string[]> theDictionary = new SortedDictionary<string, string[]>();//term to (total tf, df, postingFileName, position in posting file)
        Indexer indexer = new Indexer();
        Dictionary<string, string[]> DocDictionary = new Dictionary<string, string[]>();
        /// <summary>
        /// run the engine, control all the classes
        /// </summary>
        /// <param name="path"></param>
        public void Engine(string path, string finalPath, bool stem)
        {
            Stemmer stemmer = new Stemmer();
            ReadFile rf = new ReadFile(path);
            Parser parser = new Parser(rf.ReadStopWords(path + "\\stop_words.txt"));
            int filesAmount = rf.FilesAmount();
            Document currentDoc = null;
            string tempPath1 = @"./temp Posting Files1";
            string tempPath2 = @"./temp Posting Files2";
            Directory.CreateDirectory(tempPath1);
            Directory.CreateDirectory(tempPath2);
            Directory.CreateDirectory(finalPath);
            string[] filesInTmp1 = Directory.GetFiles(tempPath1, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < filesInTmp1.Length; i++)
            {
                File.Delete(filesInTmp1[i]);
            }
            string[] filesInTmp2 = Directory.GetFiles(tempPath2, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < filesInTmp2.Length; i++)
            {
                File.Delete(filesInTmp2[i]);
            }
            DirectoryInfo di = new DirectoryInfo(path);
            long size = di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            long avgFilesSize = size / filesAmount;
            long tenPrecent = (size * 9) / 100;
            long numFiles = tenPrecent / avgFilesSize;
            int count = 0;
            //numFiles = 20;           
            for (int i = 0; i < filesAmount; i++)//going through the files in the dictionary and send each to the parser 
            {
                Match matchTEXT = rf.Seperate(i);// get a sperated files from red file
                while (matchTEXT.Success)
                {
                    Term[] terms = parser.Parse(matchTEXT.Groups[1].Value).Values.ToArray();
                    int max = -1;
                    if (stem)
                    {
                        for (int j = 0; j < terms.Length; j++)
                            terms[j].SetName(stemmer.stemTerm(terms[j].GetName()));
                    }
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
                    string[] details = new string[3];
                    details[0] = currentDoc.GetMaxTfString();
                    details[1] = currentDoc.GetLengthString();
                    details[2] = currentDoc.GetDateString();
                    DocDictionary.Add(currentDoc.GetName(), details);
                    matchTEXT = matchTEXT.NextMatch();
                }
                count++;
                if (count == numFiles)
                {
                    Console.WriteLine("create posting");
                    indexer.CreateTempPostingFile(tempPath1);
                    count = 0;
                }
            }//for
            if (count > 0)// if we finished the for and there are still terms in the hash
            {
                indexer.CreateTempPostingFile(tempPath1);

            }
            int temporarlyPostingFolder1 = Directory.GetFiles(tempPath1, "*.*", SearchOption.AllDirectories).Length;
            int temporarlyPostingFolder2 = Directory.GetFiles(tempPath2, "*.*", SearchOption.AllDirectories).Length;
            //continue until there is only two files
            while (!(temporarlyPostingFolder1 == 2 && temporarlyPostingFolder2 == 0) || !(temporarlyPostingFolder1 == 0 && temporarlyPostingFolder2 == 2))
            {
                indexer.SetPostingNumber(0);
                Merge(tempPath1, tempPath2);
                temporarlyPostingFolder1 = Directory.GetFiles(tempPath1, "*.*", SearchOption.AllDirectories).Length;
                temporarlyPostingFolder2 = Directory.GetFiles(tempPath2, "*.*", SearchOption.AllDirectories).Length;
                if (temporarlyPostingFolder1 == 0 && temporarlyPostingFolder2 == 2)
                {
                    string[] temporarlyPostingFolder = Directory.GetFiles(tempPath2, "*.*", SearchOption.AllDirectories);
                    indexer.FinalMerge(temporarlyPostingFolder[0], temporarlyPostingFolder[1], finalPath, stem);
                    File.Delete(temporarlyPostingFolder[0]);
                    File.Delete(temporarlyPostingFolder[1]);
                    break;
                }
                indexer.SetPostingNumber(0);
                Merge(tempPath2, tempPath1);
                temporarlyPostingFolder1 = Directory.GetFiles(tempPath1, "*.*", SearchOption.AllDirectories).Length;
                temporarlyPostingFolder2 = Directory.GetFiles(tempPath2, "*.*", SearchOption.AllDirectories).Length;
                if (temporarlyPostingFolder1 == 2 && temporarlyPostingFolder2 == 0)
                {
                    string[] temporarlyPostingFolder = Directory.GetFiles(tempPath1, "*.*", SearchOption.AllDirectories);
                    indexer.FinalMerge(temporarlyPostingFolder[0], temporarlyPostingFolder[1], finalPath, stem);
                    File.Delete(temporarlyPostingFolder[0]);
                    File.Delete(temporarlyPostingFolder[1]);
                    break;
                }
            }
            theDictionary = indexer.GetFinalDic();
            //cach
            List<KeyValuePair<string, string[]>> tempDic = theDictionary.ToList();
            tempDic = tempDic.OrderByDescending(a => Int32.Parse(a.Value[0])).ToList();//sort by max tf
            for (int i = 0; i < 10000; i++)
            {
                string pathtToPosting = Path.Combine(finalPath, theDictionary[tempDic[i].Key][2]);
                FileStream fs = new FileStream(pathtToPosting, FileMode.Open, FileAccess.Read);
                UTF8Encoding utf8 = new UTF8Encoding();
                BinaryReader br = new BinaryReader(fs, utf8);
                br.BaseStream.Seek(int.Parse(theDictionary[tempDic[i].Key][3]), SeekOrigin.Begin);
                BufferedStream bs = new BufferedStream(fs);
                StreamReader sr = new StreamReader(bs);
                cache.Add(tempDic[i].Key, sr.ReadLine());
            }
            tempDic.Clear();
            Save(finalPath, stem);
        }//engine
         /// <summary>
         /// take every two files from source dictionary merge and save  the new file in the dest dictionary;
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
        /// <summary>
        /// delete the files in path1 and path2 and clear the dictionary and the cache
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        public void Delete(string savePath, string finalPath, bool stem)
        {
            List<char> letters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            if (!stem)
            {
                for (int i = 0; i < 26; i++)
                {
                    File.Delete(finalPath + "/" + letters[i] + "posting");
                }
                File.Delete(finalPath + "/Numbers posting");
                File.Delete(finalPath + "\\Poodle_Dictionary");
                File.Delete(finalPath + "\\Poodle_Cache");
                File.Delete(finalPath + "\\Poodle_Doc");
                if (!savePath.Equals(""))
                {
                    if (!finalPath.Equals(savePath))
                    {
                        for (int i = 0; i < 26; i++)
                        {
                            File.Delete(savePath + "/" + letters[i] + "posting");
                        }
                        File.Delete(savePath + "/Numbers posting");
                        File.Delete(savePath + "\\Poodle_Dictionary");
                        File.Delete(savePath + "\\Poodle_Cache");
                        File.Delete(savePath + "\\Poodle_Doc");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 26; i++)
                {
                    File.Delete(finalPath + "/" + letters[i] + "postingSTM");
                }
                File.Delete(finalPath + "/Numbers postingSTM");
                File.Delete(finalPath + "\\Poodle_DictionarySTM");
                File.Delete(finalPath + "\\Poodle_CacheSTM");
                File.Delete(finalPath + "\\Poodle_DocSTM");
                if (!savePath.Equals(""))
                {
                    if (!finalPath.Equals(savePath))
                    {
                        for (int i = 0; i < 26; i++)
                        {
                            File.Delete(savePath + "/" + letters[i] + "postingSTM");
                        }
                        File.Delete(savePath + "/Numbers postingSTM");
                        File.Delete(savePath + "\\Poodle_DictionarySTM");
                        File.Delete(savePath + "\\Poodle_CacheSTM");
                        File.Delete(savePath + "\\Poodle_DocSTM");
                    }
                }
            }
            theDictionary.Clear();
            cache.Clear();
            DocDictionary.Clear();
        }

        /// <summary>
        /// write the dictionary and the cache to files
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path, bool stem)
        {
            List<string> tempTermList = theDictionary.Keys.ToList();
            List<string> tempTermList1 = cache.Keys.ToList();
            List<string> tempDocsNums = DocDictionary.Keys.ToList();
            string pathDictionery = "";
            string pathCache = "";
            string pathDoc = "";
            if(stem)
            {
                pathDictionery = path + "\\Poodle_DictionarySTM";
                pathCache = path + "\\Poodle_CacheSTM";
                pathDoc = path + "\\Poodle_DocSTM";
            }
            else
            {
                pathDictionery = path + "\\Poodle_Dictionary";
                pathCache = path + "\\Poodle_Cache";
                pathDoc = path + "\\Poodle_Doc";
            }
            StreamWriter saveDictionery = new StreamWriter(pathDictionery);
            StreamWriter saveCache = new StreamWriter(pathCache);
            StreamWriter saveDocs = new StreamWriter(pathDoc);
            for (int i = 0; i < theDictionary.Count; i++)
            {
                saveDictionery.WriteLine(tempTermList[i] + " total tf:" + theDictionary[tempTermList[i]][0] + "" + theDictionary[tempTermList[i]][1] + "~" + theDictionary[tempTermList[i]][2] + "~" + theDictionary[tempTermList[i]][3]);
            }
            for (int i = 0; i < cache.Count; i++)
            {
                saveCache.WriteLine(tempTermList1[i] + "^The Line of the term:^" + cache[tempTermList1[i]]);
            }
            for(int i = 0; i < DocDictionary.Count; i ++)
            {
                saveDocs.WriteLine(tempDocsNums[i] + ":" + DocDictionary[tempDocsNums[i]][0] + ":" + DocDictionary[tempDocsNums[i]][1] + ":" + DocDictionary[tempDocsNums[i]][2]);
            }
            saveDictionery.Close();
            saveCache.Close();
        }

        /// <summary>
        /// compute the size of the index in bytes
        /// </summary>
        /// <param name="finalPath"></param>
        /// <returns></returns>
        public long SizeOfIndex(string finalPath, bool stem)
        {
            long l = 0;
            List<char> letters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            if (!stem)
            {
                for (int i = 0; i < 26; i++)
                {
                    FileInfo fi = new FileInfo(finalPath + "/" + letters[i] + "posting");
                    l += fi.Length;
                }
                FileInfo fi1 = new FileInfo(finalPath + "/Numbers posting");
                FileInfo fi2 = new FileInfo(finalPath + "\\Poodle_Dictionary");
                FileInfo fi3 = new FileInfo(finalPath + "\\Poodle_Cache");
                FileInfo fi4 = new FileInfo(finalPath + "\\Poodle_Doc");
                l += fi1.Length + fi2.Length + fi3.Length + fi4.Length;
            }
            else
            {
                for (int i = 0; i < 26; i++)
                {
                    FileInfo fi = new FileInfo(finalPath + "/" + letters[i] + "postingSTM");
                    l += fi.Length;
                }
                FileInfo fi1 = new FileInfo(finalPath + "/Numbers postingSTM");
                FileInfo fi2 = new FileInfo(finalPath + "\\Poodle_DictionarySTM");
                FileInfo fi3 = new FileInfo(finalPath + "\\Poodle_CacheSTM");
                FileInfo fi4 = new FileInfo(finalPath + "\\Poodle_DocSTM");
                l += fi1.Length + fi2.Length + fi3.Length + fi4.Length;
            }
            return l;
        }

        /// <summary>
        /// get path and load from there the dictionary and cache
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path, bool stem)
        {
            string path1;
            string path2;
            string path3;
            if (stem)
            {
                path1 = path + "\\Poodle_DictionarySTM";
                path2 = path + "\\Poodle_CacheSTM";
                path3 = path + "\\Poodle_DocSTM";
            }
            else
            {
                path1 = path + "\\Poodle_Dictionary";
                path2 = path + "\\Poodle_Cache";
                path3 = path + "\\Poodle_Doc";
            }
            StreamReader file1 = new StreamReader(path1);
            StreamReader file2 = new StreamReader(path2);
            StreamReader file3 = new StreamReader(path3);
            cache = new Dictionary<string, string>();
            theDictionary = new SortedDictionary<string, string[]>();
            DocDictionary = new Dictionary<string, string[]>();
            while (!file1.EndOfStream)
            {
                string line = file1.ReadLine();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < line.IndexOf("total tf:") - 1; i++)
                {
                    sb.Append(line[i]);
                }
                string name = sb.ToString();
                String[] details = new string[4];
                int count = 0;
                for (int i = line.IndexOf("total tf:") + 10; i < line.Length; i++)
                {
                    if (line[i] != '~')
                        sb.Append(line[i]);
                    else
                    {
                        details[count] = sb.ToString();
                        sb.Clear();
                    }

                }
                //  string[] data = line.Split('~');
                // String[] details = { data[1], data[2], data[3], data[4] };
                theDictionary.Add(name, details);
            }
            while (!file2.EndOfStream)
            {
                string line = file2.ReadLine();
                string[] split = line.Split('^');
                cache.Add(split[0], split[2]);
            }
            while (!file3.EndOfStream)
            {
                string line = file3.ReadLine();
                string[] data = line.Split('~');
                if (data.Length != 4)
                    continue;
                string[] details = { data[1], data[2], data[3] };
                DocDictionary.Add(data[0], details);
            }
            file1.Close();
            file2.Close();
            file3.Close();
        }

        public int GetNumOfDocs()
        {
            return DocDictionary.Count();
        }

        public string GetDicPath(bool stem)
        {
            if (stem)
            {
                return "\\Poodle_DictionarySTM";
            }
            else
                return "\\Poodle_Dictionary";
        }

        public string GetCachePath(bool stem)
        {
            if (stem)
            {
                return "\\Poodle_CacheSTM";
            }
            else
                return "\\Poodle_Cache";
        }
    }
}



