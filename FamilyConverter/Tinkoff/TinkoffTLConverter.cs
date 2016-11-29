using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public class TinkoffTLConverter : TLConverter
    {
        public TinkoffTLConverter(TLConverterSettings settings) : base("Tinkoff", settings)
        {
        }

        public override IEnumerable<FamilyTransactionEntry> Convert(IEnumerable<TransactionEntry> transaction_list)
        {
            List<FamilyTransactionEntry> FList = new List<FamilyTransactionEntry>();

            foreach(var transaction in transaction_list)
            {
                TinkoffTransactionEntry TTran = transaction as TinkoffTransactionEntry;
                FamilyTransactionEntry FTran = new FamilyTransactionEntry()
                {
                    Date = TTran.OperTime,
                    Sum = (TTran.Type == TransactionEntryType.Expense) ? (0 - TTran.OperAmount) : TTran.OperAmount,
                };

                // Just for debug
                FTran.Comment = TTran.OperLocation;

                MapFields(nameof(TTran.Category), TTran.Category, FTran);
                MapFields(nameof(TTran.OperLocation), TTran.OperLocation, FTran);
                MapFields(nameof(TTran.MCC), TTran.MCC, FTran);

                if(String.IsNullOrEmpty(FTran.Category))
                {
                    FTran.Category = Settings.DefaultCategory;
                }

                FList.Add(FTran);
            }

            return FList;
        }
    }
}
