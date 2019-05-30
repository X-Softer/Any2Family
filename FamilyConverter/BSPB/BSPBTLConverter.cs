using System;
using System.Collections.Generic;

namespace FamilyConverter
{
    public class BSPBTLConverter : TLConverter
    {
        public BSPBTLConverter(TLConverterSettings settings) : base("BSPB", settings)
        {
        }

        public override IEnumerable<FamilyTransactionEntry> Convert(IEnumerable<TransactionEntry> transactionList)
        {
            List<FamilyTransactionEntry> fList = new List<FamilyTransactionEntry>();

            foreach(var transaction in transactionList)
            {
                BSPBTransactionEntry tran = (BSPBTransactionEntry)transaction;
                FamilyTransactionEntry fTran = new FamilyTransactionEntry
                {
                    Id = tran.Id,
                    Date = tran.OperTime,
                    Sum = (tran.Type == TransactionEntryType.Expense) ? (0 - tran.OperAmount) : tran.OperAmount,
                };

                // Just for debug
                fTran.Comment = tran.OperLocation;

                MapFields(nameof(tran.Category), tran.Category, fTran);
                MapFields(nameof(tran.OperLocation), tran.OperLocation, fTran);

                if (String.IsNullOrEmpty(fTran.Category))
                {
                    fTran.Category = Settings.DefaultCategory;
                    //FTran.Comment = TTran.OperLocation;
                }

                fList.Add(fTran);
            }

            return fList;
        }
    }
}
