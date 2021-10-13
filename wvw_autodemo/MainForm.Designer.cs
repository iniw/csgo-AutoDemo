
namespace wvw_autodemo
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LogOutput = new System.Windows.Forms.TextBox();
            this.WindowsStart = new System.Windows.Forms.CheckBox();
            this.SetPath = new System.Windows.Forms.Button();
            this.ForceRecord = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LogOutput
            // 
            this.LogOutput.AcceptsReturn = true;
            this.LogOutput.AcceptsTab = true;
            this.LogOutput.Location = new System.Drawing.Point(12, 37);
            this.LogOutput.Multiline = true;
            this.LogOutput.Name = "LogOutput";
            this.LogOutput.ReadOnly = true;
            this.LogOutput.Size = new System.Drawing.Size(310, 165);
            this.LogOutput.TabIndex = 0;
            this.LogOutput.Text = "Starting up...";
            // 
            // WindowsStart
            // 
            this.WindowsStart.AutoSize = true;
            this.WindowsStart.Location = new System.Drawing.Point(12, 12);
            this.WindowsStart.Name = "WindowsStart";
            this.WindowsStart.Size = new System.Drawing.Size(128, 19);
            this.WindowsStart.TabIndex = 1;
            this.WindowsStart.Text = "Start with Windows";
            this.WindowsStart.UseVisualStyleBackColor = true;
            this.WindowsStart.CheckedChanged += new System.EventHandler(this.WindowsStart_CheckedChanged);
            // 
            // SetPath
            // 
            this.SetPath.Location = new System.Drawing.Point(146, 9);
            this.SetPath.Name = "SetPath";
            this.SetPath.Size = new System.Drawing.Size(176, 23);
            this.SetPath.TabIndex = 2;
            this.SetPath.Text = "Set CS:GO path";
            this.SetPath.UseVisualStyleBackColor = true;
            this.SetPath.Click += new System.EventHandler(this.SetPath_Click);
            // 
            // ForceRecord
            // 
            this.ForceRecord.Location = new System.Drawing.Point(12, 208);
            this.ForceRecord.Name = "ForceRecord";
            this.ForceRecord.Size = new System.Drawing.Size(310, 23);
            this.ForceRecord.TabIndex = 3;
            this.ForceRecord.Text = "Force record";
            this.ForceRecord.UseVisualStyleBackColor = true;
            this.ForceRecord.Click += new System.EventHandler(this.ForceRecord_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 241);
            this.Controls.Add(this.ForceRecord);
            this.Controls.Add(this.SetPath);
            this.Controls.Add(this.WindowsStart);
            this.Controls.Add(this.LogOutput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "wvw_autodemo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox LogOutput;
        public System.Windows.Forms.Button SetPath;
        private System.Windows.Forms.CheckBox WindowsStart;
        private System.Windows.Forms.Button ForceRecord;
    }
}

