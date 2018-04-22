using System;
using System.Windows.Forms;
using System.Drawing;

namespace Minecraft_staircase
{
    public partial class FormSettings : Form
    {
        Properties.Settings properties;

        Color defMeshColor;
        Color chunkMeshColor;
        Color mapMeshColor;

        public FormSettings()
        {
            InitializeComponent();
            properties = Properties.Settings.Default;
            checkBox2.Checked = properties.HideTips;
            comboBox2.Text = comboBox2.Items[properties.ConvertingMethod] as string;
            comboBox3.Text = comboBox3.Items[properties.ConvertingMethod] as string;

            switch (properties.Language)
            {
                case "en":
                    comboBox1.SelectedItem = "English en";
                    break;
                case "ru-RU":
                    comboBox1.SelectedItem = "Русский ru-RU";
                    break;
            }

            defMeshColor = properties.defMeshColor;
            chunkMeshColor = properties.chunkMeshColor;
            mapMeshColor = properties.mapMeshColor;

            panel1.BackColor = defMeshColor;
            textBox1.Text = defMeshColor.IsNamedColor ? defMeshColor.Name :
                $"{defMeshColor.R}; {defMeshColor.G}; {defMeshColor.B}";
            panel2.BackColor = chunkMeshColor;
            textBox2.Text = chunkMeshColor.IsNamedColor ? chunkMeshColor.Name :
                $"{chunkMeshColor.R}; {chunkMeshColor.G}; {chunkMeshColor.B}";
            panel3.BackColor = mapMeshColor;
            textBox3.Text = mapMeshColor.IsNamedColor ? mapMeshColor.Name :
                $"{mapMeshColor.R}; {mapMeshColor.G}; {mapMeshColor.B}";

            label3.Visible = false;
            comboBox3.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            properties.HideTips = checkBox2.Checked;
            properties.ConvertingMethod = comboBox2.SelectedIndex >= 0 ? (byte)comboBox2.SelectedIndex : properties.ConvertingMethod;
            properties.GeneratingMethod = comboBox3.SelectedIndex >= 0 ? (byte)comboBox3.SelectedIndex : properties.GeneratingMethod;

            properties.defMeshColor = defMeshColor;
            properties.chunkMeshColor = chunkMeshColor;
            properties.mapMeshColor = mapMeshColor;

            if ((comboBox1.SelectedItem as string)?.Split(' ')[1] != Properties.Settings.Default.Language)
            {
                properties.Language = "en";
                properties.Language = (comboBox1.SelectedItem as string)?.Split(' ')[1];
                properties.Save();
                System.Diagnostics.Process.Start("Minecraft staircase");
                Environment.Exit(0);
            }
            properties.Save();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                defMeshColor = dialog.Color;
                panel1.BackColor = defMeshColor;
                textBox1.Text = defMeshColor.IsNamedColor ? defMeshColor.Name :
                    $"{defMeshColor.R}; {defMeshColor.G}; {defMeshColor.B}";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                chunkMeshColor = dialog.Color;
                panel2.BackColor = chunkMeshColor;
                textBox2.Text = chunkMeshColor.IsNamedColor ? chunkMeshColor.Name :
                    $"{chunkMeshColor.R}; {chunkMeshColor.G}; {chunkMeshColor.B}";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mapMeshColor = dialog.Color;
                panel3.BackColor = mapMeshColor;
                textBox3.Text = mapMeshColor.IsNamedColor ? mapMeshColor.Name :
                    $"{mapMeshColor.R}; {mapMeshColor.G}; {mapMeshColor.B}";
            }
        }
    }
}
