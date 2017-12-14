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
            // path = @"D:\EngineFiles test";
            ReadFile rf = new ReadFile(path);
            HashSet<string> stopWords = new HashSet<string>();
            stopWords = rf.ReadStopWords(path + "\\stop_words.txt");
            Parser parser = new Parser(stopWords);
            int filesAmount = rf.FilesAmount();
            Dictionary<string, Term> terms = new Dictionary<string, Term>();
            Document currentDoc = null;
            string tempPath = @"./temp Posting Files";
            Directory.CreateDirectory(tempPath);
            string finalPath = @"./finalPosting";
            Directory.CreateDirectory(finalPath);
            // Console.WriteLine(filesAmount);
            DirectoryInfo di = new DirectoryInfo(path);
            long size = di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            long avgFilesSize = size / filesAmount;
            long tenPrecent = (size * 9) / 100;
            long numFiles = tenPrecent / avgFilesSize;
            int count = 0;
            //Console.WriteLine(size);

            for (int i = 0; i < filesAmount; i++)//going through the files in the dictionery and send each to the parser 
            {
                Match matchTEXT = rf.Seperate(i);// get a sperated files from red file
                while (matchTEXT.Success)
                {
                    terms = parser.Parse(matchTEXT.Groups[1].Value);

                    currentDoc = parser.GetDoc();
                       indexer.AddDoucToDectionery(currentDoc);
                       indexer.PrepareToPosting(terms.Values.ToArray(), currentDoc);
                        currentDoc.SetMaxTF();
                    
                    matchTEXT = matchTEXT.NextMatch();

                }
               
                //count++;

                //   if (count == numFiles)
                /////למחוק מפה
               
                      if(i!=0)
                   {
                       //indexer.PrepareToPosting(terms.Values.ToArray(), currentDoc);
                       indexer.CreateTempPostingFile(tempPath);
                       terms.Clear();
                       count = 0;
                  }



               }//for
               if(terms.Count>0) // if we finished the for and there are still terms in the hash
               {
                   indexer.CreateTempPostingFile(tempPath);
                   terms.Clear();
               }
               string[] finalFolder = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories);
               string[] temporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
               // continue until there is just one file in one of the folders
               while (temporarlyPostingFolder.Length >= 1 && finalFolder.Length == 0)
               {
                   indexer.SetPostingNumber(0);
                   Merge(tempPath, finalPath);
                   temporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                   finalFolder = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories);
                   // if (temporarlyPostingFolder.Length == 1 && !Directory.EnumerateFiles(finalPath).Any())// if the final posting is in the temp directory so it move it to the right directory
                   if (temporarlyPostingFolder.Length == 0 && finalFolder.Length == 1) { 
                       break;
                   }
                   else
                   {
                       Merge(finalPath, tempPath);
                       temporarlyPostingFolder = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                       finalFolder = Directory.GetFiles(finalPath, "*.*", SearchOption.AllDirectories);

                   }

               }
               
              Console.WriteLine("the end");
          }//engine


          //}//Engine
          /// <summary>
          /// take every two files from source dictionery merge and save  the new file in the dest dictionery;
          /// </summary>
          /// <param name="source"></param>
          /// <param name="dest"></param>
          public void Merge(string source, string dest)
              {
                  string[] temporarlyPostingFolder = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
                  int index = 0;
              // if there id odd number of files in the source it move one file to the dest folder
                  if (temporarlyPostingFolder.Length % 2 != 0)
                  {
                      string fileName = System.IO.Path.GetFileName(temporarlyPostingFolder[0]);
                      string destFile = System.IO.Path.Combine(dest, fileName);
                      System.IO.File.Copy(temporarlyPostingFolder[0], destFile, true);
                      File.Delete(temporarlyPostingFolder[0]);
                      index = 1;
                  }
                  for (int i = index; i < temporarlyPostingFolder.Length; i = i + 2)
                  {

                      indexer.Merge(temporarlyPostingFolder[i], temporarlyPostingFolder[i+1], dest);
                      File.Delete(temporarlyPostingFolder[i]);
                      File.Delete(temporarlyPostingFolder[i+1]);
                  }


              }
              
            
        }
    }


