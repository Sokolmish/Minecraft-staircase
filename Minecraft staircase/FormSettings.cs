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
            checkBox2.Checked = propeteries.HideTips;
            comboBox2.Text = comboBox2.Items[propeteries.ConvertingMethod] as string;
            comboBox3.Text = comboBox3.Items[propeteries.ConvertingMethod] as string;

            switch (propeteries.Language)
            {
                case "en":
                    comboBox1.SelectedItem = "English en";
                    break;
                case "ru-RU":
                    comboBox1.SelectedItem = "Русский ru-RU";
                    break;
            }

            label3.Visible = false;
            comboBox3.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            propeteries.HideTips = checkBox2.Checked;
            propeteries.ConvertingMethod = comboBox2.SelectedIndex >= 0 ? (byte)comboBox2.SelectedIndex : propeteries.ConvertingMethod;
            propeteries.GeneratingMethod = comboBox3.SelectedIndex >= 0 ? (byte)comboBox3.SelectedIndex : propeteries.GeneratingMethod;
            if ((comboBox1.SelectedItem as string)?.Split(' ')[1] != Properties.Settings.Default.Language)
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

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
