using System;
using System.Text.RegularExpressions;
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
            IEnumerable<MappingEntry> RulesForField = Settings.MappingRules.Where(x => (x.SourceName == Name || x.SourceName == "*"))
                    .Where(x => (x.SourceEntryPropertyName == field_name && IsSuitable(field_value, x.SourceEntryPropertyValue)));

            foreach (var rule in RulesForField)
            {
                PropertyInfo pi = fte.GetType().GetProperty(rule.TargetEntryPropertyName);
                if (pi != null)
                {
                    pi.SetValue(fte, rule.TargetEntryPropertyValue);
                }
            }
        }

        private bool IsSuitable(string value, string pattern)
        {
            // If pattern is a regex
            if(pattern.IndexOf("r(") == 0)
            {
                string RegPattern = pattern.Substring(2, pattern.Length - 3);

                if (Regex.IsMatch(value, RegPattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }

                return false;
            }

            // If pattern is a string
            if(value == pattern)
            {
                return true;
            }

            return false;
        }
    }
}
