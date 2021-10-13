using System.IO;

using CSGSI;

namespace wvw_autodemo
{
    public class GSI
    {
        public static GameStateListener GSL;

        public static readonly string CFGName = "gamestate_integration_autodemo.cfg";

        public static bool CopyCFG()
        {
            if (File.Exists(CFGName))
            {
                File.Copy(CFGName, $@"{CFG.CSGOPath}\csgo\cfg\{CFGName}", true);
                Utils.Log($"Copied {CFGName} to csgo/cfg");

                return true;
            }
            else
            {
                Utils.Log($"Couldn't find {CFGName}");
                return false;
            }
  
        }

        public static bool Setup(NewGameStateHandler OnNewGameState)
        {
            GSL = new GameStateListener(3333);
            GSL.NewGameState += OnNewGameState;

            if (GSL.Start())
            {
                Utils.Log("Setup GSI");
                return true;
            }
            else
            {
                Utils.Log("Failed to setup GSI");
                return false;
            }
        }
    }
}