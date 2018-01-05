using System.Collections.Generic;
using System.IO;

namespace Engine
{
    interface IRanker
    {
        double CosSin();
        Dictionary<string, double> Rank(bool stm, Dictionary<string, string[]> words, string path);
        string ReadLine(this BinaryReader reader);
    }
}