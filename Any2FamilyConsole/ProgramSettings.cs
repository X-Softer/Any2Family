using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using FamilyConverter;

namespace Any2FamilyConsole
{
    public class ProgramSettings
    {
        public List<FamilyCategory> FamilyCategories { get; private set; }
        public List<string> FamilyContragents { get; private set; }
        public TLConverterSettings TLSettings { get; private set; }

        private readonly string _fileName;
        private static ProgramSettings _instance;

        private ProgramSettings(string fileName, bool needToLoad)
        {
            _fileName = fileName;

            if(needToLoad)
            {
                Reload();
            }
        }

        public static ProgramSettings Create(string fileName, bool needToLoad = true)
        {
            _instance = _instance ?? new ProgramSettings(fileName, needToLoad);
            return _instance;
        }

        // Load settings
        public void Reload()
        {
            LoadTLSettings();
            LoadFamilyCategories();
            LoadFamilyContragents();
        }

        // Save settings
        public void Save()
        {
            Encoding enc = new UTF8Encoding(false);

            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            ws.IndentChars = "\t";
            ws.NewLineChars = "\n";
            ws.Encoding = enc;

            using (XmlWriter writer = XmlWriter.Create(_fileName, ws))
            {
                writer.WriteStartElement("settings");
                WriteTLSettings(writer);
                WriteFamilyCategories(writer);
                WriteFamilyContragents(writer);
                writer.WriteEndElement();
            }
        }

        // Load settings from XML-file
        private void LoadTLSettings()
        {
            if(TLSettings == null)
            {
                TLSettings = new TLConverterSettings();
            }
            TLSettings.DefaultCategory = null;
            TLSettings.MappingRules.Clear();

            XDocument settingsDoc = XDocument.Load(_fileName);

            var defaultValElements = settingsDoc.Element("settings")?.Descendants("default_value");
            TLSettings.DefaultCategory = defaultValElements?.Where(x => x.Attribute("field_name")?.Value == "Category").First().Value;

            var mappingRulesElements = settingsDoc.Element("settings")?.Element("mapping_rules")?.Descendants("mapping_rule");
            if (mappingRulesElements != null)
            {
                foreach (var mrElement in mappingRulesElements)
                {
                    MappingEntry me = new MappingEntry();
                    me.SourceName = mrElement.Attribute("converter")?.Value;
                    me.SourceEntryPropertyName = mrElement.Element("source_property")?.Attribute("name")?.Value;
                    me.SourceEntryPropertyValue = mrElement.Element("source_property")?.Value;
                    me.TargetEntryPropertyName = mrElement.Element("target_property")?.Attribute("name")?.Value;
                    me.TargetEntryPropertyValue = mrElement.Element("target_property")?.Value;
                    TLSettings.MappingRules.Add(me);
                }
            }
        }


        // Load family contragents from settings-file
        private void LoadFamilyContragents()
        {
            if (FamilyContragents == null)
            {
                FamilyContragents = new List<string>();
            }
            FamilyContragents.Clear();

            XDocument settingsDoc = XDocument.Load(_fileName);
            var contragentElements = settingsDoc.Element("settings")?.Element("family_contragents")?.Descendants("contragent");

            if (contragentElements != null)
            {
                foreach (var contrElem in contragentElements)
                {
                    FamilyContragents.Add(contrElem.Value);
                }
            }
        }

        // Load family categories from settings-file
        private void LoadFamilyCategories()
        {
            if (FamilyCategories == null)
            {
                FamilyCategories = new List<FamilyCategory>();
            }
            FamilyCategories.Clear();

            XDocument settingsDoc = XDocument.Load(_fileName);
            var categoryElements = settingsDoc.Element("settings")?.Element("family_categories")?.Descendants("category");

            if (categoryElements != null)
            {
                foreach (var catElem in categoryElements)
                {
                    FamilyCategory cat = new FamilyCategory()
                    {
                        Type = Int32.Parse(catElem.Attribute("type")?.Value ?? throw new InvalidOperationException()),
                        Name = catElem.Value
                    };

                    FamilyCategories.Add(cat);
                }
            }
        }

        // Write contragents
        private void WriteFamilyCategories(XmlWriter writer)
        {
            
            writer.WriteStartElement("family_categories");
            foreach (var cat in FamilyCategories)
            {
                writer.WriteStartElement("category");
                writer.WriteAttributeString("type", cat.Type.ToString());
                writer.WriteValue(cat.Name);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        // Write categories
        private void WriteFamilyContragents(XmlWriter writer)
        {
            writer.WriteStartElement("family_contragents");

            foreach (var ca in FamilyContragents)
            {
                writer.WriteElementString("contragent", ca);
            }

            writer.WriteEndElement();
        }
        
        // Write TLSettings
        private void WriteTLSettings(XmlWriter writer)
        {
            // Write default value
            writer.WriteStartElement("default_value");
            writer.WriteAttributeString("field_name", "Category");
            writer.WriteString(TLSettings.DefaultCategory);
            writer.WriteEndElement();

            // Write mapping rules
            writer.WriteStartElement("mapping_rules");

            foreach (var sett in TLSettings.MappingRules)
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
        }
    }
}
