using System;

namespace FamilyConverter
{
    public class FamilyTransactionEntry
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public string Tag { get; set; }
        public string Contragent { get; set; }
        public string Category { get; set; }
        public decimal Sum { get; set; }
    }
}
