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
            ReadFile rf = new ReadFile(@"C:\Users\Shani\Desktop\study\first semester\Ihzur\Engine\corpus\corpus");
            rf.Seperate();
        }
    }
}
