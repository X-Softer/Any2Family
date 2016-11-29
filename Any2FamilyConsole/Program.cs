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

            int ConvType = Convert.ToInt32(args[1]);

            //TLConverterSettings TLSettings = LoadSettings("MappingRules.txt");
            TLConverterSettings TLSettings = LoadSettings(SettingsFileName);

            // Choose reader and converter
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
            SaveSettings(TLSettings, SettingsFileName);

            Console.WriteLine("\nPress any key ...");
            Console.ReadKey();
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

        // Load settings
        static TLConverterSettings LoadSettings(string fn)
        {
            //return LoadTXTSettings(fn);
            return LoadXMLSettings(fn);
        }

        // Save settings
        static void SaveSettings(TLConverterSettings setts, string fn)
        {
            SaveXMLSettings(setts, fn);
        }

        // Load settings from TXT-file (not using)
        private static TLConverterSettings LoadTXTSettings(string fn)
        {
            TLConverterSettings setts = new TLConverterSettings();

            using (StreamReader sr = new StreamReader(File.OpenRead(fn)))
            {
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        continue;
                    }

                    Match mc = Regex.Match(str, @"({.+})({.+})({.+})=({.+})({.+})");
                    if (mc.Groups.Count < 6)
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

        // Load settings from XML-file
        private static TLConverterSettings LoadXMLSettings(string fn)
        {
            TLConverterSettings setts = new TLConverterSettings();

            XDocument SettingsDoc = XDocument.Load(fn);

            var DefaultValElements = SettingsDoc?.Element("settings")?.Descendants("default_value");
            setts.DefaultCategory = DefaultValElements?.Where(x => x.Attribute("field_name")?.Value == "Category").First().Value;

            var MappingRulesElements = SettingsDoc?.Element("settings")?.Element("mapping_rules")?.Descendants("mapping_rule");
            foreach(var mr_element in MappingRulesElements)
            {
                MappingEntry me = new MappingEntry();
                me.SourceName = mr_element.Attribute("converter")?.Value;
                me.SourceEntryPropertyName = mr_element.Element("source_property").Attribute("name").Value;
                me.SourceEntryPropertyValue = mr_element.Element("source_property").Value;
                me.TargetEntryPropertyName = mr_element.Element("target_property").Attribute("name").Value;
                me.TargetEntryPropertyValue = mr_element.Element("target_property").Value;
                setts.MappingRules.Add(me);
            }

            return setts;
        }

        //
        private static void SaveXMLSettings(TLConverterSettings setts, string fn)
        {
            Encoding enc = new UTF8Encoding(false);

            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            ws.IndentChars = "\t";
            ws.NewLineChars = "\n";
            ws.Encoding = enc;

            using (XmlWriter writer = XmlWriter.Create(fn, ws))
            {

                writer.WriteStartElement("settings");

                writer.WriteStartElement("default_value");
                writer.WriteAttributeString("field_name", "Category");
                writer.WriteString(setts.DefaultCategory);
                writer.WriteEndElement();

                writer.WriteStartElement("mapping_rules");

                foreach (var sett in setts.MappingRules)
                {
                    writer.WriteStartElement("mapping_rule");
                    writer.WriteAttributeString("converter", sett.SourceName);
                    writer.WriteStartElement("source_property");
                    writer.WriteAttributeString("name", sett.SourceEntryPropertyName);
                    writer.WriteString(sett.SourceEntryPropertyValue);
                    writer.WriteEndElement();
                    writer.WriteStartElement("target_property");
                    writer.WriteAttributeString("name", sett.TargetEntryPropertyName);
                    writer.WriteString(sett.TargetEntryPropertyValue);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }   
    }
}
