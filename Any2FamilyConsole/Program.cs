using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;

using FamilyConverter;

namespace Any2FamilyConsole
{
    class Program
    {
        const string SettingsFileName = "Settings.xml"; // Settings filename
        static ProgramSettings Settings;

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

                int ConvType = Convert.ToInt32(args[0]);

                // Load settings
                Settings = ProgramSettings.Create(SettingsFileName);
                
                // Choose reader and converter
                ITransactionReader TransReader;
                ITLConverter TransListConverter;
                switch (ConvType)
                {
                    case 1:
                        {
                            TransReader = new TinkoffTransactionReader(fn);
                            TransListConverter = new TinkoffTLConverter(Settings.TLSettings);
                            break;
                        }
                    default:
                        {
                            throw new Exception("Неизвестный тип конвертера данных");
                        }
                }

                string out_fn = $"{TransListConverter.Name}_converted_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xls";
                if (args.Length >= 3)
                {
                    out_fn = args[2];
                }

                // Load transactions
                Console.WriteLine($"Загружаем транзакции из файла: {fn} ...");
                IEnumerable<TransactionEntry> ReadedTransactions = TransReader.ReadTransactions();
                Console.WriteLine($"Загружено {ReadedTransactions.Count()} транзакций");

                // Convert transactions
                Console.WriteLine($"Конвертируем транзакции ...");
                IEnumerable<FamilyTransactionEntry> FamilyTransactions = TransListConverter.Convert(ReadedTransactions);
                Console.WriteLine($"Сконвертировано {FamilyTransactions.Count()} транзакций");

                // Analyze transactions
                AnalizeTransactionsList(FamilyTransactions);

                // Save transaction to Family11 format
                IFamilySaver fs = new XLSFamilySaver(out_fn);
                Console.WriteLine($"Сохраняем транзакции в файл: {out_fn} ...");
                fs.SaveTransactions(FamilyTransactions);
                Console.WriteLine("Файл сохранен");

                // Save settings to file
                //SaveSettings(SettingsFileName);
                Settings.Save();

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
