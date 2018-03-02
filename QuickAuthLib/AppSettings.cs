using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuickAuthLib
{
    class AppSettings
    {
        public static object[] load(string file)
        {
            List<object> settings = new List<object>();

            // Stream de Arquivo e leitura
            FileStream fileAccess = new FileStream(file, FileMode.Open);
            StreamReader reader = new StreamReader(fileAccess);

            // Copiar dados do stream
            string rawJson = reader.ReadToEnd();

            // Fechar stream
            reader.Close();
            fileAccess.Close();

            JsonConvert.DeserializeObject(rawJson);

            return settings.ToArray();
        }
        public static object get(string key)
        {
            return null;
        }
    }
}
