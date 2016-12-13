using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public enum TransactionEntryType { Expense = 1, Income = 2 };

    public class TransactionEntry
    {
        public string Id { get; set; }
        public TransactionEntryType Type { get; set; }
        public DateTime OperTime { get; set; }
        public DateTime? AcceptTime { get; set; }
        public string Category { get; set; }
        public decimal OperAmount { get; set; }
        public decimal AcceptAmount { get; set; }
        public string OperCurrency { get; set; }
        public string AcceptCurrency { get; set; }
        public string OperLocation { get; set; }
    }
}
