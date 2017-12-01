using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Engine
{
    class Term
    {
        string name;
        Dictionary<Document, ArrayList> details; //df is the dictionary's length. tf is the arrayList's length.        
        public Term(string name)
        {
            this.name = name;
            details = new Dictionary<Document, ArrayList>();
        }

       // public addDoc(D)
    }
}
