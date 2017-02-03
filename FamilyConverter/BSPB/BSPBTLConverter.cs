using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public class BSPBTLConverter : TLConverter
    {
        public BSPBTLConverter(TLConverterSettings settings) : base("BSPB", settings)
        {
        }

        public override IEnumerable<FamilyTransactionEntry> Convert(IEnumerable<TransactionEntry> transaction_list)
        {
            List<FamilyTransactionEntry> FList = new List<FamilyTransactionEntry>();

            foreach(var transaction in transaction_list)
            {
                BSPBTransactionEntry TTran = transaction as BSPBTransactionEntry;
                FamilyTransactionEntry FTran = new FamilyTransactionEntry()
                {
                    Id = TTran.Id,
                    Date = TTran.OperTime,
                    Sum = (TTran.Type == TransactionEntryType.Expense) ? (0 - TTran.OperAmount) : TTran.OperAmount,
                };

                // Just for debug
                FTran.Comment = TTran.OperLocation;

                MapFields(nameof(TTran.Category), TTran.Category, FTran);
                MapFields(nameof(TTran.OperLocation), TTran.OperLocation, FTran);

                if (String.IsNullOrEmpty(FTran.Category))
                {
                    FTran.Category = Settings.DefaultCategory;
                    //FTran.Comment = TTran.OperLocation;
                }

                FList.Add(FTran);
            }

            return FList;
        }
    }
}
