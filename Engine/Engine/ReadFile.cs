using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class ReadFile
    {
        string path;
        public ReadFile(string path)
        {
            this.path = path;
        }

        public void Seperate()
        {
         
            String[]files=System.IO.Directory.GetFiles(path,"*.*",System.IO.SearchOption.AllDirectories);
            List<String> innerFiles = new List<string>();
            for (int i=0; i<files.Length;i++)
            {
                String current = File.ReadAllText(files[i]);
                 
                String line = "";
                //innerFiles.Add(File.ReadAllText(files[i]));
                //while (current.Equals("")
                  //  {
                    //  int begining= current.IndexOf("")
                //}
                /*
                using (StreamReader file = new StreamReader(files[i]))
                {
                    
                    bool begining = false;
                    while ((line= file.ReadLine()) != null)
                    {
                       // begining =true;
                        if (line.Equals("<DOC>"))
                        {
                            while (!line.Equals("</DOC>"))
                            {
                                //if(begining)
                                current += file.ReadLine();
                            }                       
                               innerFiles.Add(current);
                        }
                    }
                }
                */

        }
            
            
            Console.WriteLine("hey");

        }
    }
}
