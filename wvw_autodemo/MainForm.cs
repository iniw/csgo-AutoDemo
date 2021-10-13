using System;
using System.Windows.Forms;

namespace wvw_autodemo
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();

            AutoDemo.Init(this);

            WindowsStart.Checked = CFG.WindowsStart;

            Closed += (_, __) =>
            {
                Environment.Exit(0);
            };
        }

        private void WindowsStart_CheckedChanged(object sender, EventArgs e)
        {
            if (WindowsStart.Checked)
            {
                RegistryInfo.STARTUPKEY.SetValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().Location);

                Utils.Log("Starting with Windows");
            }
            else
            {
                RegistryInfo.STARTUPKEY.DeleteValue(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

                Utils.Log("Not starting with Windows");
            }

            CFG.WindowsStart = WindowsStart.Checked;
            CFG.Write();
        }

        private void SetPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = RegistryInfo.STEAMPATH;
            openFileDialog.DefaultExt = ".exe";
            openFileDialog.Filter = "exe file|csgo.exe";
            openFileDialog.FilterIndex = 0;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // remove "\csgo.exe" from the string
                string fileName = openFileDialog.FileName;
                string path = fileName.Substring(0, fileName.LastIndexOf('\\'));

                CFG.CSGOPath = path;
                CFG.Write();

                Utils.Log($"CS:GO path saved to {CFG.CFGNAME}");

                CFG.SetDirectory();

                if (GSI.CopyCFG())
                    AutoDemo.Setup = GSI.Setup(AutoDemo.OnNewGameState);

                SetPath.Visible = false;
            }
        }

        private void ForceRecord_Click(object sender, EventArgs e)
        {
            if (!AutoDemo.Setup)
            {
                Utils.Log("Set your CS:GO path first");
                return;
            }

            AutoDemo.StartRecording();
        }
    }
}