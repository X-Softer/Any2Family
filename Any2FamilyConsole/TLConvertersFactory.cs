using System;
using FamilyConverter;

namespace Any2FamilyConsole
{
    public static class TLConvertersFactory
    {
        public static ITLConverter GetTLConverter(int typeId, Bindings bindings, TLConverterSettings tlSettings)
        {
            Type t = bindings.GetBindingForType(typeId).TLConverterType;
            return (ITLConverter)Activator.CreateInstance(t, tlSettings);
        }
    }
}
