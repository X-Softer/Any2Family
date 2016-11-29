using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public interface ITLConverter
    {
        string Name { get; }
        
        IEnumerable<FamilyTransactionEntry> Convert(IEnumerable<TransactionEntry> transaction_list);
    }
}
