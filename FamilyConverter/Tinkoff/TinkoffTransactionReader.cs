using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public class TinkoffTransactionReader : FileTransactionReader
    {
        private NumberFormatInfo Nfi = new NumberFormatInfo();

        public TinkoffTransactionReader(string file_name)
            :base(file_name)
        {
            FileName = file_name;
            Nfi.CurrencyGroupSeparator = "";
            Nfi.CurrencyDecimalSeparator = ",";
            Nfi.NumberGroupSeparator = "";
            Nfi.NumberDecimalSeparator = ",";
            Nfi.CurrencyDecimalDigits = 2;
            Nfi.NumberDecimalDigits = 2;
        }

        public override IEnumerable<TransactionEntry> ReadTransactions()
        {
            List<TransactionEntry> TransList = new List<TransactionEntry>();

            using (SCVReader csvr = new SCVReader(FileName, Encoding.GetEncoding("windows-1251")))
            {
                int tId = 1;
                string[] str;
                while ((str = csvr.ReadRow()) != null)
                {
                    if(str.Length < 12)
                    {
                        throw new Exception("File Format Error, not enough colums!");
                    }

                    str = str.Select(x => DeQuote(x)).Select(x => UnMaskQuotes(x)).ToArray();

                    // Pass header
                    if(Regex.IsMatch(str[0], "Дата", RegexOptions.IgnoreCase))
                    {
                        continue;
                    }

                    string[] dateFormats = {"dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy"};

                    TinkoffTransactionEntry TransEntry = new TinkoffTransactionEntry()
                    {
                        Id = tId.ToString(), 
                        OperTime = DateTime.ParseExact(str[0], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        AcceptTime = String.IsNullOrEmpty(str[1]) ? null : (DateTime?)DateTime.ParseExact(str[1], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None),
                        CardMask = str[2],
                        Type = (Decimal.Parse(str[4], Nfi) < 0) ? TransactionEntryType.Expense : TransactionEntryType.Income,
                        OperAmount = Math.Abs(Decimal.Parse(str[4], Nfi)),
                        OperCurrency = str[5],
                        AcceptAmount = Math.Abs(Decimal.Parse(str[6], Nfi)),
                        AcceptCurrency = str[7],
                        CashBackSum = String.IsNullOrEmpty(str[8]) ? 0 : Decimal.Parse(str[8], Nfi),
                        Category = str[9],
                        MCC = str[10],
                        OperLocation = str[11],
                        TotalBonusesSum = Decimal.Parse(str[12], Nfi)
                    };

                    TransList.Add(TransEntry);
                    tId++;
                }
            }

            return TransList;
        }

        private string DeQuote(string str)
        {
            str = Regex.Replace(str, "^\"", "");
            str = Regex.Replace(str, "\"$", "");

            return str;
        }

        private string UnMaskQuotes(string str)
        {
            return Regex.Replace(str, "(\"\")", "\"");
        }
    }
}
