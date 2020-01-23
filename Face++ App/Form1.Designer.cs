using System.Drawing;
using System.Windows.Forms;

namespace Facial_Recognition_Smart_Alarm
{

    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.Monitor = new System.Windows.Forms.PictureBox();
            this.VideoDevices = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.AddNewGB = new System.Windows.Forms.GroupBox();
            this.DecFacLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.FaceNameTextBox = new System.Windows.Forms.TextBox();
            this.AddFaceButton = new System.Windows.Forms.Button();
            this.SnapFaceButton = new System.Windows.Forms.Button();
            this.PreviewBox = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TrustedFacesList = new System.Windows.Forms.DataGridView();
            this.SlNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PersonName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemoveOption = new System.Windows.Forms.DataGridViewButtonColumn();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.FrontDoorGB = new System.Windows.Forms.GroupBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.RingBell = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.MonitorControls = new System.Windows.Forms.GroupBox();
            this.StartMonitorButton = new System.Windows.Forms.Button();
            this.StopMonitorButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ArduinoStatus = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.Monitor)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.AddNewGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrustedFacesList)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.FrontDoorGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.MonitorControls.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(488, 22);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(221, 28);
            this.button1.TabIndex = 0;
            this.button1.Text = "Refresh Camera Devices";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // Monitor
            // 
            this.Monitor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Monitor.Location = new System.Drawing.Point(13, 62);
            this.Monitor.Margin = new System.Windows.Forms.Padding(4);
            this.Monitor.Name = "Monitor";
            this.Monitor.Size = new System.Drawing.Size(695, 500);
            this.Monitor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.Monitor.TabIndex = 2;
            this.Monitor.TabStop = false;
            // 
            // VideoDevices
            // 
            this.VideoDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoDevices.FormattingEnabled = true;
            this.VideoDevices.Location = new System.Drawing.Point(13, 23);
            this.VideoDevices.Margin = new System.Windows.Forms.Padding(4);
            this.VideoDevices.Name = "VideoDevices";
            this.VideoDevices.Size = new System.Drawing.Size(465, 24);
            this.VideoDevices.TabIndex = 88;
            this.VideoDevices.SelectedIndexChanged += new System.EventHandler(this.VideoDevices_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Monitor);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.VideoDevices);
            this.groupBox1.Location = new System.Drawing.Point(8, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(725, 576);
            this.groupBox1.TabIndex = 89;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Face ID";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.AddNewGB);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(469, 540);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Add/Remove Faces";
            // 
            // AddNewGB
            // 
            this.AddNewGB.Controls.Add(this.DecFacLabel);
            this.AddNewGB.Controls.Add(this.NameLabel);
            this.AddNewGB.Controls.Add(this.FaceNameTextBox);
            this.AddNewGB.Controls.Add(this.AddFaceButton);
            this.AddNewGB.Controls.Add(this.SnapFaceButton);
            this.AddNewGB.Controls.Add(this.PreviewBox);
            this.AddNewGB.Location = new System.Drawing.Point(8, 7);
            this.AddNewGB.Margin = new System.Windows.Forms.Padding(4);
            this.AddNewGB.Name = "AddNewGB";
            this.AddNewGB.Padding = new System.Windows.Forms.Padding(4);
            this.AddNewGB.Size = new System.Drawing.Size(451, 319);
            this.AddNewGB.TabIndex = 2;
            this.AddNewGB.TabStop = false;
            this.AddNewGB.Text = "Add New";
            // 
            // DecFacLabel
            // 
            this.DecFacLabel.AutoSize = true;
            this.DecFacLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.DecFacLabel.Location = new System.Drawing.Point(152, 145);
            this.DecFacLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DecFacLabel.Name = "DecFacLabel";
            this.DecFacLabel.Size = new System.Drawing.Size(131, 20);
            this.DecFacLabel.TabIndex = 93;
            this.DecFacLabel.Text = "Detecting Face..";
            this.DecFacLabel.Visible = false;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Enabled = false;
            this.NameLabel.Location = new System.Drawing.Point(8, 261);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(98, 17);
            this.NameLabel.TabIndex = 92;
            this.NameLabel.Text = "Person Name ";
            // 
            // FaceNameTextBox
            // 
            this.FaceNameTextBox.Enabled = false;
            this.FaceNameTextBox.Location = new System.Drawing.Point(11, 282);
            this.FaceNameTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.FaceNameTextBox.Name = "FaceNameTextBox";
            this.FaceNameTextBox.Size = new System.Drawing.Size(316, 22);
            this.FaceNameTextBox.TabIndex = 0;
            // 
            // AddFaceButton
            // 
            this.AddFaceButton.Enabled = false;
            this.AddFaceButton.Location = new System.Drawing.Point(336, 262);
            this.AddFaceButton.Margin = new System.Windows.Forms.Padding(4);
            this.AddFaceButton.Name = "AddFaceButton";
            this.AddFaceButton.Size = new System.Drawing.Size(105, 47);
            this.AddFaceButton.TabIndex = 90;
            this.AddFaceButton.Text = "Add Face";
            this.AddFaceButton.UseVisualStyleBackColor = true;
            this.AddFaceButton.Click += new System.EventHandler(this.AddFaceButton_Click);
            // 
            // SnapFaceButton
            // 
            this.SnapFaceButton.Location = new System.Drawing.Point(11, 20);
            this.SnapFaceButton.Margin = new System.Windows.Forms.Padding(4);
            this.SnapFaceButton.Name = "SnapFaceButton";
            this.SnapFaceButton.Size = new System.Drawing.Size(431, 39);
            this.SnapFaceButton.TabIndex = 89;
            this.SnapFaceButton.Text = "Snap Face";
            this.SnapFaceButton.UseVisualStyleBackColor = true;
            this.SnapFaceButton.Click += new System.EventHandler(this.SnapFaceButton_Click);
            // 
            // PreviewBox
            // 
            this.PreviewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PreviewBox.Location = new System.Drawing.Point(11, 69);
            this.PreviewBox.Margin = new System.Windows.Forms.Padding(4);
            this.PreviewBox.Name = "PreviewBox";
            this.PreviewBox.Size = new System.Drawing.Size(430, 180);
            this.PreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PreviewBox.TabIndex = 89;
            this.PreviewBox.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TrustedFacesList);
            this.groupBox2.Location = new System.Drawing.Point(8, 334);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(451, 201);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trusted Faces";
            // 
            // TrustedFacesList
            // 
            this.TrustedFacesList.AllowUserToAddRows = false;
            this.TrustedFacesList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.TrustedFacesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TrustedFacesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SlNo,
            this.PersonName,
            this.RemoveOption});
            this.TrustedFacesList.Location = new System.Drawing.Point(11, 23);
            this.TrustedFacesList.Margin = new System.Windows.Forms.Padding(4);
            this.TrustedFacesList.Name = "TrustedFacesList";
            this.TrustedFacesList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.TrustedFacesList.RowHeadersVisible = false;
            this.TrustedFacesList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.TrustedFacesList.Size = new System.Drawing.Size(431, 170);
            this.TrustedFacesList.TabIndex = 0;
            // 
            // SlNo
            // 
            this.SlNo.Frozen = true;
            this.SlNo.HeaderText = "#";
            this.SlNo.MinimumWidth = 6;
            this.SlNo.Name = "SlNo";
            this.SlNo.ReadOnly = true;
            this.SlNo.Width = 38;
            // 
            // PersonName
            // 
            this.PersonName.Frozen = true;
            this.PersonName.HeaderText = "Name";
            this.PersonName.MinimumWidth = 6;
            this.PersonName.Name = "PersonName";
            this.PersonName.ReadOnly = true;
            this.PersonName.Width = 210;
            // 
            // RemoveOption
            // 
            this.RemoveOption.Frozen = true;
            this.RemoveOption.HeaderText = "";
            this.RemoveOption.MinimumWidth = 6;
            this.RemoveOption.Name = "RemoveOption";
            this.RemoveOption.ReadOnly = true;
            this.RemoveOption.Width = 70;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.FrontDoorGB);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.MonitorControls);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(469, 540);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Security Status";
            // 
            // FrontDoorGB
            //
            this.FrontDoorGB.Controls.Add(this.pictureBox3);
            this.FrontDoorGB.Controls.Add(this.RingBell);
            this.FrontDoorGB.Enabled = false;
            this.FrontDoorGB.Location = new System.Drawing.Point(23, 424);
            this.FrontDoorGB.Margin = new System.Windows.Forms.Padding(4);
            this.FrontDoorGB.Name = "FrontDoorGB";
            this.FrontDoorGB.Padding = new System.Windows.Forms.Padding(4);
            this.FrontDoorGB.Size = new System.Drawing.Size(423, 106);
            this.FrontDoorGB.TabIndex = 94;
            this.FrontDoorGB.TabStop = false;
            this.FrontDoorGB.Text = "Front Door";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox3.Location = new System.Drawing.Point(63, 44);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(56, 38);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 93;
            this.pictureBox3.TabStop = false;
            // 
            // RingBell
            // 
            this.RingBell.Location = new System.Drawing.Point(127, 37);
            this.RingBell.Margin = new System.Windows.Forms.Padding(4);
            this.RingBell.Name = "RingBell";
            this.RingBell.Size = new System.Drawing.Size(231, 50);
            this.RingBell.TabIndex = 89;
            this.RingBell.Text = "Ding Dong!";
            this.RingBell.UseVisualStyleBackColor = true;
            this.RingBell.Click += new System.EventHandler(this.RingBell_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.StatusLabel);
            this.groupBox4.Location = new System.Drawing.Point(23, 174);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(423, 112);
            this.groupBox4.TabIndex = 93;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Status";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.StatusLabel.ForeColor = System.Drawing.Color.Red;
            this.StatusLabel.Location = new System.Drawing.Point(8, 31);
            this.StatusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(407, 42);
            this.StatusLabel.TabIndex = 93;
            this.StatusLabel.Text = "NOT RUNNING";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MonitorControls
            // 
            this.MonitorControls.Controls.Add(this.StartMonitorButton);
            this.MonitorControls.Controls.Add(this.StopMonitorButton);
            this.MonitorControls.Location = new System.Drawing.Point(23, 18);
            this.MonitorControls.Margin = new System.Windows.Forms.Padding(4);
            this.MonitorControls.Name = "MonitorControls";
            this.MonitorControls.Padding = new System.Windows.Forms.Padding(4);
            this.MonitorControls.Size = new System.Drawing.Size(423, 138);
            this.MonitorControls.TabIndex = 92;
            this.MonitorControls.TabStop = false;
            this.MonitorControls.Text = "Face Monitoring";
            // 
            // StartMonitorButton
            // 
            this.StartMonitorButton.Location = new System.Drawing.Point(49, 62);
            this.StartMonitorButton.Margin = new System.Windows.Forms.Padding(4);
            this.StartMonitorButton.Name = "StartMonitorButton";
            this.StartMonitorButton.Size = new System.Drawing.Size(159, 49);
            this.StartMonitorButton.TabIndex = 90;
            this.StartMonitorButton.Text = "Start";
            this.StartMonitorButton.UseVisualStyleBackColor = true;
            this.StartMonitorButton.Click += new System.EventHandler(this.StartMonitorButton_Click_1);
            // 
            // StopMonitorButton
            // 
            this.StopMonitorButton.Enabled = false;
            this.StopMonitorButton.Location = new System.Drawing.Point(216, 62);
            this.StopMonitorButton.Margin = new System.Windows.Forms.Padding(4);
            this.StopMonitorButton.Name = "StopMonitorButton";
            this.StopMonitorButton.Size = new System.Drawing.Size(159, 49);
            this.StopMonitorButton.TabIndex = 91;
            this.StopMonitorButton.Text = "Stop";
            this.StopMonitorButton.UseVisualStyleBackColor = true;
            this.StopMonitorButton.Click += new System.EventHandler(this.StopMonitorButton_Click_1);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(741, 15);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(477, 572);
            this.tabControl1.TabIndex = 90;
            // 
            // groupBox3 
            //  
            this.groupBox3.Controls.Add(this.ArduinoStatus);
            this.groupBox3.Location = new System.Drawing.Point(23, 294);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(423, 112);
            this.groupBox3.TabIndex = 94;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Arduino Status";
            //  
            // ArduinoStatus 
            //  
            this.ArduinoStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.ArduinoStatus.ForeColor = System.Drawing.Color.Red;
            this.ArduinoStatus.Location = new System.Drawing.Point(8, 31);
            this.ArduinoStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ArduinoStatus.Name = "ArduinoStatus";
            this.ArduinoStatus.Size = new System.Drawing.Size(407, 42);
            this.ArduinoStatus.TabIndex = 93;
            this.ArduinoStatus.Text = "NOT CONNECTED";
            this.ArduinoStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //  
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1224, 594);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Face++ Recognition";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Monitor)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.AddNewGB.ResumeLayout(false);
            this.AddNewGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrustedFacesList)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.FrontDoorGB.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.MonitorControls.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox Monitor;
        private System.Windows.Forms.ComboBox VideoDevices;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox AddNewGB;
        private System.Windows.Forms.Label DecFacLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox FaceNameTextBox;
        private System.Windows.Forms.Button AddFaceButton;
        private System.Windows.Forms.Button SnapFaceButton;
        private System.Windows.Forms.PictureBox PreviewBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView TrustedFacesList;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn PersonName;
        private System.Windows.Forms.DataGridViewButtonColumn RemoveOption;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox FrontDoorGB;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button RingBell;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.GroupBox MonitorControls;
        private System.Windows.Forms.Button StartMonitorButton;
        private System.Windows.Forms.Button StopMonitorButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label ArduinoStatus;
    }
}

