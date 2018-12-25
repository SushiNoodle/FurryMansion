using Discord;
using Discord.WebSocket;
using Discord_Bot.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_Bot.Modules.Role_System
{
    public static class RoleManager
    {
        private static Dictionary<string, System.UInt64> Roles;
        private static List<string> SelfAssignable;
        internal static List<string> SelfAssignable18;
        private static List<string> ModRoles;
        private static List<string> AdminRoles;

        private static string SelfAssignableRolesFile = "Resources/Roles/SelfAssignable.json";
        private static string SelfAssignable18RolesFile = "Resources/Roles/SelfAssignable18.json";
        private static string ModRolesFile = "Resources/Roles/Mod.json";
        private static string AdminRolesFile = "Resources/Roles/Admin.json";


        static RoleManager()
        {
            if (DataStorage.SaveExists(SelfAssignableRolesFile))
                SelfAssignable = DataStorage.LoadRoles(SelfAssignableRolesFile).ToList();
            else
            {
                SetSelfAssignableDefault();
                SaveSelfAssignableRoles();
            }

            if (DataStorage.SaveExists(SelfAssignable18RolesFile))
                SelfAssignable18 = DataStorage.LoadRoles(SelfAssignable18RolesFile).ToList();
            else
            {
                SetSelfAssignable18Default();
                SaveSelfAssignable18Roles();
            }

            if (DataStorage.SaveExists(ModRolesFile))
                ModRoles = DataStorage.LoadRoles(ModRolesFile).ToList();
            else
            {
                ModRoles = new List<string> { "🤠 Chief", "⚔️ White Knight", "🤖 Bully Hunter", "🧠 Big Brain" };
                SaveModRoles();
            }

            if (DataStorage.SaveExists(AdminRolesFile))
                AdminRoles = DataStorage.LoadRoles(AdminRolesFile).ToList();
            else
            {
                AdminRoles = new List<string> { "🤠 Chief", "⚔️ White Knight", "🧠 Big Brain" };
                SaveAdminRoles();
            }

            Initialize();
        }

        static internal void Initialize()
        {
            SocketGuild guild = Global.Client.GetGuild(Global.GuildID);

            // Allocate memory for roles dictionary.
            Roles = new Dictionary<string, UInt64>();

            // Save all roles.
            foreach (SocketRole i in guild.Roles)
            {
                var role = guild.GetRole(i.Id);
                Roles[i.Name] = i.Id;
            }
        }

        public static void SetSelfAssignableDefault()
        {
            SelfAssignable = new List<string>
            {
                "Africa",
                "Asia",
                "Europe",
                "North America",
                "Oceania",
                "South America",

                "Male",
                "Female",
                "Trans",
                "Non Binary",

                "Amphibian",
                "Aquatic",
                "Avian",
                "Bovine",
                "Camelid",
                "Canine",
                "Cervine",
                "Dragon",
                "Equine",
                "Feline",
                "Furry",
                "Hyena",
                "Insect",
                "Lupine",
                "Mammal (Other)",
                "Marsupial",
                "Mustelid",
                "Primate",
                "Scalie",
                "Rodent",
                "Vulpine",
                "Sergal",
                "ShapeShifter",
                "Snek",
                "Ursine",
                "Human"
            };
        }

        public static void SetSelfAssignable18Default()
        {
            SelfAssignable18 = new List<string>
            {
                "Straight",
                "Homosexual",
                "Bisexual",
                "Pansexual",
                "Demisexual",
                "Asexual",

                "Single",
                "Taken",
                "Open Relationship",

                "Sub",
                "Switch",
                "Dom",

                "Master/Mistress",
                "Pet",
                "Owner",
                "Slave",
                "Daddy/Mommy",
                "Brat",

                "Bearbody",
                "Chubby",
                "Curvy",
                "Femboy",
                "Muscular",
                "Thick",
                "Tomboy",
                "Toned",
                "Trap",
                "Twink",
                "Slim",

                "Extreme +18"
            };
        }

        public static void SaveRoles(List<string> list, string file)
        {
            DataStorage.SaveRoles(list, file);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{ DateTime.Now.ToShortTimeString()} | [Discord] : Roles saved for {file}.");
            Console.ResetColor();
        }

        public static void SaveModRoles() => SaveRoles(ModRoles, ModRolesFile);
        public static void SaveAdminRoles() => SaveRoles(AdminRoles, AdminRolesFile);
        public static void SaveSelfAssignableRoles() => SaveRoles(SelfAssignable, SelfAssignableRolesFile);
        public static void SaveSelfAssignable18Roles() => SaveRoles(SelfAssignable18, SelfAssignable18RolesFile);

        public static IRole RemoveRole(string role, List<string> list, string file)
        {
            var r = GetRole(role);

            if (list.Contains(r.Name))
            {
                list.Remove(role);
                SaveRoles(list, file);
                return r;
            }

            return null;
        }

        public static IRole RemoveModRole(string role) => RemoveRole(role, ModRoles, ModRolesFile);
        public static IRole RemoveAdminRole(string role) => RemoveRole(role, AdminRoles, AdminRolesFile);
        public static IRole RemoveSelfAssignable(string role) => RemoveRole(role, SelfAssignable, SelfAssignableRolesFile);
        public static IRole RemoveSelfAssignable18(string role) => RemoveRole(role, SelfAssignable18, SelfAssignable18RolesFile);

        public static IRole AddRole(string role, List<string> list, string file)
        {
            var r = GetRole(role);

            if (!list.Contains(r.Name))
            {
                list.Add(role);
                SaveRoles(list, file);
                return r;
            }

            return null;
        }

        public static IRole AddModRole(string role) => AddRole(role, ModRoles, ModRolesFile);
        public static IRole AddAdminRole(string role) => AddRole(role, AdminRoles, AdminRolesFile);
        public static IRole AddSelfAssignable(string role) => AddRole(role, SelfAssignable, SelfAssignableRolesFile);
        public static IRole AddSelfAssignable18(string role) => AddRole(role, SelfAssignable18, SelfAssignable18RolesFile);
        
        public static bool CheckUserRole(SocketGuildUser user, string role)
        {
            var targetRole = GetRole(role);
            return user.Roles.Contains(targetRole);
        }
        
        public static bool CheckUserRole(SocketGuildUser user, params string[] roles)
        {
            for (int i = 0; i < roles.Length; ++i)
            {
                if (CheckUserRole(user, roles[i]))
                    return true;
            }
        
            return false;
        }
       
        public static bool HasModRole(SocketGuildUser user)
        {
            return CheckUserRole(user, ModRoles.ToArray());
        }
        
        public static bool HasAdminRole(SocketGuildUser user)
        {
            return CheckUserRole(user, AdminRoles.ToArray());
        }

        public static bool Has18Role(SocketGuildUser user)
        {
            string[] r = { "+18", "+18 (no lewd)" };
            return CheckUserRole(user, r);
        }

        public static string GetSelfAssignable(string role)
        {
            foreach (string i in SelfAssignable)
            {
                if (i.ToLowerInvariant() == role.ToLowerInvariant())
                    return i;
            }

            return "";
        }

        public static string GetSelfAssignable18(string role)
        {
            foreach (string i in SelfAssignable18)
            {
                if (i.ToLowerInvariant() == role.ToLowerInvariant())
                    return i;
            }

            return "";
        }

        public static SocketRole GetRole(string role)
        {
            var guild = Global.Client.GetGuild(Global.GuildID);
            var result = from r in guild.Roles
                         where r.Name.ToLowerInvariant() == role.ToLowerInvariant()
                         select r;

            var rolefound = result.FirstOrDefault();

            return rolefound;
        }

        
        public static async Task<IRole> GetOrCreateRoleAsync(string role)
        {
            var guild = Global.Client.GetGuild(Global.GuildID);
            var result = from r in guild.Roles
                         where r.Name.ToLowerInvariant() == role.ToLowerInvariant()
                         select r;

            var rolefound = result.FirstOrDefault();

            if (rolefound == null)
                return await guild.CreateRoleAsync(role, GuildPermissions.None);

            return rolefound;
        }
    }
}
