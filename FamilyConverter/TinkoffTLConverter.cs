﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public class TinkoffTLConverter : BaseTLConverter
    {
        public TinkoffTLConverter(IEnumerable<MappingEntry> mapping_rules) : base("Tinkoff", mapping_rules)
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

                MapFields(nameof(TTran.Category), TTran.OperLocation, FTran);
                MapFields(nameof(TTran.OperLocation), TTran.OperLocation, FTran);
                MapFields(nameof(TTran.MCC), TTran.OperLocation, FTran);

                FList.Add(FTran);
            }

            return FList;
        }
    }
}
