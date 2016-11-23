using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public class FamilyTransactionEntry
    {
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public string Tag { get; set; }
        public string Contragent { get; set; }
        public string Category { get; set; }
        public decimal Sum { get; set; }
    }
}
