using System.Collections.Generic;

namespace FamilyConverter
{
    public class TLConverterSettings
    {
        public List<MappingEntry> MappingRules { get;  set; }
        public string DefaultCategory { get; set; }

        public TLConverterSettings()
        {
            MappingRules = new List<MappingEntry>();
        }
    }
}
