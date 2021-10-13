using System;
using System.Windows;
using System.IO;

using Microsoft.Win32;

using CSGSI;
using CSGSI.Nodes;

namespace wvw_autodemo
{
    public partial class MainWindow : Window
    {

        private bool Setup = false;
        private bool Recording = false;
        private string CurrentMap = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            if (CFG.Read())
            {
                Log("CS:GO path retrieved from cfg file");

                if (CFG.SetDirectory())
                    Log("Setup csgo/pov directory");

                SetGSICFG();

                if (GSI.Setup(OnNewGameState))
                    Log("Setup GSI");
                else
                    Log("Failed to setup GSI");

                Setup = true;

                SetPath.Visibility = Visibility.Hidden;
            }
            else
                Log("Set your CS:GO path");

            Closed += (_, __) =>
            {
                Environment.Exit(0);
            };
        }


        private void OnNewGameState(GameState gs)
        {
            CurrentMap = gs.Map.Name;

            if (!Recording)
            {
                if (gs.Round.Phase == RoundPhase.FreezeTime)
                {
                    StartRecording();
                }
            }
            else
            {
                if (gs.Player.Activity != PlayerActivity.Playing && gs.Map.Phase == MapPhase.Undefined)
                {
                    Log("Stopped recording");
                    Recording = false;
                }
            }
        }

        private void StartRecording()
        {
            var Y = DateTime.Now.ToString("yyyy");
            var M = DateTime.Now.ToString("MMMM");
            var D = DateTime.Now.ToString("dd") + Utils.GetDaySuffix(DateTime.Now.Day);
            var HMS = DateTime.Now.ToString("HH_mm_ss");

            Directory.CreateDirectory(CFG.CSGOPath + $@"\csgo\pov\{Y}\{M}\{D}");

            // TODO: support for custom formatting of the demo name/folder
            var demoName = $"pov/{Y}/{M}/{D}/{CurrentMap}_{HMS}";

            if (!CSGO.ExecuteCmd("stop") || !CSGO.ExecuteCmd("record ", demoName))
            {
                Log("Failed to execute command");
                return;
            }

            Log($"Started recording to {demoName}");

            Recording = true;
        }

        private void SetGSICFG()
        {
            try
            {
                if (File.Exists("gamestate_integration_autodemo.cfg"))
                {
                    File.Copy("gamestate_integration_autodemo.cfg", $@"{CFG.CSGOPath}\csgo\cfg\gamestate_integration_autodemo.cfg", true);
                }
                else
                {
                    Log("Default GSI config not found");
                    return;
                }

                Log("Copied GSI config to csgo/cfg");
            }
            catch (Exception exc)
            {
                Log($"Failed to copy GSI config: {exc.Message}");
                return;
            }
        }


        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

        public void Log(string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                LogOutput.Text = LogOutput.Text + '\n' + msg;
                LogOutput.ScrollToEnd();
            });
        }

        private void ForceRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!Setup)
            {
                Log("Set your CS:GO path first");
                return;
            }

            StartRecording();
        }

        private void SetPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = RegistryInfo.STEAMPATH;
            openFileDialog.DefaultExt = ".exe";
            openFileDialog.Filter = "exe file|csgo.exe";
            openFileDialog.FilterIndex = 0;
            if (openFileDialog.ShowDialog() == true)
            {
                // remove "\csgo.exe" from the string
                string fileName = openFileDialog.FileName;
                string path = fileName.Substring(0, fileName.LastIndexOf('\\'));

                CFG.CSGOPath = path;
                CFG.Write();

                Log($"Changes written to {CFG.CFGNAME} file");

                if (CFG.SetDirectory())
                    Log("Setup csgo/pov directory");

                SetGSICFG();

                if (GSI.Setup(OnNewGameState))
                    Log("Setup GSI");
                else
                    Log("Failed to setup GSI");

                Setup = true;

                SetPath.Visibility = Visibility.Hidden;
            }
        }

        private void WindowsStart_Checked(object sender, RoutedEventArgs e)
        {
            RegistryInfo.STARTUPKEY.SetValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().Location);

            Log("Starting with Windows from now on");
        }

        private void WindowsStart_UnChecked(object sender, RoutedEventArgs e)
        {
            RegistryInfo.STARTUPKEY.DeleteValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

            Log("Not starting with Windows from now on");
        }

        private void WindowsStart_Initialized(object sender, EventArgs e)
        {
            if (RegistryInfo.STARTUPKEY.GetValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name) != null)
                WindowsStart.IsChecked = true;
        }
    }
}