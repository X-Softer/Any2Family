using System.Collections.Generic;

namespace FamilyConverter
{
    public interface IFamilySaver
    {
        string Name { get; }
        void SaveTransactions(IEnumerable<FamilyTransactionEntry> transactions);
    }
}
