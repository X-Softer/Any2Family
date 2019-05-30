using System.Collections.Generic;

namespace FamilyConverter
{
    public interface ITLConverter
    {
        string Name { get; }
        
        IEnumerable<FamilyTransactionEntry> Convert(IEnumerable<TransactionEntry> transactionList);
    }
}
