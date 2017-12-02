using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadFile rf = new ReadFile(@"D:\EngineFiles test");
            rf.Seperate();
        }
    }
}
