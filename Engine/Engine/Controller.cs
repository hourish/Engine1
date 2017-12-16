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
        
        public void Engine(string path)
        {
            ReadFile rf = new ReadFile(path);
            Parser parser = new Parser(rf.ReadStopWords(path + "\\stop_words.txt"));
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
            for (int i = 0; i < filesAmount; i++)//going through the files in the dictionery and send each to the parser 
            {
                Match matchTEXT = rf.Seperate(i);// get a sperated files from red file
                while (matchTEXT.Success)
                {
                    Term[] terms = parser.Parse(matchTEXT.Groups[1].Value).Values.ToArray();
                     indexer.PrepareToPosting(terms, currentDoc = parser.GetDoc());
                     int max = -1;
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
                  //  Console.WriteLine("finish SetMaxTF and SetLength");
                    indexer.AddDocToDictionary(currentDoc);
                    matchTEXT = matchTEXT.NextMatch();
                }          
                count++;
              //  Console.WriteLine("count " + count + " numFiles " + numFiles);
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
            // continue until there is only two files
            while (!(temporarlyPostingFolder == 2 && finalFolder == 0) || !(temporarlyPostingFolder == 0 && finalFolder == 2))
            {
                indexer.SetPostingNumber(0);
                Merge(tempPath, finalPath);
                temporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories).Length;
                finalFolder = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories).Length;
                if (temporarlyPostingFolder == 0 && finalFolder == 2)
                {
                    string[] from = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                    string fileName = Path.GetFileName(from[0]);//take a file
                }
                if(temporarlyPostingFolder == 2 && finalFolder == 0)
                {
                    break;
                }
                indexer.SetPostingNumber(0);
                Merge(finalPath, tempPath);
                temporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories).Length;
                finalFolder = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories).Length;

                if (temporarlyPostingFolder == 0 && finalFolder == 2)
                {
                    string[] from = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                    string fileName = Path.GetFileName(from[0]);//take a file
                }
                if (temporarlyPostingFolder == 2 && finalFolder == 0)
                {
                    break;
                }
            }

           Console.WriteLine("the end");
           Console.ReadLine();
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
                indexer.Merge(temporarlyPostingFolder[i], temporarlyPostingFolder[i+1], dest);//merge two file to destination direcory
                File.Delete(temporarlyPostingFolder[i]);
                File.Delete(temporarlyPostingFolder[i+1]);
            }
       }
    }
}


