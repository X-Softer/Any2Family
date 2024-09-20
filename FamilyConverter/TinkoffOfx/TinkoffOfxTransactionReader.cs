using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace FamilyConverter
{
    public class TinkoffOfxTransactionReader : FileTransactionReader
    {
        private readonly NumberFormatInfo _nfi = new NumberFormatInfo();

        public TinkoffOfxTransactionReader(string fileName)
            :base(fileName)
        {
            FileName = fileName;
            _nfi.CurrencyGroupSeparator = "";
            _nfi.CurrencyDecimalSeparator = ".";
            _nfi.NumberGroupSeparator = "";
            _nfi.NumberDecimalSeparator = ".";
            _nfi.CurrencyDecimalDigits = 4;
            _nfi.NumberDecimalDigits = 4;
        }

        public override IEnumerable<TransactionEntry> ReadTransactions(params string[] pars)
        {
            List<TransactionEntry> transList = new List<TransactionEntry>();

            string accountNumber = null;
            if (pars != null && pars.Length >= 1)
            {
                accountNumber = pars[0];
            }

            XDocument xd = XDocument.Load(FileName);

            var transLists = xd.Element("OFX")?.Element("BANKMSGSRSV1")?.Elements("STMTTRNRS");

            if (transLists != null)
            {
                foreach (var element in transLists)
                {
                    var accNumber = element.Element("STMTRS")?.Element("BANKACCTFROM")?.Element("ACCTID")?.Value;
                    if (accountNumber != null && accNumber != accountNumber)
                    {
                        continue;
                    }

                    var tranElements = element.Element("STMTRS")?.Element("BANKTRANLIST")?.Elements("STMTTRN");
                    if (tranElements == null)
                    {
                        continue;
                    }

                    int tId = 1;
                    foreach (var tranElement in tranElements)
                    {
                        TinkoffOfxTransactionEntry transEntry = new TinkoffOfxTransactionEntry()
                        {
                            Id = tId.ToString(), 
                            FitId = tranElement.Element("FITID")?.Value,
                            OperTime = DateTime.ParseExact(tranElement.Element("DTPOSTED")?.Value.Substring(0, 14), "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            AcceptTime = null,
                            Type = tranElement.Element("TRNTYPE")?.Value.ToUpper() == "DEBIT" ? TransactionEntryType.Expense : TransactionEntryType.Income,
                            OperAmount = Math.Abs(decimal.Parse(tranElement.Element("TRNAMT")?.Value, _nfi)),
                            OperCurrency = tranElement.Element("CURRENCY")?.Element("CURSYM")?.Value,
                            AcceptAmount = Math.Abs(decimal.Parse(tranElement.Element("TRNAMT")?.Value, _nfi)),
                            AcceptCurrency = tranElement.Element("CURRENCY")?.Element("CURSYM")?.Value,
                            Category = tranElement.Element("MEMO")?.Value,
                            OperLocation = tranElement.Element("NAME")?.Value
                        };

                        transList.Add(transEntry);
                        tId++;
                    }
                }
            }
            
            return transList;
        }
    }
}
