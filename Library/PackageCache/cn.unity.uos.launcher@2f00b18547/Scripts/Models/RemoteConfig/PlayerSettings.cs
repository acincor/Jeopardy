using System.Collections.Generic;
using Unity.UOS.Common;

namespace Unity.UOS.Models.RemoteConfig
{
    public class PlayerSettings : GenericSingleton<PlayerSettings>
    {
        private Dictionary<string, Setting> _settings = new();

        public void CopyFrom(Dictionary<string, Setting> settings)
        {
            _settings = settings;
        }

        public void Add(string key, Setting value)
        {
            _settings.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _settings.Remove(key);
        }

        public Setting Get(string key)
        {
            return _settings.GetValueOrDefault(key);
        }
    }
}