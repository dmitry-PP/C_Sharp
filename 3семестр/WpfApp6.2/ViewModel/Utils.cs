using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApp6._2.ViewModel
{
    static class Manager
    {
        static string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Saver.json");

        static public void Serializer<T>(T records)
        {
            string json = JsonConvert.SerializeObject(records);

            File.WriteAllText(path, json);

        }

        static public T Deserializer<T>()
        {
            string json = File.ReadAllText(path);
            T data = JsonConvert.DeserializeObject<T>(json);

            return data;
        }
    }

}
