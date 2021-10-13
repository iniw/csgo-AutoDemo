﻿using System;
using System.IO;
using System.Windows;
using System.Text;

using Microsoft.Win32;

using CSGSI;
using CSGSI.Nodes;
using Newtonsoft.Json;

namespace wvw_autodemo
{
    public partial class MainWindow : Window
    {
        private bool m_Recording = false;
        private string m_CurrentMap = string.Empty;
        
        private bool m_Setup = false;

        private GameStateListener m_GSL;

        private static readonly RegistryKey STARTUPKEY = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private static readonly string STEAMPATH = Registry.GetValue( @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", "").ToString();
        
        private static readonly string CFGNAME = "wvw_autodemo_cfg.json";

        private static string m_CSGOPath = string.Empty;

        private void StartRecording()
        {
            var Y = DateTime.Now.ToString("yyyy");
            var M = DateTime.Now.ToString("MMMM");
            var D = DateTime.Now.ToString("dd") + Misc.GetDaySuffix(DateTime.Now.Day);
            var HMS = DateTime.Now.ToString("HH_mm_ss");

            Directory.CreateDirectory(m_CSGOPath + $@"\csgo\pov\{Y}\{M}\{D}");

            // TODO: support for custom formatting of the demo name/folder
            var demoName = $"pov/{Y}/{M}/{D}/{m_CurrentMap}_{HMS}";

            if (!CSGO.ExecuteCmd("stop") || !CSGO.ExecuteCmd("record ", demoName))
            {
                Log("Failed to execute command");
            }

            Log($"Started recording to {demoName}");

            m_Recording = true;
        }

        private void OnNewGameState(GameState gs)
        {
            m_CurrentMap = gs.Map.Name;

            if (!m_Recording)
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
                    m_Recording = false;
                }
            }
        }

        private void SetupDirectories()
        {
            try
            {
                File.Copy("gamestate_integration_autodemo.cfg", $@"{m_CSGOPath}\csgo\cfg\gamestate_integration_autodemo.cfg", true);
                Log("Setup GSI config");
            }
            catch (Exception exc)
            {
                Log($"Failed to create GSI config: {exc.Message}");
                return;
            }

            string dir = m_CSGOPath + @"\csgo\pov";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Log("Setup csgo/pov directory");
            }
        }

        private void SetupCSGSI()
        {
            m_GSL = new GameStateListener(3333);
            m_GSL.NewGameState += OnNewGameState;
                
            if (m_GSL.Start())
                Log("Setup CSGSI");
            else
                Log("Couldn't setup CSGSI");
        }

        private bool ReadFromCFG()
        {
            if (!File.Exists(CFGNAME))
                return false;

            string text = File.ReadAllText(CFGNAME);

            JsonTextReader reader = new JsonTextReader(new StringReader(text));
            
            // this is so shit
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "csgo_path")
                        m_CSGOPath = reader.ReadAsString();
                }
            }

            return true;
        }

        private void SetupCFG(string newCsgoPath)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("csgo_path");
                writer.WriteValue(newCsgoPath);
                writer.WriteEndObject();
            }

            using (StreamWriter file = File.CreateText(@"D:\path.txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, newCsgoPath);
            }

            File.WriteAllText(CFGNAME, sb.ToString());
            Log("Changes written to cfg file");

            ReadFromCFG();
        }

        public MainWindow()
        {
            InitializeComponent();

            if (ReadFromCFG())
            {
                Log("CSGO path retrieved from cfg file");

                SetupDirectories();
                SetupCSGSI();
                m_Setup = true;

                SetPath.Visibility = Visibility.Hidden;
            }
            else
                Log("Setup your CSGO path");

            this.Closed += (_, __) =>
            {
                Environment.Exit(0);
            };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        private void Log(string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                LogOutput.Text = LogOutput.Text + "\n" + msg;
                LogOutput.ScrollToEnd();
            });
        }

        private void ForceRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!m_Setup)
            {
                Log("Setup your CSGO path first");
                return;
            }

            StartRecording();
        }

        private void SetPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = STEAMPATH;
            openFileDialog.DefaultExt = ".exe";
            openFileDialog.Filter = "exe file|csgo.exe";
            openFileDialog.FilterIndex = 0;
            if (openFileDialog.ShowDialog() == true)
            {
                // remove "\csgo.exe" from the string
                string fileName = openFileDialog.FileName;
                string path = fileName.Substring(0, fileName.LastIndexOf('\\'));

                SetupCFG(path);
                SetupDirectories();
                SetupCSGSI();
                m_Setup = true;

                SetPath.Visibility = Visibility.Hidden;
            }
        }

        private void WindowsStart_Checked(object sender, RoutedEventArgs e)
        {
            STARTUPKEY.SetValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().Location);

            Log("Starting with Windows from now on");
        }

        private void WindowsStart_UnChecked(object sender, RoutedEventArgs e)
        {
            STARTUPKEY.DeleteValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

            Log("Not starting with Windows from now on");
        }

        private void WindowsStart_Initialized(object sender, EventArgs e)
        {
            if (STARTUPKEY.GetValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name) != null)
                WindowsStart.IsChecked = true;
        }
    }
}