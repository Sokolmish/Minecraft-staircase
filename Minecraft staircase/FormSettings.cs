using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Minecraft_staircase
{
    public partial class FormSettings : Form
    {
        Properties.Settings properties;

        Color defMeshColor;
        Color chunkMeshColor;
        Color mapMeshColor;

        Dictionary<string, string> langsList = new Dictionary<string, string>()
        {
            {"en", "English" },
            {"ru-RU", "Русский" }
        };

        public FormSettings()
        {
            InitializeComponent();
            properties = Properties.Settings.Default;
            checkBox2.Checked = properties.HideTips;
            comboBox2.Text = comboBox2.Items[properties.ConvertingMethod] as string;
            comboBox3.Text = comboBox3.Items[properties.GeneratingMethod] as string;

            foreach (var str in langsList)
                comboBox1.Items.Add(str.Value);
            comboBox1.SelectedItem = langsList[properties.Language];

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
        }

        private void ApplyButtonClick(object sender, EventArgs e)
        {            
            properties.HideTips = checkBox2.Checked;
            properties.ConvertingMethod = comboBox2.SelectedIndex >= 0 ? (byte)comboBox2.SelectedIndex : properties.ConvertingMethod;
            properties.GeneratingMethod = comboBox3.SelectedIndex >= 0 ? (byte)comboBox3.SelectedIndex : properties.GeneratingMethod;

            properties.defMeshColor = defMeshColor;
            properties.chunkMeshColor = chunkMeshColor;
            properties.mapMeshColor = mapMeshColor;

            if (LangByName(comboBox1.SelectedItem as string) != properties.Language)
            {
                properties.Language = LangByName(comboBox1.SelectedItem as string);
                properties.Save();
                System.Diagnostics.Process.Start("Minecraft staircase");
                Environment.Exit(0);
            }
            properties.Save();
            Close();
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }


        private string LangByName(string name)
        {
            foreach (var str in langsList)
                if (str.Value == name)
                    return str.Key;
            return null;
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(Lang.GetHint("ConvertionDesc"), "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(Lang.GetHint("GeneratingDesc"), "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
