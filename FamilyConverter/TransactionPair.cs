using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public class TransactionPair
    {
        public TransactionEntry SourceTransaction { get; set; }
        public FamilyTransactionEntry TargetTransaction { get; set; }
    }
}
