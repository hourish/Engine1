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
        private string name;
        int max_tf;
        string date;
        ArrayList terms;
        public Document(string name)
        {
            this.name = name;
            max_tf = 0;
            terms = new ArrayList();
        }

        public int GetMaxTF()
        {
            return max_tf;
        }
        public int GetLength()
        {
            return terms.Count;
        }
        public void SetDate(string date)
        {
            this.date = date;
        }

        /// <summary>
        /// add term to the arraylist only if its new (unique)
        /// </summary>
        /// <param name="term"></param>
        public void AddTerm(Term term)
        {
            if (!terms.Contains(term))
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

        public bool ContainTerm(Term term)
        {
            return terms.Contains(term);
        }

        public void SetMaxTF()
        {
            int max = -1;
            for (int i = 0; i < terms.Count; i++)
            {
                int currentTF = ((Term)terms[i]).GetTF(this);
                if (currentTF > max)
                {
                    max = currentTF;
                }
            }
            max_tf = max;
        }

        public String GetName()
        {
            return name;
        }

    }
}