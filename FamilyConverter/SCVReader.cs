using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FamilyConverter
{
    public class SCVReader: StreamReader
    {
        private char Delimiter = ';';
        private string RowPattern;
        private string SplitterPattern;

        public SCVReader(string file_name)
            :base(file_name)
        {
            SetRowPattern();
        }

        public SCVReader(string file_name, Encoding encoding)
            : base(file_name, encoding)
        {
            SetRowPattern();
        }

        public SCVReader(string file_name, Encoding encoding, char delimiter)
            : this(file_name, encoding)
        {
            Delimiter = delimiter;
            SetRowPattern();
        }

        private void SetRowPattern()
        {
            RowPattern = $"(?:^|{Delimiter})(?=[^\"]|(\")?)\"?((?(1)[^\"]*|[^{Delimiter}\"]*))\"?(?={Delimiter}|$)";
            SplitterPattern = $"{Delimiter}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
        }

        public string[] ReadRow()
        {
            //List<string> res = new List<string>();
            string[] res = null;
            string str;
            if ((str = ReadLine()) != null)
            {
                if (!String.IsNullOrEmpty(str))
                {
                    //res = str.Split(Delimiter);
                    Regex Parser = new Regex(SplitterPattern);
                    res = Parser.Split(str);
                }
            }

            return res;
        }
    }
}
