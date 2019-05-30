using System.Collections.Generic;

namespace FamilyConverter
{
    public abstract class FileTransactionReader : ITransactionReader
    {
        protected string FileName;

        protected FileTransactionReader(string fileName)
        {
            FileName = fileName;
        }

        public abstract IEnumerable<TransactionEntry> ReadTransactions();
    }
}
