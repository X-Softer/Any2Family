using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public interface IFamilyConverter
    {
        IEnumerable<TransactionEntry> Read(ITransactionReader reader);

    }
}
