using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConverter
{
    public class MappingEntry
    {
        public string SourceName { get; set; }
        public string SourceEntryPropertyName { get; set; }
        public string SourceEntryPropertyValue { get; set; }
        public string TargetEntryPropertyName { get; set; }
        public string TargetEntryPropertyValue { get; set; }
    }
}
