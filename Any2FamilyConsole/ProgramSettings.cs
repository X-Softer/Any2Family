using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        string FileName;
        static ProgramSettings _instance;

        private ProgramSettings(string file_name, bool need_to_load)
        {
            FileName = file_name;

            if(need_to_load)
            {
                Reload();
            }
        }

        public static ProgramSettings Create(string file_name, bool need_to_load = true)
        {
            _instance = _instance ?? new ProgramSettings(file_name, need_to_load);
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

            using (XmlWriter writer = XmlWriter.Create(FileName, ws))
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

            XDocument SettingsDoc = XDocument.Load(FileName);

            var DefaultValElements = SettingsDoc?.Element("settings")?.Descendants("default_value");
            TLSettings.DefaultCategory = DefaultValElements?.Where(x => x.Attribute("field_name")?.Value == "Category").First().Value;

            var MappingRulesElements = SettingsDoc?.Element("settings")?.Element("mapping_rules")?.Descendants("mapping_rule");
            foreach (var mr_element in MappingRulesElements)
            {
                MappingEntry me = new MappingEntry();
                me.SourceName = mr_element.Attribute("converter")?.Value;
                me.SourceEntryPropertyName = mr_element.Element("source_property").Attribute("name").Value;
                me.SourceEntryPropertyValue = mr_element.Element("source_property").Value;
                me.TargetEntryPropertyName = mr_element.Element("target_property").Attribute("name").Value;
                me.TargetEntryPropertyValue = mr_element.Element("target_property").Value;
                TLSettings.MappingRules.Add(me);
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

            XDocument SettingsDoc = XDocument.Load(FileName);
            var ContragentElements = SettingsDoc?.Element("settings")?.Element("family_contragents")?.Descendants("contragent");

            if (ContragentElements != null)
            {
                foreach (var contrElem in ContragentElements)
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

            XDocument SettingsDoc = XDocument.Load(FileName);
            var CategoryElements = SettingsDoc?.Element("settings")?.Element("family_categories")?.Descendants("category");

            if (CategoryElements != null)
            {
                foreach (var catElem in CategoryElements)
                {
                    FamilyCategory cat = new FamilyCategory()
                    {
                        Type = Int32.Parse(catElem.Attribute("type").Value),
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
