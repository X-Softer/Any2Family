using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FamilyConverter
{
    public class TinkoffTransactionReader : FileTransactionReader
    {
        private readonly NumberFormatInfo _nfi = new NumberFormatInfo();

        public TinkoffTransactionReader(string fileName)
            :base(fileName)
        {
            FileName = fileName;
            _nfi.CurrencyGroupSeparator = "";
            _nfi.CurrencyDecimalSeparator = ",";
            _nfi.NumberGroupSeparator = "";
            _nfi.NumberDecimalSeparator = ",";
            _nfi.CurrencyDecimalDigits = 2;
            _nfi.NumberDecimalDigits = 2;
        }

        public override IEnumerable<TransactionEntry> ReadTransactions()
        {
            List<TransactionEntry> transList = new List<TransactionEntry>();

            using (CSVReader csvr = new CSVReader(FileName, Encoding.GetEncoding("windows-1251")))
            {
                int tId = 1;
                string[] str;
                while ((str = csvr.ReadRow()) != null)
                {
                    if(str.Length < 12)
                    {
                        throw new Exception("File Format Error, not enough colums!");
                    }

                    str = str.Select(DeQuote).Select(UnMaskQuotes).ToArray();

                    // Pass header
                    if(Regex.IsMatch(str[0], "Дата", RegexOptions.IgnoreCase))
                    {
                        continue;
                    }

                    string[] dateFormats = {"dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy"};

                    TinkoffTransactionEntry transEntry = new TinkoffTransactionEntry()
                    {
                        Id = tId.ToString(), 
                        OperTime = DateTime.ParseExact(str[0], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        AcceptTime = String.IsNullOrEmpty(str[1]) ? null : (DateTime?)DateTime.ParseExact(str[1], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None),
                        CardMask = str[2],
                        Type = (Decimal.Parse(str[4], _nfi) < 0) ? TransactionEntryType.Expense : TransactionEntryType.Income,
                        OperAmount = Math.Abs(Decimal.Parse(str[4], _nfi)),
                        OperCurrency = str[5],
                        AcceptAmount = Math.Abs(Decimal.Parse(str[6], _nfi)),
                        AcceptCurrency = str[7],
                        CashBackSum = String.IsNullOrEmpty(str[8]) ? 0 : Decimal.Parse(str[8], _nfi),
                        Category = str[9],
                        MCC = str[10],
                        OperLocation = str[11],
                        TotalBonusesSum = Decimal.Parse(str[12], _nfi)
                    };

                    transList.Add(transEntry);
                    tId++;
                }
            }

            return transList;
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
