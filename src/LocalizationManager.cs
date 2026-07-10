using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SyncraRPC.Localization {
    public class LocalizationManager {
        private static LocalizationManager? _instance;
        private readonly Dictionary<string, string> _resources;
        private string? _currentLanguage;

        public static LocalizationManager Instance => _instance ??= new LocalizationManager();

        private LocalizationManager() {
            _resources = new Dictionary<string, string>();
        }

        public void SetLanguage(string languageCode) {
            _currentLanguage = languageCode;
            LoadResources(languageCode);
        }

        public string GetString(string key) {
            if (_resources.TryGetValue(key, out var value))
                return value;
            
            return $"[{key}]";
        }


        private void LoadResources(string languageCode) {
            _resources.Clear();
            
            var fileName = languageCode == "en" 
                ? "bundle.properties" 
                : $"bundle-{languageCode}.properties";
            
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"SyncraRPC.Assets.Localization.{fileName}";
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null) ParsePropertiesStream(stream);
            else {
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(basePath, "Assets", "Localization", fileName);
                
                if (File.Exists(filePath)) ParsePropertiesFile(filePath);
            }
        }

        private void ParsePropertiesStream(Stream stream) {
            using var reader = new StreamReader(stream);
            string? line;
            
            while ((line = reader.ReadLine()) != null) ProcessLine(line);
        }

        private void ParsePropertiesFile(string filePath) {
            var lines = File.ReadAllLines(filePath);
            
            foreach (var line in lines) {
                ProcessLine(line);
            }
        }

        private void ProcessLine(string line) {
            var trimmedLine = line.Trim();
            
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#")) return;

            var equalIndex = trimmedLine.IndexOf('=');
            if (equalIndex > 0) {
                var key = trimmedLine.Substring(0, equalIndex).Trim();
                var value = trimmedLine.Substring(equalIndex + 1).Trim();
                
                value = UnescapeValue(value);
                _resources[key] = value;
            }
        }

        private string UnescapeValue(string value) {
            return value.Replace("\\n", "\n")
                       .Replace("\\t", "\t")
                       .Replace("\\\\", "\\")
                       .Replace("\\\"", "\"");
        }
    }
}