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
                    Console.WriteLine("\nUsage: any2familycon <type> <filename> [params]");
                    Console.WriteLine("\nTypes: 1 -  Tinkoff Bank CSV-file");
                    Console.WriteLine("       2 -  Bank SPB CSV-file");
                    Console.WriteLine("       11 -  Tinkoff Bank OFX-file");
                    return;
                }
                
                string fn = args[1];

                int convType = Convert.ToInt32(args[0]);

                // Load settings
                _settings = ProgramSettings.Create(SettingsFileName);
                
                // Choose reader and converter
                ITransactionReader transReader = TransactionReadersFactory.GetTransactionReader(convType, _settings.Bindings, fn);
                ITLConverter transListConverter = TLConvertersFactory.GetTLConverter(convType, _settings.Bindings, _settings.TLSettings);

                string outFn = $"{transListConverter.Name}_converted_{DateTime.Now:yyyyMMdd_HHmmss}.xls";

                string[] addParams = null;
                if (args.Length >= 3)
                {
                    addParams = new string[args.Length - 2];
                    int j = 0;
                    for (int i = 2; i < args.Length; i++)
                    {
                        addParams[j++] = args[i];
                    }
                }
                
                // Load transactions
                Console.WriteLine($"Loading transactions from file: {fn}...");
                var readedTransactions = transReader.ReadTransactions(addParams).ToList();
                Console.WriteLine($"{readedTransactions.Count} transactions loaded");

                // Convert transactions
                Console.WriteLine($"Converting transactions...");
                var familyTransactions = transListConverter.Convert(readedTransactions).ToList();
                Console.WriteLine($"{familyTransactions.Count} transactions converted");

                // Analyze transactions
                AnalizeTransactionsList(familyTransactions);

                // Save transaction to Family11 XLS format
                IFamilySaver fs = new XLSFamilySaver(outFn);
                Console.WriteLine($"Saving transactions in file: {outFn} ...");
                fs.SaveTransactions(familyTransactions);
                Console.WriteLine("File saved");

                // Save settings to file
                _settings.Save();

                Console.WriteLine("\nPress any key ...");
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Fatal error: " + ex);
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
