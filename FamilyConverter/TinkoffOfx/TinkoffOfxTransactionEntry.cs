using System;

namespace FamilyConverter
{
    public enum OfxTransationType { Debit, Credit }

    public class TinkoffOfxTransactionEntry: TransactionEntry
    {
        public string FitId { get; set; }

        public override string ToString()
        {

            string s = String.Empty;

            foreach (var property in typeof(TinkoffOfxTransactionEntry).GetProperties())
            {
                s += $"{property.Name}={property.GetValue(this)}; ";
            }

            return s;

        }
    }
}
