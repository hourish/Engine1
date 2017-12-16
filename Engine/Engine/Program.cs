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
             string path = @"C:\Users\Shani\Desktop\study\first semester\Ihzur\Engine\corpus\corpus";
             Controller control = new Controller();
             control.Engine(path);

        /*    HashSet<string> hs = new HashSet<string>();
            string[] from = Directory.GetFiles(@".\finalPosting", "*.*", SearchOption.AllDirectories);
            for(int i = 0; i < from.Length; i++)
            {
                StreamReader file = new StreamReader(from[i]);
                List<string> l = new List<string>();
                string line = "";
                StringBuilder sb = new StringBuilder();
                int index = 0;
                while (!file.EndOfStream)
                {
                    if ((line = file.ReadLine()) != null)
                    {
                        sb.Clear();
                        while (!line[index].Equals('\0'))
                        {
                            if (!line[index].Equals('|'))
                            {
                                sb.Append(line[index]);
                                index++;
                            }
                            else
                            {
                                index = 0;
                                break;
                            }
                        }
                        l.Add(sb.ToString());
                        hs.Add(sb.ToString());
                    }
                }

                Console.WriteLine(hs.Count);
                for (int j = 1; j < l.Count; j++)
                {
                    if (String.Compare(l[j - 1], l[j]) > 0)
                    {
                        Console.WriteLine(l[j - 1]);
                        Console.WriteLine(l[j]);
                    }
                }
                hs.clear();

            }
            Console.WriteLine("end");
            Console.ReadLine();*/
        }
    }
}
