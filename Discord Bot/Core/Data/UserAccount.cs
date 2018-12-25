using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Core.Data
{
    public class UserAccount
    {
        public ulong id { get; set; }
        public UserProfile userProfile;
        public Dictionary<string, SonaData> fursonas; 
        public ModData modData;

        public UserProfile GetUserProfile()
        {
            if (userProfile != null)
                return userProfile;

            userProfile = new UserProfile();
            return userProfile;
        }

        public Dictionary<string, SonaData> GetFursonas()
        {
            if (fursonas != null)
                return fursonas;

            fursonas = new Dictionary<string, SonaData>();
            return fursonas;
        }

        public SonaData GetSonaData(string sona)
        {
            var sonas = GetFursonas();
            if (sonas == null)
                return null;

            if (sonas.ContainsKey(sona.ToLowerInvariant()))
                return sonas[sona];
            else
                return null;
        }
    };

    public class UserProfile
    {
        public UserProfile()
        {
            age = "";
            location = "";
            telegram = "";
            furaffinity = "";
            battlenet = "";
            steam = "";
            psn = "";
            xbox = "";
            hobbies = "";
            aboutme = "";
        }

        public string age;
        public string location;
        public string telegram;
        public string furaffinity;
        public string battlenet;
        public string steam;
        public string psn;
        public string xbox;
        public string hobbies;
        public string aboutme;
    }

    public class SonaData
    {
        public SonaData()
        {
            nsfw = false;
            name = "";
            species = "";
            gender = "";
            age = 0;
            height = 0;
            weight = 0;
            bodytype = "";
            clothing = "";
            personality = "";
            biography = "";
            imagelink = "";

        }

        public bool nsfw;
        public string name;
        public string species;
        public string gender;
        public float age;
        public float height;
        public float weight;
        public string bodytype;
        public string clothing;
        public string personality;
        public string biography;
        public string imagelink;

    }

    public class ModData
    {
        public ModData()
        {
            warnings = new List<PenaltyData>();
            mutes = new List<MuteData>();
            kicks = new List<PenaltyData>();
            softBans = new List<PenaltyData>();
            bans = new List<PenaltyData>();

            banned = false;
        }

        public int WarnCount()
        {
            try { return warnings.Count; }
            catch { return 0; }
        }
        
        public int MuteCount()
        {
            try { return mutes.Count; }
            catch { return 0; }
        }

        public int KickCount()
        {
            try { return kicks.Count; }
            catch { return 0; }
        }

        public int SoftBanCount()
        {
            try { return softBans.Count; }
            catch { return 0; }
        }

        public int BanCount()
        {
            try { return bans.Count; }
            catch { return 0; }
        }

        public int UnBanCount()
        {
            try { return unBans.Count; }
            catch { return 0; }
        }

        public List<PenaltyData> warnings;
        public List<MuteData> mutes;
        public List<PenaltyData> kicks;
        public List<PenaltyData> softBans;
        public List<PenaltyData> bans;
        public List<PenaltyData> unBans;
        public bool banned;

        public struct PenaltyData
        {
            public PenaltyData(ulong _staff = 0, string _date = "", string _reason = "No reason specified")
            {
                staff = _staff;
                date = _date;
                reason = _reason;
            }

            public ulong staff;
            public string date;
            public string reason;
        }

        public struct MuteData
        {
            public MuteData(ulong _staff = 0, string _date = "", string _reason = "No reason specified", string _time = "")
            {
                staff = _staff;
                date = _date;
                reason = _reason;
                time = _time;
            }

            public ulong staff;
            public string date;
            public string reason;
            public string time;
        }
    }

}
