using System;

namespace FamilyConverter
{
    public class BSPBTransactionEntry: TransactionEntry
    {
        public string CardMask { get; set; }
        public string Account { get; set; }

        public override string ToString()
        {

            string s = String.Empty;

            foreach (var property in typeof(BSPBTransactionEntry).GetProperties())
            {
                s += $"{property.Name}={property.GetValue(this)}; ";
            }

            return s;

        }
    }
}
