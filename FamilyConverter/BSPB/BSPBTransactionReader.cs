using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace FamilyConverter
{
    public class BSPBTransactionReader : FileTransactionReader
    {
        private readonly NumberFormatInfo _nfi = new NumberFormatInfo();

        public BSPBTransactionReader(string fileName)
            :base(fileName)
        {
            FileName = fileName;
            _nfi.CurrencyGroupSeparator = "";
            _nfi.CurrencyDecimalSeparator = ",";
            _nfi.NumberGroupSeparator = "";
            _nfi.NumberDecimalSeparator = ",";
            _nfi.CurrencyDecimalDigits = 4;
            _nfi.NumberDecimalDigits = 4;
        }

        public override IEnumerable<TransactionEntry> ReadTransactions()
        {
            List<TransactionEntry> transList = new List<TransactionEntry>();

            System.Collections.IEnumerator rows;

            try
            {
                IWorkbook inputWb;
                using (FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    var fileExt = Path.GetExtension(FileName);
                    if (fileExt == ".xls")
                    {
                        inputWb = new HSSFWorkbook(file);
                    }
                    else
                    {
                        inputWb = new XSSFWorkbook(file);
                    }
                }

                ISheet sheet = inputWb.GetSheetAt(0);
                rows = sheet.GetRowEnumerator();
            }
            catch (Exception ex)
            {
                throw new Exception("[BSPB DATAREADER] Ошибка открытия файла, возможно неверный формат\n" + ex);
            }

            int tId = 1;

            while (rows.MoveNext())
            {
                IRow row = (IRow)rows.Current;

                string acceptDatePattern = @"^\d{2}\.\d{2}\.\d{4}$";

                // Pass header and footer
                DateTime acceptDate;
                if (row.Cells.Count == 0 || (row.Cells[0].CellType != CellType.Numeric && !Regex.IsMatch(row.Cells[0].StringCellValue.Trim(), acceptDatePattern)))
                {
                    continue;
                }
                if (row.Cells[0].CellType == CellType.Numeric)
                {
                     acceptDate = row.Cells[0].DateCellValue;
                }
                else
                {
                    string acceptDateStr = Regex.Match(row.Cells[0].StringCellValue.Trim(), acceptDatePattern).Value;
                    acceptDate = DateTime.ParseExact(acceptDateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                }

                BSPBTransactionEntry transEntry = new BSPBTransactionEntry();
                transEntry.Id = tId.ToString();
                transEntry.AcceptTime = acceptDate;
                transEntry.Account = row.Cells[1].StringCellValue;
                transEntry.OperCurrency = transEntry.AcceptCurrency = "RUB";
                transEntry.OperLocation = row.Cells[2].StringCellValue;

                // Parse operation describe
                Match catMatch = Regex.Match(row.Cells[3].StringCellValue, @"([\w\W.]+) (\d{2}\.\d{2}\.\d{4} \d{2}:\d{2}) (.+) (\*\d{4})");
                if(catMatch.Groups.Count >= 4)
                {
                    transEntry.Category = catMatch.Groups[1].Value;
                    transEntry.OperTime = DateTime.ParseExact(catMatch.Groups[2].Value, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                    transEntry.CardMask = catMatch.Groups[4].Value;
                }
                else
                {
                    transEntry.Category = row.Cells[3].StringCellValue;
                    transEntry.OperTime = (DateTime)transEntry.AcceptTime;
                }

                decimal acceptAmount = Convert.ToDecimal(row.Cells[4].NumericCellValue);
                transEntry.AcceptAmount = Math.Abs(acceptAmount);
                transEntry.OperAmount = transEntry.AcceptAmount;
                transEntry.Type = acceptAmount < 0 ? TransactionEntryType.Expense : TransactionEntryType.Income;

                transList.Add(transEntry);
                tId++;
            }

            return transList;
        }
    }
}
