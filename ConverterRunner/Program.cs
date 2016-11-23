using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

using FamilyConverter;

namespace ConverterRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string fn = @"d:\Downloads\operations Fri Oct 21 14_55_30 MSK 2016-Sun Nov 20 13_40_48 MSK 2016.csv";

            TinkoffTransactionReader tr = new TinkoffTransactionReader(fn);
            IEnumerable<TransactionEntry> ts = tr.ReadTransactions();

            IEnumerable<MappingEntry> MappingRules = LoadMappingRules("MappingRules.txt");

            TinkoffTLConverter tc = new TinkoffTLConverter(MappingRules);
            IEnumerable<FamilyTransactionEntry> fts = tc.Convert(ts);
            

            Console.ReadKey();
        }

        static IEnumerable<MappingEntry> LoadMappingRules(string fn)
        {
            List<MappingEntry> mr_list = new List<MappingEntry>();

            using (StreamReader sr = new StreamReader(File.OpenRead(fn)))
            {
                string str;
                while((str = sr.ReadLine()) != null)
                {
                    if(String.IsNullOrEmpty(str))
                    {
                        continue;
                    }

                    Match mc = Regex.Match(str, @"({.+})({.+})({.+})=({.+})({.+})");
                    if(mc.Groups.Count < 6)
                    {
                        throw new Exception("Invalid file format");
                    }

                    MappingEntry me = new MappingEntry()
                    {
                        SourceName = mc.Groups[1].Value.Trim('{', '}'),
                        SourceEntryPropertyName = mc.Groups[2].Value.Trim('{', '}'),
                        SourceEntryPropertyValue = mc.Groups[3].Value.Trim('{', '}'),
                        TargetEntryPropertyName = mc.Groups[4].Value.Trim('{', '}'),
                        TargetEntryPropertyValue = mc.Groups[5].Value.Trim('{', '}')
                    };

                    mr_list.Add(me);
                }
            }

            return mr_list;
        }
    }
}
