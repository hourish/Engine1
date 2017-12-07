using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class Program
    {
        ReadFile rf;
        public Program()
        {
            rf = new ReadFile(@"C:\Users\Shani\Desktop\study\first semester\Ihzur\Engine\dugma")
        }
        static void Main(string[] args)
        {
            ReadFile rf = new ReadFile(@"C:\Users\Shani\Desktop\study\first semester\Ihzur\Engine\dugma");
            rf.Seperate();
        }
    }
}
