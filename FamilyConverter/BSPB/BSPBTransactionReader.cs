using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace FamilyConverter
{
    public class BSPBTransactionReader : FileTransactionReader
    {
        private NumberFormatInfo Nfi = new NumberFormatInfo();

        public BSPBTransactionReader(string file_name)
            :base(file_name)
        {
            FileName = file_name;
            Nfi.CurrencyGroupSeparator = "";
            Nfi.CurrencyDecimalSeparator = ",";
            Nfi.NumberGroupSeparator = "";
            Nfi.NumberDecimalSeparator = ",";
            Nfi.CurrencyDecimalDigits = 4;
            Nfi.NumberDecimalDigits = 4;
        }

        public override IEnumerable<TransactionEntry> ReadTransactions()
        {
            List<TransactionEntry> TransList = new List<TransactionEntry>();

            IWorkbook InputWB;
            int RowTotal = 0;
            System.Collections.IEnumerator rows;

            try
            {
                using (FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    var fileExt = Path.GetExtension(FileName);
                    if (fileExt == ".xls")
                    {
                        InputWB = new HSSFWorkbook(file);
                    }
                    else
                    {
                        InputWB = new XSSFWorkbook(file);
                    }
                }

                ISheet sheet = InputWB.GetSheetAt(0);
                rows = sheet.GetRowEnumerator();
                RowTotal = sheet.LastRowNum;

            }
            catch (Exception ex)
            {
                throw new Exception("[BSPB DATAREADER] Ошибка открытия файла, возможно неверный формат\n" + ex);
            }

            int tId = 1;

            while (rows.MoveNext())
            {
                IRow row = (IRow)rows.Current;

                string AcceptDatePattern = @"^\d{2}\.\d{2}\.\d{4}$";

                // Pass header and footer
                if (row.Cells.Count == 0 || !Regex.IsMatch(row.Cells[0].StringCellValue.Trim(), AcceptDatePattern))
                {
                    continue;
                }

                string AcceptDateStr = Regex.Match(row.Cells[0].StringCellValue.Trim(), AcceptDatePattern).Value;

                BSPBTransactionEntry TransEntry = new BSPBTransactionEntry();
                TransEntry.Id = tId.ToString();
                TransEntry.AcceptTime = DateTime.ParseExact(AcceptDateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                TransEntry.Account = row.Cells[1].StringCellValue;
                TransEntry.OperCurrency = TransEntry.AcceptCurrency = "RUB";
                TransEntry.OperLocation = row.Cells[2].StringCellValue;

                // Parse operation describe
                Match CatMatch = Regex.Match(row.Cells[3].StringCellValue, @"(.+) (\d{2}.\d{2}.\d{4} \d{2}:\d{2}) (.+) (\*\d{4})");
                if(CatMatch.Groups.Count >= 4)
                {
                    TransEntry.Category = CatMatch.Groups[1].Value;
                    TransEntry.OperTime = DateTime.ParseExact(CatMatch.Groups[2].Value, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                    TransEntry.CardMask = CatMatch.Groups[4].Value;
                }
                else
                {
                    TransEntry.Category = row.Cells[3].StringCellValue;
                    TransEntry.OperTime = (DateTime)TransEntry.AcceptTime;
                }

                decimal AcceptAmount = Convert.ToDecimal(row.Cells[4].NumericCellValue);
                TransEntry.AcceptAmount = Math.Abs(AcceptAmount);
                TransEntry.OperAmount = TransEntry.AcceptAmount;
                TransEntry.Type = AcceptAmount < 0 ? TransactionEntryType.Expense : TransactionEntryType.Income;

                TransList.Add(TransEntry);
                tId++;
            }

            return TransList;
        }
    }
}
