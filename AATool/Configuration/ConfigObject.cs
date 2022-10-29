using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;

namespace AATool.Configuration
{
    public abstract partial class Config
    {
        private readonly List<ISetting> settings = new ();
        
        [JsonIgnore] private string FileName => $"config_{this.GetId()}.json";
        [JsonIgnore] private string LegacyFileName => $"{this.GetLegacyId()}.xml";

        public bool TrySave() => TrySave(this);

        public void RegisterSetting(ISetting setting)
        {
            if (setting is not null && !this.settings.Contains(setting))
                this.settings.Add(setting);
        }

        protected abstract string GetId();
        protected abstract string GetLegacyId();

        protected virtual void ApplyLegacySetting(string key, object value) { }
        protected virtual void MigrateDepricatedConfigs() { }

        protected virtual void ApplyDefaultValues()
        {
            foreach (ISetting setting in this.settings)
                setting.ApplyDefault();
        }

        private void ClearFlags()
        {
            foreach (ISetting setting in this.settings)
                setting.ClearFlag();
        }

        private void ApplyAllLegacySettings(XmlDocument document)
        {
            foreach (XmlNode setting in document.DocumentElement?.ChildNodes)
            {
                if (TryParseLegacySetting(setting, out string key, out object value))
                    this.ApplyLegacySetting(key, value);
            }
        }
    }
}
