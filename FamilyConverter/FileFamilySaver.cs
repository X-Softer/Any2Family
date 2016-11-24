using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public abstract class FileFamilySaver : IFamilySaver
    {
        protected string FileName;

        public FileFamilySaver(string file_name)
        {
            FileName = file_name;
        }

        public string Name
        {
            get
            {
                return "AbstractFileSaver";   
            }
        }

        public abstract void SaveTransactions(IEnumerable<FamilyTransactionEntry> transactions);
    }
}
