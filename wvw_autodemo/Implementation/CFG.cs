using System.IO;
using Newtonsoft.Json.Linq;

namespace wvw_autodemo
{
    public class CFG
    {
        public static readonly string CFGNAME = "wvw_autodemo_cfg.json";

        public static string CSGOPath = string.Empty;

        public static bool SetDirectory()
        {
            string dir = CSGOPath + @"\csgo\pov";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return true;
            }

            return false;
        }

        public static bool Read()
        {
            if (!File.Exists(CFGNAME))
                return false;

            string json = File.ReadAllText(CFGNAME);

            JObject obj = JObject.Parse(json);

            CSGOPath = obj["csgo_path"].ToString();

            return true;
        }

        public static void Write()
        {
            JObject obj = new JObject();

            obj.Add("csgo_path", CSGOPath);

            File.WriteAllText(CFGNAME, obj.ToString());
        }
    }
}