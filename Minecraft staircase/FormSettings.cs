using System;
using System.Windows.Forms;

namespace Minecraft_staircase
{
    public partial class FormSettings : Form
    {
        Properties.Settings propeteries;

        public FormSettings()
        {
            InitializeComponent();
            propeteries = Properties.Settings.Default;
            checkBox1.Checked = propeteries.LimitedHeight;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            propeteries.LimitedHeight = checkBox1.Checked;
            if (comboBox1.SelectedItem as string != Properties.Settings.Default.Language)
            {
                propeteries.Language = "en";
                propeteries.Language = (comboBox1.SelectedItem as string)?.Split(' ')[1];
                propeteries.Save();
                System.Diagnostics.Process.Start("Minecraft staircase");
                Environment.Exit(0);
            }
            propeteries.Save();
            Close();
        }
    }
}
