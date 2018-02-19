using System;
using System.Windows.Forms;

namespace Minecraft_staircase
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Language = (comboBox1.SelectedItem as string).Split(' ')[1];
            Properties.Settings.Default.Save();
            System.Diagnostics.Process.Start("Minecraft staircase");
            Environment.Exit(0);
        }
    }
}
