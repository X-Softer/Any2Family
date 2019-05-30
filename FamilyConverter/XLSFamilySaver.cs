using System.Collections.Generic;
using System.Linq;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace FamilyConverter
{
    public class XLSFamilySaver : FileFamilySaver
    {
        public XLSFamilySaver(string fileName) 
            : base(fileName)
        {
        }

        public override void SaveTransactions(IEnumerable<FamilyTransactionEntry> transactions)
        {
            System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo
            {
                CurrencyGroupSeparator = "",
                CurrencyDecimalSeparator = ",",
                NumberGroupSeparator = "",
                NumberDecimalSeparator = ",",
                CurrencyDecimalDigits = 2,
                NumberDecimalDigits = 2
            };

            using (FileStream stream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                IWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet("Данные из Family");
                //ICreationHelper cH = wb.GetCreationHelper();
                
                // Header
                IRow row = sheet.CreateRow(0);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue("Дата");
                cell = row.CreateCell(1);
                cell.SetCellValue("Комментарий");
                cell = row.CreateCell(2);
                cell.SetCellValue("Тэг");
                cell = row.CreateCell(3);
                cell.SetCellValue("Контрагент");
                cell = row.CreateCell(4);
                cell.SetCellValue("Категория");
                cell = row.CreateCell(5);
                cell.SetCellValue("Сумма");

                // Data
                int rowNum = 1;
                foreach (var transaction in transactions.OrderBy(x => x.Date))
                {
                    row = sheet.CreateRow(rowNum);

                    cell = row.CreateCell(0);
                    cell.SetCellValue(transaction.Date.ToString("dd.MM.yyyy"));
                    cell = row.CreateCell(1);
                    cell.SetCellValue(transaction.Comment);
                    cell = row.CreateCell(2);
                    cell.SetCellValue(transaction.Tag);
                    cell = row.CreateCell(3);
                    cell.SetCellValue(transaction.Contragent);
                    cell = row.CreateCell(4);
                    cell.SetCellValue(transaction.Category);
                    cell = row.CreateCell(5);
                    cell.SetCellValue(transaction.Sum.ToString("##################0.00", nfi));

                    rowNum++;
                }

                wb.Write(stream);
            }
        }
    }
}
