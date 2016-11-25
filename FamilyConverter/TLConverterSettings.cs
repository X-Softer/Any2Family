using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
