using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FamilyConverter
{
    public abstract class BaseTLConverter : ITLConverter
    {
        protected string name;
        protected IEnumerable<MappingEntry> MappingRules;
 
        public BaseTLConverter(string name, IEnumerable<MappingEntry> mapping_rules)
        {
            this.name = name;
            this.MappingRules = mapping_rules;
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
            IEnumerable<MappingEntry> RulesForField = MappingRules.Where(x => x.SourceName == Name)
                    .Where(x => (x.SourceEntryPropertyName == field_name && x.SourceEntryPropertyValue == field_value));

            foreach(var rule in RulesForField)
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
