using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.Library;

namespace FireEngine.Test.CommonLibraryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TreeMap<string, string> map = new TreeMap<string, string>();


            foreach(KeyValuePair<string,string> pair in map)
            {
                Console.WriteLine("{0}, {1}", pair.Key, pair.Value);
            }
        }
    }
}
