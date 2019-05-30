using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FamilyConverter
{
    public class CSVReader: StreamReader
    {
        private readonly char _delimiter = ';';
        private string _rowPattern;
        private string _splitterPattern;

        public CSVReader(string fileName)
            :base(fileName)
        {
            SetRowPattern();
        }

        public CSVReader(string fileName, Encoding encoding)
            : base(fileName, encoding)
        {
            SetRowPattern();
        }

        public CSVReader(string fileName, Encoding encoding, char delimiter)
            : this(fileName, encoding)
        {
            _delimiter = delimiter;
            SetRowPattern();
        }

        private void SetRowPattern()
        {
            _rowPattern = $"(?:^|{_delimiter})(?=[^\"]|(\")?)\"?((?(1)[^\"]*|[^{_delimiter}\"]*))\"?(?={_delimiter}|$)";
            _splitterPattern = $"{_delimiter}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
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
                    Regex parser = new Regex(_splitterPattern);
                    res = parser.Split(str);
                }
            }

            return res;
        }
    }
}
