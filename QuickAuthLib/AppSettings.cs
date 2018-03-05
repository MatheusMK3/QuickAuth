using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuickAuthLib
{
    public class AppSettings
    {
        private string LoadedFile;

        public IDictionary<string, string> App { get; set; }
        public IDictionary<string, string> LoginPage { get; set; }
        public IDictionary<string, ConnectionStatusValue> ConnectionStatus { get; set; }
        public IDictionary<string, SavedNetwork> SavedNetworks { get; set; }

        public static AppSettings LoadedSettings;

        public struct ConnectionStatusValue
        {
            public string StatusText;
            public string StatusColor;
        }
        public struct SavedNetwork
        {
            public string Username;
            public string Password;
        }

        public static AppSettings Load(string file)
        {
            // Stream de Arquivo e leitura
            FileStream fileAccess = new FileStream(file, FileMode.Open);
            StreamReader reader = new StreamReader(fileAccess);

            // Copiar dados do stream
            string rawJson = reader.ReadToEnd();

            // Fechar stream
            reader.Close();
            fileAccess.Close();

            AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(rawJson);
            settings.LoadedFile = file;

            // Global (loaded) app settings
            LoadedSettings = settings;

            return settings;
        }
        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            
            if (this.LoadedFile != null)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(this.LoadedFile, false);
                    writer.Write(json);
                    writer.Close();
                }
                catch { }
            }
        }
    }
}
