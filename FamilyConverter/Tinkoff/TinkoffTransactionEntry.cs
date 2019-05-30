using System;

namespace FamilyConverter
{
    public class TinkoffTransactionEntry: TransactionEntry
    {
        public string CardMask { get; set; }
        public string MCC { get; set; }
        public decimal CashBackSum { get; set; }
        public decimal TotalBonusesSum { get; set; }

        public override string ToString()
        {

            string s = String.Empty;

            foreach (var property in typeof(TinkoffTransactionEntry).GetProperties())
            {
                s += $"{property.Name}={property.GetValue(this)}; ";
            }

            return s;

        }
    }
}
