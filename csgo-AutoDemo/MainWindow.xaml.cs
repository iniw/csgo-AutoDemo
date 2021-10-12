using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSGSI;
using CSGSI.Nodes;
using Microsoft.Win32;

namespace csgo_AutoDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        private bool enabled = true;

        private GameStateListener gsl;

        private static readonly string csgopath =
                (string)
                    Registry.GetValue(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam",
                        "InstallPath", "") + @"\steamapps\common\Counter-Strike Global Offensive";

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT : IDisposable
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;

            public void Dispose()
            {
                if (lpData != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(lpData);
                    lpData = IntPtr.Zero;
                    cbData = 0;
                }
            }
            public static COPYDATASTRUCT CreateForString(int dwData, string value, bool Unicode = false)
            {
                var result = new COPYDATASTRUCT();
                result.dwData = (IntPtr)dwData;
                result.lpData = Unicode ? Marshal.StringToCoTaskMemUni(value) : Marshal.StringToCoTaskMemAnsi(value);
                result.cbData = value.Length + 1;
                return result;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        private void ExecuteCmd(string cmd, string args = "")
        {
            var message = cmd + args;

            var cds = COPYDATASTRUCT.CreateForString(0, message);

            var window = FindWindow("Valve001", null);

            SendMessage(window, 0x4A, IntPtr.Zero, ref cds);

            cds.Dispose();
        }
        private string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        private bool recording = false;
        private void Record()
        {
            var Y = DateTime.Now.ToString("yyyy");
            var M = DateTime.Now.ToString("MMMM");
            var D = DateTime.Now.ToString("dd") + GetDaySuffix(DateTime.Now.Day);
            var HMS = DateTime.Now.ToString("HH_mm_ss");
            Directory.CreateDirectory(csgopath + $@"\csgo\pov\{Y}\{M}\{D}");

            var demo_name = $"pov/{Y}/{M}/{D}/{HMS}";

            ExecuteCmd("stop");
            ExecuteCmd("record ", demo_name);

            Log($"Started recording to {demo_name}");

            recording = true;
        }

        private void OnNewGameState(GameState gs)
        {
            if (!recording)
            {
                if (gs.Round.Phase == RoundPhase.FreezeTime)
                {
                    Record();
                }
            }
            else
            {
                if (gs.Player.Activity != PlayerActivity.Playing && gs.Map.Phase == MapPhase.Undefined)
                {
                    Log("Stopped recording");
                    recording = false;

                }
            }
        }

        private void PlaceGSLConfig()
        {
            try
            {
                File.Copy("gamestate_integration_autodemo.cfg", $@"{csgopath}\csgo\cfg\gamestate_integration_autodemo.cfg", true);
                Log("Created CSGI config");
            }
            catch (Exception exc) {
                Log($"Failed to create CSGI config: {exc.Message}");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(csgopath))
            {
                Log("Couldn't find csgo");
            }
            else
            {
                PlaceGSLConfig();
                Directory.CreateDirectory(csgopath + @"\csgo\pov");
                Log("Created csgo/pov directory");
            }

            gsl = new GameStateListener(3000);
            gsl.NewGameState += OnNewGameState;

            if (!gsl.Start())
            {
                Log("Couldn't start listener");
                Log($"Maybe port {gsl.Port} can't be bound?");
            }
            else
            {
                Log("CSGI started");
            }

            this.Closed += (_, __) =>
            {
                Environment.Exit(0);
            };

            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("csgo-AutoDemo.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        private void Log(string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                DebugOutput.Text = DebugOutput.Text + "\n" + msg;
                DebugOutput.ScrollToEnd();
            });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (enabled)
            {
                gsl.NewGameState -= OnNewGameState;
                Enabler.Content = Enabler.Content.ToString().Replace("Disable", "Enable");

                Log("Stopped listening");
            }
            else
            {
                gsl.NewGameState += OnNewGameState;
                Enabler.Content = Enabler.Content.ToString().Replace("Enable", "Disable");
                Log("Started listening");
            }

            enabled = !enabled;
        }

        private void Record_Button_Click(object sender, RoutedEventArgs e)
        {
            Record();
        }

        private static readonly RegistryKey StartUpKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private void StartWithWindows_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            StartUpKey.SetValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().Location);

            Log("Starting with Windows from now on");
        }

        private void StartWithWindows_checkBox_UnChecked(object sender, RoutedEventArgs e)
        {
            StartUpKey.DeleteValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

            Log("Not starting with Windows from now on");
        }

        private void StartWithWindows_checkBox_Initialized(object sender, EventArgs e)
        {
            if (StartUpKey.GetValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name) != null)
                StartWithWindows_checkBox.IsChecked = true;
        }
    }
}
