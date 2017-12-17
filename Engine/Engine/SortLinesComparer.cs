using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class SortLinesComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            StringBuilder sby = new StringBuilder();
            StringBuilder sbx = new StringBuilder();

            int indexX = 0, indexY = 0;
            while(!x[indexX].Equals('\0'))
            {
                if (!x[indexX].Equals('|'))
                {
                    sbx.Append(x[indexX]);
                    indexX++;
                }
                else
                    break;
            }
            while (!y[indexY].Equals('\0'))
            {
                if (!y[indexY].Equals('|'))
                {
                    sby.Append(y[indexY]);
                    indexY++;
                }
                else
                    break;
            }
            return String.Compare(sbx.ToString(), sby.ToString());
        }
    }
}
