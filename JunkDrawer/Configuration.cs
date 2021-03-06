using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace JunkDrawer {

    public class DelimiterElementCollection : ConfigurationElementCollection {

        public DelimiterConfigurationElement this[int index] {
            get { return BaseGet(index) as DelimiterConfigurationElement; }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public override bool IsReadOnly() {
            return false;
        }

        protected override ConfigurationElement CreateNewElement() {
            return new DelimiterConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((DelimiterConfigurationElement)element).Character.ToLower();
        }

        public void Add(DelimiterConfigurationElement element) {
            BaseAdd(element);
        }
    }

    public class DelimiterConfigurationElement : ConfigurationElement {

        private const string NAME = "name";
        private const string CHARACTER = "character";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(CHARACTER, IsRequired = true)]
        public string Character {
            get { return this[CHARACTER] as string; }
            set { this[CHARACTER] = value; }
        }
    }

    public class TypeConfigurationElement : ConfigurationElement {

        private const string TYPE = "type";

        [ConfigurationProperty(TYPE, IsRequired = true)]
        public string Type {
            get { return this[TYPE] as string; }
            set { this[TYPE] = value; }
        }
    }

    public class TypeElementCollection : ConfigurationElementCollection {

        private const string TOP = "top";
        private const string SAMPLE = "sample";
        private const string DEFAULT = "default";
        private const string DEFAULT_LENGTH = "default-length";

        [ConfigurationProperty(TOP, IsRequired = false, DefaultValue = 0)]
        public int Top {
            get { return (int)this[TOP]; }
            set { this[TOP] = value; }
        }

        [ConfigurationProperty(SAMPLE, IsRequired = false, DefaultValue = "100")]
        public decimal Sample {
            get { return Convert.ToDecimal(this[SAMPLE]); }
            set { this[SAMPLE] = value; }
        }

        [ConfigurationProperty(DEFAULT, IsRequired = false, DefaultValue = "string")]
        public string Default {
            get { return this[DEFAULT] as string; }
            set { this[DEFAULT] = value; }
        }

        [ConfigurationProperty(DEFAULT_LENGTH, IsRequired = false, DefaultValue = "1024")]
        public string DefaultLength {
            get { return this[DEFAULT_LENGTH] as string; }
            set { this[DEFAULT_LENGTH] = value; }
        }

        public TypeConfigurationElement this[int index] {
            get { return BaseGet(index) as TypeConfigurationElement; }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public override bool IsReadOnly() {
            return false;
        }

        protected override ConfigurationElement CreateNewElement() {
            return new TypeConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((TypeConfigurationElement)element).Type.ToLower();
        }

        public void Add(TypeConfigurationElement element) {
            BaseAdd(element);
        }
    }

    public class Configuration : ConfigurationSection {

        [ConfigurationProperty("types")]
        public TypeElementCollection Types {
            get { return this["types"] as TypeElementCollection; }
        }

        [ConfigurationProperty("delimiters")]
        public DelimiterElementCollection Delimiters {
            get { return this["delimiters"] as DelimiterElementCollection; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        public InspectionRequest GetInspectionRequest() {
            return new InspectionRequest() {
                DataTypes = Types.Cast<TypeConfigurationElement>().Select(t => t.Type).ToArray(),
                DefaultLength = Types.DefaultLength,
                DefaultType = Types.Default,
                Delimiters = Delimiters.Cast<DelimiterConfigurationElement>().ToDictionary(d => d.Character[0], d => d.Name),
                Top = Types.Top,
                Sample = Types.Sample
            };
        }
    }
}