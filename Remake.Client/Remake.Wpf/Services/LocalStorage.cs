using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Remake.Wpf.Services
{
    public interface ILocalStorage
    {
        string? GetItem(string key);
        void SetItem(string key, string value);
    }

    public class LocalStorage : ILocalStorage
    {
        public string? GetItem(string key)
        {
            var storage = Load();

            if (storage == null || !storage.TryGetValue(key, out var value))
            {
                return null;
            }

            return value;
        }

        public void SetItem(string key, string value)
        {
            var storage = Load();

            if (storage == null)
            {
                return;
            }

            storage[key] = value;

            Save(storage);
        }

        private Dictionary<string, string>? Load()
        {
            var filePath = GetFilePath();

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            catch 
            {
                File.WriteAllText(filePath, JsonSerializer.Serialize(new Dictionary<string, string>()));
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
        }

        private void Save(Dictionary<string, string> storage)
        {
            var filePath = GetFilePath();
            var json = JsonSerializer.Serialize(storage);
            File.WriteAllText(filePath, json);
        }

        private string GetFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Remake");
        }

        private string GetFilePath()
        {
            return Path.Combine(GetFolderPath(), "appsettings.json");
        }
    }
}
