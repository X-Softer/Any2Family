using System.Collections.Generic;

namespace FamilyConverter
{
    public abstract class FileFamilySaver : IFamilySaver
    {
        protected string FileName;

        protected FileFamilySaver(string fileName)
        {
            FileName = fileName;
        }

        public string Name => "AbstractFileSaver";

        public abstract void SaveTransactions(IEnumerable<FamilyTransactionEntry> transactions);
    }
}
