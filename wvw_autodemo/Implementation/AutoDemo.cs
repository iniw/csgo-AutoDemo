using System;
using System.IO;

using CSGSI;
using CSGSI.Nodes;

namespace wvw_autodemo
{
	public class AutoDemo
	{
		public static bool Setup = false;

		private static bool Recording = false;
		private static string CurrentMap = string.Empty;

		public static void StartRecording()
		{
			var Y = DateTime.Now.ToString("yyyy");
			var M = DateTime.Now.ToString("MMMM");
			var D = DateTime.Now.ToString("dd") + Utils.GetDaySuffix(DateTime.Now.Day);
			var HMS = DateTime.Now.ToString("HH_mm_ss");

			Directory.CreateDirectory(CFG.CSGOPath + $@"\csgo\pov\{Y}\{M}\{D}");

			// TODO: support for custom formatting of the demo name/folder
			var demoName = $"pov/{Y}/{M}/{D}/{CurrentMap}_{HMS}";

			if (!CSGO.ExecuteCmd("stop") || !CSGO.ExecuteCmd("record ", demoName))
				return;

			Utils.Log($"Started recording to {demoName}");

			Recording = true;
		}

		public static void StopRecording()
		{
			if (!CSGO.ExecuteCmd("stop"))
				return;

			Utils.Log("Stopped recording");

			Recording = false;
		}

		public static void OnNewGameState(GameState gs)
		{
			CurrentMap = gs.Map.Name;

			if (!Recording)
			{
				if (gs.Round.Phase == RoundPhase.FreezeTime)
					StartRecording();
			}
			else
			{
				if (gs.Player.Activity != PlayerActivity.Playing && gs.Map.Phase == MapPhase.Undefined)
					StopRecording();
			}
		}

		static public void Init(MainForm Form)
		{
			Utils.Init(Form);

			if (CFG.Read())
			{
				Utils.Log($"CS:GO path retrieved from {CFG.CFGNAME}");

				CFG.SetDirectory();

				if (GSI.CopyCFG())
					Setup = GSI.Setup(OnNewGameState);

				Form.SetPath.Visible = false;
			}
			else
				Utils.Log("Set your CS:GO path");
		}
	}
}
