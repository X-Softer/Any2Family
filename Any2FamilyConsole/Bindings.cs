using System;
using System.Collections.Generic;
using FamilyConverter;

namespace Any2FamilyConsole
{
    public struct Binding
    {
        public Type TransactionReaderType;
        public Type TLConverterType;
    }

    public class Bindings
    {
        private static Bindings _instance;
        private readonly Dictionary<int, Binding> _bindings;

        private Bindings()
        {
            _bindings = new Dictionary<int, Binding>();
        }

        public static Bindings Instance => _instance ?? (_instance = new Bindings());

        public void Bind(int type, Type transactionReaderType, Type tlConverterType)
        {
            if (!typeof(ITransactionReader).IsAssignableFrom(transactionReaderType))
            {
                throw new ArgumentException("Parameter transactionReaderType should be subclass of ITransactionReader");
            }

            if (!typeof(ITLConverter).IsAssignableFrom(tlConverterType))
            {
                throw new ArgumentException("Parameter tlConverterType should be subclass of ITTLConverter");
            }

            Binding binding = new Binding
            {
                TransactionReaderType = transactionReaderType,
                TLConverterType = tlConverterType
            };
            
            _bindings[type] = binding;
        }

        public Binding GetBindingForType(int type)
        {
            if (_bindings.ContainsKey(type))
            {
                return _bindings[type];
            }

            throw new Exception($"Bindings for type \"{type}\" not found");
        }
    }
}
