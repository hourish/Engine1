using System;
using System.Collections.Generic;
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
            Console.WriteLine(path);
        }
    }
}
