using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Discord_Bot.Core.Data
{
    public class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static void SaveRoles(IEnumerable<string> roles, string filePath)
        {
            string json = JsonConvert.SerializeObject(roles, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<string> LoadRoles(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<string>>(json);
        }

        public static void SaveList<T>(List<T> list, string filePath)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static List<T> LoadList<T>(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }

        // So this bot can access the info and not external APIs.
        private static Dictionary<string, string> pairs = new Dictionary<string, string>();

        public static void AddPairToStorage(string key, string value)
        {
            pairs.Add(key, value);
            SaveData();
        }

        static public int GetPairsCount()
        {
            return pairs.Count();
        }

        // Load Data
        static DataStorage()
        {
            if(!ValidateStorageFile("DataStorage.json"))
                return;

            string json = File.ReadAllText("DataStorage.json");
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SaveData()
        {
            // Save Data
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText("DataStorage.json", json);
        }

        private static bool ValidateStorageFile(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }
            return true;
        }
    }
}
