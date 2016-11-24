
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace FamilyConverter
{
    public class XLSFamilySaver : FileFamilySaver
    {
        public XLSFamilySaver(string file_name) 
            : base(file_name)
        {
        }

        public override void SaveTransactions(IEnumerable<FamilyTransactionEntry> transactions)
        {
            System.Globalization.NumberFormatInfo Nfi = new System.Globalization.NumberFormatInfo();
            Nfi.CurrencyGroupSeparator = "";
            Nfi.CurrencyDecimalSeparator = ",";
            Nfi.NumberGroupSeparator = "";
            Nfi.NumberDecimalSeparator = ",";
            Nfi.CurrencyDecimalDigits = 2;
            Nfi.NumberDecimalDigits = 2;

            using (FileStream stream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                IWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet("Данные из Family");
                ICreationHelper cH = wb.GetCreationHelper();
                
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
                int RowNum = 1;
                foreach (var transaction in transactions.OrderBy(x => x.Date))
                {
                    row = sheet.CreateRow(RowNum);

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
                    cell.SetCellValue(transaction.Sum.ToString("##################0.00", Nfi));

                    RowNum++;
                }

                wb.Write(stream);
            }
        }
    }
}
