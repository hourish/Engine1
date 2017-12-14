 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Engine
    {
        class Term : IComparable<Term>
        {
            string name;
            Dictionary<Document, ArrayList> details; //df is the dictionary's length. tf is the arrayList's length.        
            public Term(string name)
            {
                this.name = name;
                details = new Dictionary<Document, ArrayList>();
            }

            public string StringToPosting(Document currentDoc)
            {
            // string res = name+"|"+currentDoc.GetName() + ':';
            string res = currentDoc.GetName() + ':';
            if (details.ContainsKey(currentDoc))
                {
                ArrayList positions = details[currentDoc];//all the positions in the document
                for (int j = 0; j < positions.Count; j++)
                {
                    if (j != positions.Count - 1)
                        res += positions[j] + ",";
                    else
                        res += positions[j] + "?";
                }
            }
            else
                res = "";
            return res;
        }

            /// <summary>
            /// update the details dictionary of a term.
            /// </summary>
            /// <param name="currentDoc"></param>
            /// <param name="position"></param> the position of the term in currentDoc
            public void UpdateDetails(Document currentDoc, int position)
            {
                if (!details.ContainsKey(currentDoc))
                {
                    details.Add(currentDoc, new ArrayList());
                }
                details[currentDoc].Add(position);
            }

            /// <summary>
            /// determinide a term hash code according to his name
            /// </summary>
            /// <returns>integer, hash code of a term</returns>
            public override int GetHashCode()
            {
                return name.GetHashCode();
            }
            /// <summary>
            /// compare to terms according to there name 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns>true- if two terms have the same name 
            /// false- if two terms dosent have the same name</returns>
            public override bool Equals(object obj)
            {
                return name.Equals(((Term)obj).name);
            }
            public int GetTF(Document currentDoc)
            {
                return details[currentDoc].Count;
            }
            public int GetDF()
            {
                return details.Count;
            }
            public List<Document> GetDocsList()
            {
                return details.Keys.ToList();
            }
            public string GetName()
            {
                return name;
            }
            public int CompareTo(Term t)
            {
                return (name.CompareTo(t.GetName()));
            }
        }
    }       