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
            Controller c = new Controller();
            c.Engine(@"C:\Users\Shani\Desktop\study\first semester\Ihzur\Engine\corpus\corpus");
        }
    }
}
