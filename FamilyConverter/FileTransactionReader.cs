using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public abstract class FileTransactionReader : ITransactionReader
    {
        protected string FileName;

        public FileTransactionReader(string file_name)
        {
            FileName = file_name;
        }

        public abstract IEnumerable<TransactionEntry> ReadTransactions();
    }
}
