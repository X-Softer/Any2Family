using System.Collections.Generic;

namespace FamilyConverter
{
    public interface IFamilyConverter
    {
        IEnumerable<TransactionEntry> Read(ITransactionReader reader);

    }
}
