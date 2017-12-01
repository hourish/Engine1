using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Engine
{
    class Document
    {
        string name;
        int max_tf;
        int length;
        DateTime date;
        ArrayList terms;
        public Document(string name)
        {
            this.name = name;
            max_tf = 0;
            length = 0;
            date = new DateTime();
            terms = new ArrayList();
        }

        public void SetMax_tf(int max_tf)
        {
            this.max_tf = max_tf;
        }

        public void SetDate(DateTime date)
        {
            this.date = date;
        }

        public void SetLength(int length)
        {
            this.length = length;
        }
    }
}
