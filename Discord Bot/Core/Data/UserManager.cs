using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace Discord_Bot.Core.Data
{
    public static class UserManager
    {
        public static List<UserAccount> accounts;
        private static string accountsFile = "Resources/accounts.json";

        static UserManager()
        {
            if (DataStorage.SaveExists(accountsFile))
                accounts = DataStorage.LoadUserAccounts(accountsFile).ToList();
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        public static List<UserAccount> GetAccounts()
        {
            return accounts;
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        public static bool AccountExists(SocketUser user)
        {
            var result = from a in accounts
                         where a.id == user.Id
                         select a;

            var account = result.FirstOrDefault();

            if (account == null) return false;
            else return true;
        }

        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts
                         where a.id == id
                         select a;

            var account = result.FirstOrDefault();

            if (account == null) return CreateUserAccount(id);

            return account;
        }

        private static UserAccount CreateUserAccount(ulong _id)
        {
            var newAccount = new UserAccount
            {
                id = _id,
                userProfile = new UserProfile(),
                fursonas = new Dictionary<string, SonaData>(),
                modData = new ModData()
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }

    }
}
