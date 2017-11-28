using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            // ReadFile rf = new ReadFile(@"D:\EngineFiles test");
            Parser parse=new Parser();
            String[] files = System.IO.Directory.GetFiles(@"D:\EngineFiles test", "*.*", System.IO.SearchOption.AllDirectories);
            String[] innerFiles = new String[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                String current = File.ReadAllText(files[i]);
                innerFiles[i] = current;
            }
            parse.Parse(innerFiles); 
            //rf.Seperate();
        }
    }
}
