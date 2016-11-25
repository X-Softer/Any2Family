using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FamilyConverter
{
    public abstract class TLConverter : ITLConverter
    {
        protected string name;
        protected TLConverterSettings Settings;

        public TLConverter(string name, TLConverterSettings settings)
        {
            this.name = name;
            this.Settings = settings ?? new TLConverterSettings();
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public abstract IEnumerable<FamilyTransactionEntry> Convert(IEnumerable<TransactionEntry> transaction_list);

        protected void MapFields(string field_name, string field_value, FamilyTransactionEntry fte)
        {
            IEnumerable<MappingEntry> RulesForField = Settings.MappingRules.Where(x => x.SourceName == Name)
                    .Where(x => (x.SourceEntryPropertyName == field_name && x.SourceEntryPropertyValue == field_value));

            foreach (var rule in RulesForField)
            {
                PropertyInfo pi = fte.GetType().GetProperty(rule.TargetEntryPropertyName);
                if (pi != null)
                {
                    pi.SetValue(fte, rule.TargetEntryPropertyValue);
                }
            }
        }
    }
}
