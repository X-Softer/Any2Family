using System;
using System.Collections.Generic;

namespace FamilyConverter
{
    public class TinkoffTLConverter : TLConverter
    {
        public TinkoffTLConverter(TLConverterSettings settings) : base("Tinkoff", settings)
        {
        }

        public override IEnumerable<FamilyTransactionEntry> Convert(IEnumerable<TransactionEntry> transactionList)
        {
            List<FamilyTransactionEntry> fList = new List<FamilyTransactionEntry>();

            foreach(var transaction in transactionList)
            {
                TinkoffTransactionEntry tran = (TinkoffTransactionEntry)transaction;
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
                MapFields(nameof(tran.MCC), tran.MCC, fTran);

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
