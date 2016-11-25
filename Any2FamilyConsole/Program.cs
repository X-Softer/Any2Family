using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

using FamilyConverter;

namespace Any2FamilyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("\nUsage: any2familycon <type> <filename> [output filename]");
                Console.WriteLine("\nTypes: 1 -  Tinkoff Bank CSV-file");
                Console.WriteLine("       2 -  Bank SPB CSV-file");
                Console.WriteLine("       3 -  PSCB CSV-file");
                return;
            }

            string fn = args[0];
            string out_fn = $"converted_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls";
            if (args.Length >= 3)
            {
                out_fn = args[2];
            }

            int ConvType = Convert.ToInt32(args[1]);

            TLConverterSettings TLSettings = LoadSettings("MappingRules.txt");

            ITransactionReader TransReader;
            ITLConverter TransListConverter;
            switch (ConvType)
            {
                case 1:
                    {
                        TransReader = new TinkoffTransactionReader(fn);
                        TransListConverter = new TinkoffTLConverter(TLSettings);
                        break;
                    }
                default:
                    {
                        throw new Exception("Неизвестный тип конвертера данных");
                    }
            }

            Console.WriteLine($"Загружаем транзакции из файла: {fn} ...");
            IEnumerable<TransactionEntry> ReadedTransactions = TransReader.ReadTransactions();
            Console.WriteLine($"Загружено {ReadedTransactions.Count()} транзакций");

            Console.WriteLine($"Конвертируем транзакции ...");
            IEnumerable<FamilyTransactionEntry> FamilyTransactions = TransListConverter.Convert(ReadedTransactions);
            Console.WriteLine($"Сконвертировано {FamilyTransactions.Count()} транзакций");

            IFamilySaver fs = new XLSFamilySaver(out_fn);
            Console.WriteLine($"Сохраняем транзакции в файл: {out_fn} ...");
            fs.SaveTransactions(FamilyTransactions);
            Console.WriteLine("Файл сохранен");

            Console.WriteLine("\nPress any key ...");
            Console.ReadKey();
        }

        static TLConverterSettings LoadSettings(string fn)
        {
            TLConverterSettings setts = new TLConverterSettings();

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

                    setts.MappingRules.Add(me);
                }
            }

            return setts;
        }
    }
}
