using System.Collections.Generic;

namespace FamilyConverter
{
    public interface ITransactionReader
    {
        IEnumerable<TransactionEntry> ReadTransactions();
    }
}
