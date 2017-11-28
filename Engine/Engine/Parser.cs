using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Engine
{
    class Parser
    {
        String[][] terms;
        //List<String> files;
        /*
        public Parser ( List<String> files)
        {
            this.files = files;
        }
        */

        public String[][] Parse(String[] files)
        {
            int num = files.Length; 
            terms = new string[num][];
            for(int i=0; i<num; i++)
            {
                string pattern = "\t";
                terms[i]=Regex.Split(files[i],pattern);
                /*
                for (int j = 0; j < terms[i].Length; j++)
                {
                    String input = terms[i][j];
                    if (System.Text.RegularExpressions.Regex.IsMatch()
                }
                */
            }
            return terms;
        }



    }
}
