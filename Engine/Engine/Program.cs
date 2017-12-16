using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Engine
{
    class Program
    {
        static void  Main(string[] args)
        {
            //File.Create(@"./temporarly posting file");
            //TextWriter tw = new StreamWriter(@"./temporarly posting file");
            //tw.WriteLine("The very first line!");
            //tw.Close();
           // StreamWriter posting = StreamWriter("log.txt");
          //  posting.WriteLine("hello");
            // long i = new DirectoryInfo(@"D:\EngineFiles\corpus\corpus").GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
            // double precent=(i*10)/100;
          
          string path = @"D:\EngineFiles\corpus\corpus";

           // string path = @"D:\EngineFiles\corpus\corpus";
            Controller control = new Controller();
            control.Engine(path);
            /*
            int postingNumber = 1;
            string pathPosting = @"./finalPosting" + "/posting" + postingNumber;
            StreamWriter posting = new StreamWriter(pathPosting);
            posting.Write("First line of example");
            posting.Flush();
            posting.Close();
            */



        }
    }
}
