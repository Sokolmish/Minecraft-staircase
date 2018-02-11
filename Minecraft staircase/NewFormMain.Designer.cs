namespace Minecraft_staircase
{
    partial class NewFormMain
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.MaterialsButton = new System.Windows.Forms.Button();
            this.CreateButton = new System.Windows.Forms.Button();
            this.TopViewButton = new System.Windows.Forms.Button();
            this.CrossViewButton = new System.Windows.Forms.Button();
            this.SchematicButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.UsedMaterialsButton = new System.Windows.Forms.Button();
            this.SettingsButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.FinalImageButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.HelpButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showControlsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(150, 26);
            // 
            // showControlsToolStripMenuItem
            // 
            this.showControlsToolStripMenuItem.Name = "showControlsToolStripMenuItem";
            this.showControlsToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.showControlsToolStripMenuItem.Text = "Show controls";
            this.showControlsToolStripMenuItem.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // OpenButton
            // 
            this.OpenButton.Location = new System.Drawing.Point(12, 12);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(201, 23);
            this.OpenButton.TabIndex = 1;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Width";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(56, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(69, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "1";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(131, 43);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(92, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Custom size";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(56, 67);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(69, 20);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "1";
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Height";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(20, 122);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(49, 17);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Flat";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(72, 122);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(67, 17);
            this.radioButton2.TabIndex = 8;
            this.radioButton2.Text = "3d lite";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(143, 122);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(67, 17);
            this.radioButton3.TabIndex = 9;
            this.radioButton3.Text = "3d full";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // MaterialsButton
            // 
            this.MaterialsButton.Location = new System.Drawing.Point(12, 93);
            this.MaterialsButton.Name = "MaterialsButton";
            this.MaterialsButton.Size = new System.Drawing.Size(201, 23);
            this.MaterialsButton.TabIndex = 10;
            this.MaterialsButton.Text = "Materials options";
            this.MaterialsButton.UseVisualStyleBackColor = true;
            this.MaterialsButton.Click += new System.EventHandler(this.MaterialsButton_Click);
            // 
            // CreateButton
            // 
            this.CreateButton.Enabled = false;
            this.CreateButton.Location = new System.Drawing.Point(12, 145);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(201, 23);
            this.CreateButton.TabIndex = 11;
            this.CreateButton.Text = "Create!";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // TopViewButton
            // 
            this.TopViewButton.Enabled = false;
            this.TopViewButton.Location = new System.Drawing.Point(12, 210);
            this.TopViewButton.Name = "TopViewButton";
            this.TopViewButton.Size = new System.Drawing.Size(201, 23);
            this.TopViewButton.TabIndex = 12;
            this.TopViewButton.Text = "Top view";
            this.TopViewButton.UseVisualStyleBackColor = true;
            this.TopViewButton.Click += new System.EventHandler(this.TopViewButton_Click);
            // 
            // CrossViewButton
            // 
            this.CrossViewButton.Enabled = false;
            this.CrossViewButton.Location = new System.Drawing.Point(12, 239);
            this.CrossViewButton.Name = "CrossViewButton";
            this.CrossViewButton.Size = new System.Drawing.Size(201, 23);
            this.CrossViewButton.TabIndex = 13;
            this.CrossViewButton.Text = "Cross view";
            this.CrossViewButton.UseVisualStyleBackColor = true;
            this.CrossViewButton.Click += new System.EventHandler(this.CrossViewButton_Click);
            // 
            // SchematicButton
            // 
            this.SchematicButton.Enabled = false;
            this.SchematicButton.Location = new System.Drawing.Point(12, 304);
            this.SchematicButton.Name = "SchematicButton";
            this.SchematicButton.Size = new System.Drawing.Size(201, 23);
            this.SchematicButton.TabIndex = 14;
            this.SchematicButton.Text = "Export schematic";
            this.SchematicButton.UseVisualStyleBackColor = true;
            this.SchematicButton.Click += new System.EventHandler(this.SchematicButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Images(*.bmp, *.jpg, *.png, *.gif)|*.bmp;*.jpg;*.png;*.gif|All files|*.*";
            this.openFileDialog1.Title = "Open image";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "schematic";
            this.saveFileDialog1.Filter = "Schematic files|*.schematic|All files|*.*";
            this.saveFileDialog1.Title = "Export schematic";
            // 
            // UsedMaterialsButton
            // 
            this.UsedMaterialsButton.Enabled = false;
            this.UsedMaterialsButton.Location = new System.Drawing.Point(12, 275);
            this.UsedMaterialsButton.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.UsedMaterialsButton.Name = "UsedMaterialsButton";
            this.UsedMaterialsButton.Size = new System.Drawing.Size(201, 23);
            this.UsedMaterialsButton.TabIndex = 16;
            this.UsedMaterialsButton.Text = "View materials";
            this.UsedMaterialsButton.UseVisualStyleBackColor = true;
            this.UsedMaterialsButton.Click += new System.EventHandler(this.UsedMaterialsButton_Click);
            // 
            // SettingsButton
            // 
            this.SettingsButton.Enabled = false;
            this.SettingsButton.Location = new System.Drawing.Point(12, 340);
            this.SettingsButton.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(201, 23);
            this.SettingsButton.TabIndex = 17;
            this.SettingsButton.Text = "Advanced settings";
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(223, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 512);
            this.panel1.TabIndex = 18;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // FinalImageButton
            // 
            this.FinalImageButton.Enabled = false;
            this.FinalImageButton.Location = new System.Drawing.Point(12, 181);
            this.FinalImageButton.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.FinalImageButton.Name = "FinalImageButton";
            this.FinalImageButton.Size = new System.Drawing.Size(201, 23);
            this.FinalImageButton.TabIndex = 15;
            this.FinalImageButton.Text = "Image";
            this.FinalImageButton.UseVisualStyleBackColor = true;
            this.FinalImageButton.Click += new System.EventHandler(this.FinalImageButton_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // HelpButton
            // 
            this.HelpButton.Enabled = false;
            this.HelpButton.Location = new System.Drawing.Point(12, 369);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(201, 23);
            this.HelpButton.TabIndex = 19;
            this.HelpButton.Text = "Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(20, 501);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(193, 23);
            this.progressBar1.TabIndex = 20;
            // 
            // NewFormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 536);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SettingsButton);
            this.Controls.Add(this.UsedMaterialsButton);
            this.Controls.Add(this.FinalImageButton);
            this.Controls.Add(this.SchematicButton);
            this.Controls.Add(this.CrossViewButton);
            this.Controls.Add(this.TopViewButton);
            this.Controls.Add(this.CreateButton);
            this.Controls.Add(this.MaterialsButton);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenButton);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "NewFormMain";
            this.Text = "Minecraft staircase";
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Button MaterialsButton;
        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Button TopViewButton;
        private System.Windows.Forms.Button CrossViewButton;
        private System.Windows.Forms.Button SchematicButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button UsedMaterialsButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showControlsToolStripMenuItem;
        private System.Windows.Forms.Button SettingsButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button FinalImageButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}