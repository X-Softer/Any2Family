using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public interface IFamilySaver
    {
        string Name { get; }
        void SaveTransactions(IEnumerable<FamilyTransactionEntry> transactions);
    }
}
