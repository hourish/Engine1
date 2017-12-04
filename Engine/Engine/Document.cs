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
        string date;
        ArrayList terms;
        public Document(string name)
        {
            this.name = name;
            max_tf = 0;
            length = 0;
           
            terms = new ArrayList();
        }

        public void SetMax_tf(int max_tf)
        {
            this.max_tf = max_tf;
        }

        public void SetDate(string date)
        {
            this.date = date;
        }

        public void SetLength(int length)
        {
            this.length = length;
        }
        
        /// <summary>
        /// add term to the arraylist only if its new (unique)
        /// </summary>
        /// <param name="term"></param>
        public void addTerm(Term term)
        {
            if(!terms.Contains(term))
            {
                terms.Add(term);
            }
        }

        /// <summary>
        /// compare to terms according to there name 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true- if two terms have the same name 
        /// false- if two terms dosent have the same name</returns>
        public override bool Equals(object obj)
        {
            return name.Equals(((Document)obj).name);
        }
    }
}
