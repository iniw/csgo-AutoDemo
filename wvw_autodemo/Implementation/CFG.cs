using System.IO;
using Newtonsoft.Json.Linq;

namespace wvw_autodemo
{
    public class CFG
    {
        public static readonly string CFGNAME = "wvw_autodemo_cfg.json";

        public static string CSGOPath = string.Empty;
        public static readonly string CSGOPATHKEY = "csgo_path";

        public static bool WindowsStart = false;
        public static readonly string WINDOWSSTARTKEY = "start_windows";

        public static void SetDirectory()
        {
            string dir = CSGOPath + @"\csgo\pov";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Utils.Log("Setup csgo/pov directory");
            }
        }

        public static bool Read()
        {
            if (!File.Exists(CFGNAME))
                return false;

            string json = File.ReadAllText(CFGNAME);

            JObject obj = JObject.Parse(json);
            JToken val;

            if (obj.TryGetValue(CSGOPATHKEY, out val))
                CSGOPath = (string)val;

            if (obj.TryGetValue(WINDOWSSTARTKEY, out val))
                WindowsStart = (bool)val;

            return true;
        }

        public static void Write()
        {
            JObject obj = new JObject();

            obj.Add(CSGOPATHKEY, CSGOPath);
            obj.Add(WINDOWSSTARTKEY, WindowsStart);

            File.WriteAllText(CFGNAME, obj.ToString());
        }
    }
}