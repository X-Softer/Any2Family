using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FamilyConverter
{
    public abstract class TLConverter : ITLConverter
    {
        protected TLConverterSettings Settings;
        
        public string Name { get; }

        protected TLConverter(string name, TLConverterSettings settings)
        {
            Name = name;
            Settings = settings ?? new TLConverterSettings();
        }

        public abstract IEnumerable<FamilyTransactionEntry> Convert(IEnumerable<TransactionEntry> transactionList);

        protected void MapFields(string fieldName, string fieldValue, FamilyTransactionEntry fte)
        {
            IEnumerable<MappingEntry> rulesForField = Settings.MappingRules.Where(x => x.SourceName == Name || x.SourceName == "*")
                    .Where(x => x.SourceEntryPropertyName == fieldName && IsSuitable(fieldValue, x.SourceEntryPropertyValue));

            foreach (var rule in rulesForField)
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
                string regPattern = pattern.Substring(2, pattern.Length - 3);

                return Regex.IsMatch(value, regPattern, RegexOptions.IgnoreCase);
            }

            // If pattern is a string
            return value == pattern;
        }
    }
}
