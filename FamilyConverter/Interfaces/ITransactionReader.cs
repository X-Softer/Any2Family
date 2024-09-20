using System.Collections.Generic;
using NPOI.OpenXmlFormats.Dml;

namespace FamilyConverter
{
    public interface ITransactionReader
    {
        IEnumerable<TransactionEntry> ReadTransactions(params string[] parameters);
    }
}
