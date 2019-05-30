using System;
using System.Collections.Generic;
using System.Linq;
using FamilyConverter;

namespace Any2FamilyConsole
{
    class Program
    {
        private const string SettingsFileName = "Settings.xml"; // Settings filename
        private static ProgramSettings _settings;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("\nUsage: any2familycon <type> <filename> [output filename]");
                    Console.WriteLine("\nTypes: 1 -  Tinkoff Bank CSV-file");
                    Console.WriteLine("       2 -  Bank SPB CSV-file");
                    Console.WriteLine("       3 -  PSCB CSV-file");
                    return;
                }
                
                string fn = args[1];

                int convType = Convert.ToInt32(args[0]);

                // Load settings
                _settings = ProgramSettings.Create(SettingsFileName);
                
                // Choose reader and converter
                ITransactionReader transReader;
                ITLConverter transListConverter;
                switch (convType)
                {
                    case 1:
                        {
                            transReader = new TinkoffTransactionReader(fn);
                            transListConverter = new TinkoffTLConverter(_settings.TLSettings);
                            break;
                        }
                    case 2:
                        {
                            transReader = new BSPBTransactionReader(fn);
                            transListConverter = new BSPBTLConverter(_settings.TLSettings);
                            break;
                        }
                    default:
                        {
                            throw new Exception("Неизвестный тип конвертера данных");
                        }
                }

                string outFn = $"{transListConverter.Name}_converted_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                if (args.Length >= 3)
                {
                    outFn = args[2];
                }

                // Load transactions
                Console.WriteLine($"Загружаем транзакции из файла: {fn} ...");
                var readedTransactions = transReader.ReadTransactions().ToList();
                Console.WriteLine($"Загружено {readedTransactions.Count()} транзакций");

                // Convert transactions
                Console.WriteLine($"Конвертируем транзакции ...");
                var familyTransactions = transListConverter.Convert(readedTransactions).ToList();
                Console.WriteLine($"Сконвертировано {familyTransactions.Count()} транзакций");

                // Analyze transactions
                AnalizeTransactionsList(familyTransactions);

                // Save transaction to Family11 format
                IFamilySaver fs = new XLSFamilySaver(outFn);
                Console.WriteLine($"Сохраняем транзакции в файл: {outFn} ...");
                fs.SaveTransactions(familyTransactions);
                Console.WriteLine("Файл сохранен");

                // Save settings to file
                _settings.Save();

                Console.WriteLine("\nPress any key ...");
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Системная ошибка: " + ex);
                Console.WriteLine("\nPress any key ...");
                Console.ReadKey();
            }
        }

        private static void AnalizeTransactionsList(IEnumerable<FamilyTransactionEntry> familyTransactions)
        {
            //foreach (var trans in familyTransactions)
            //{
            //    if(String.IsNullOrEmpty(trans.Category))
            //    {
            //        Console.WriteLine($"--- Для транзакции {trans.}")
            //    }
            //}
        }
    }
}
