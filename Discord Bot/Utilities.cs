using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Bot
{
    class Utilities
    {
        private static Dictionary<string, string> alerts;

        static Utilities()
        {
            string json = File.ReadAllText("SystemLang/alerts.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string,string>>();
        }

        public static string GetAlert(string key)
        {
            if (alerts.ContainsKey(key)) return alerts[key];
            return "";
        }

        public static string GetFormattedAlert(string key, params object[] parameter)
        {
            if (alerts.ContainsKey(key))
            {
                return String.Format(alerts[key], parameter);
            }
            return "";
        }

        public static string GetFormattedAlert(string key, object parameter)
        {
            return GetFormattedAlert(key, new object[] { parameter });
        }

        public static string GetMonth()
        {
            string m = "";
            switch (DateTime.Today.Month)
            {
                case 1: m = "January"; break;
                case 2: m = "February"; break;
                case 3: m = "March"; break;
                case 4: m = "April"; break;
                case 5: m = "May"; break;
                case 6: m = "June"; break;
                case 7: m = "July"; break;
                case 8: m = "August"; break;
                case 9: m = "September"; break;
                case 10: m = "October"; break;
                case 11: m = "November"; break;
                case 12: m = "December"; break;
            }

            return m;
        }

        public static string GetMonth(DateTime date)
        {
            string m = "";
            switch (date.Month)
            {
                case 1: m = "January"; break;
                case 2: m = "February"; break;
                case 3: m = "March"; break;
                case 4: m = "April"; break;
                case 5: m = "May"; break;
                case 6: m = "June"; break;
                case 7: m = "July"; break;
                case 8: m = "August"; break;
                case 9: m = "September"; break;
                case 10: m = "October"; break;
                case 11: m = "November"; break;
                case 12: m = "December"; break;
            }

            return m;
        }

        public static string GetMonth(int month)
        {
            string m = "";
            switch (month)
            {
                case 1: m = "January"; break;
                case 2: m = "February"; break;
                case 3: m = "March"; break;
                case 4: m = "April"; break;
                case 5: m = "May"; break;
                case 6: m = "June"; break;
                case 7: m = "July"; break;
                case 8: m = "August"; break;
                case 9: m = "September"; break;
                case 10: m = "October"; break;
                case 11: m = "November"; break;
                case 12: m = "December"; break;
            }

            return m;
        }

        public static string GetDate()
        {
            int d = DateTime.Today.Day;
            string m = GetMonth();
            int y = DateTime.Today.Year;

            return $"{d} {m} {y}";
        }


        public static string GetDate(DateTime date)
        {
            int d = date.Day;
            string m = GetMonth(date);
            int y = date.Year;

            return $"{d} {m} {y}";
        }
    }
}
