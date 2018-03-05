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
        public IDictionary<string, string> App { get; set; }
        public static AppSettings LoadedSettings;

        public static AppSettings load(string file)
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

            // Global (loaded) app settings
            LoadedSettings = settings;

            return settings;
        }
        public static object get(string key)
        {
            return null;
        }
    }
}
