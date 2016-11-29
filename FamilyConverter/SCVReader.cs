using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public class SCVReader: StreamReader
    {
        private char Delimiter = ';';

        public SCVReader(string file_name)
            :base(file_name)
        {
        }

        public SCVReader(string file_name, Encoding encoding)
            : base(file_name, encoding)
        {
        }

        public SCVReader(string file_name, Encoding encoding, char delimiter)
            : this(file_name, encoding)
        {
            Delimiter = delimiter;
        }

        public string[] ReadRow()
        {
            string[] res = null;
            string str;
            if ((str = ReadLine()) != null)
            {
                res = str.Split(Delimiter);
            }

            return res;
        }
    }
}
