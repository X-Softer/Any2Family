using System;
using FamilyConverter;

namespace Any2FamilyConsole
{
    public static class TransactionReadersFactory
    {
        public static ITransactionReader GetTransactionReader(int typeId, Bindings bindings, string fn)
        {
            Type t = bindings.GetBindingForType(typeId).TransactionReaderType;
            return (ITransactionReader)Activator.CreateInstance(t, fn);
        }
    }
}
