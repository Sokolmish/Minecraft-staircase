using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Minecraft_staircase
{
    public partial class FormSelectMaterials : Form
    {
        const string BlockIDS = @"data\PossibleBlocks.txt";

        bool admin;

        List<ColorNote> colorsList;

        public FormSelectMaterials()
        {
            InitializeComponent();
            Height = 187;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Properties.Resources.HelpSelectingMaterials;
        }

        #region admin
        private void pictureBoxTexture_DoubleClick(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 4)
                admin = true;
        }

        private void pictureBoxColors_DoubleClick(object sender, EventArgs e)
        {
            if (admin && comboBox1.SelectedIndex == 4)
                Height = 325;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //if (textBoxName.Text != null && textBoxTexture.Text != null && int.TryParse(textBoxID.Text, out int id) && int.TryParse(textBoxData.Text, out int data))
            //{
            //    BlockData[] temp = possibleBlocks[comboBox1.SelectedIndex];
            //    possibleBlocks[comboBox1.SelectedIndex] = new BlockData[temp.Length + 1];
            //    temp.CopyTo(possibleBlocks[comboBox1.SelectedIndex], 0);
            //    possibleBlocks[comboBox1.SelectedIndex][possibleBlocks[comboBox1.SelectedIndex].Length - 1] = 
            //        new BlockData(comboBox1.SelectedIndex, textBoxTexture.Text, textBoxName.Text, id, data, checkBox1.Checked);
            //    comboBox2.Items.Clear();
            //    foreach (BlockData block in possibleBlocks[comboBox1.SelectedIndex])
            //        comboBox2.Items.Add(block.Name);
            //    comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
            //    WriteFile();
            //}
        }
        #endregion

        public void ShowDialog(ref List<ColorNote> colorsList)
        {
            this.colorsList = colorsList;
            foreach (ColorNote note in colorsList)
                comboBox1.Items.Add(note.SelectedBlock.Name);
            foreach (BlockData block in colorsList[0].PossibleBlocks)
                comboBox2.Items.Add(block.Name);
            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();
            button4.Text = colorsList[0].Use ? "Do not use" : "Use";

            textBoxName.Text = colorsList[0].SelectedBlock.Name;
            textBoxTexture.Text = colorsList[0].SelectedBlock.TextureName;
            textBoxID.Text = colorsList[0].SelectedBlock.ID.ToString();
            textBoxData.Text = colorsList[0].SelectedBlock.Data.ToString();
            checkBox1.Checked = colorsList[0].SelectedBlock.IsTransparent;
            pictureBoxTexture.Image = Image.FromFile($@"data\Textures\{colorsList[0].SelectedBlock.TextureName}");

            pictureBoxColors.Image = new Bitmap(pictureBoxColors.Width, pictureBoxColors.Height);
            Graphics graph = Graphics.FromImage(pictureBoxColors.Image);
            graph.FillRectangle(new SolidBrush(colorsList[0].DarkColor), new Rectangle(0, 0, 50, 32));
            graph.FillRectangle(new SolidBrush(colorsList[0].NormalColor), new Rectangle(50, 0, 50, 32));
            graph.FillRectangle(new SolidBrush(colorsList[0].LightColor), new Rectangle(100, 0, 50, 32));
            Show();
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            comboBox2.Items.Clear();
            foreach (BlockData block in colorsList[index].PossibleBlocks)
                comboBox2.Items.Add(block.Name);
            comboBox2.Text = comboBox2.Items[0].ToString();

            textBoxName.Text = colorsList[index].SelectedBlock.Name;
            textBoxTexture.Text = colorsList[index].SelectedBlock.TextureName;
            textBoxID.Text = colorsList[index].SelectedBlock.ID.ToString();
            textBoxData.Text = colorsList[index].SelectedBlock.Data.ToString();
            checkBox1.Checked = colorsList[index].SelectedBlock.IsTransparent;
            button4.Text = colorsList[index].Use ? "Do not use" : "Use";
            pictureBoxTexture.Image = Image.FromFile($@"data\Textures\{colorsList[index].SelectedBlock.TextureName}");

            pictureBoxColors.Image = new Bitmap(pictureBoxColors.Width, pictureBoxColors.Height);
            Graphics graph = Graphics.FromImage(pictureBoxColors.Image);
            graph.FillRectangle(new SolidBrush(colorsList[index].DarkColor), new Rectangle(0, 0, 50, 32));
            graph.FillRectangle(new SolidBrush(colorsList[index].NormalColor), new Rectangle(50, 0, 50, 32));
            graph.FillRectangle(new SolidBrush(colorsList[index].LightColor), new Rectangle(100, 0, 50, 32));
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxTexture.Image = Image.FromFile($@"data\Textures\{colorsList[comboBox1.SelectedIndex].PossibleBlocks[comboBox2.SelectedIndex].TextureName}");
            textBoxName.Text = colorsList[comboBox1.SelectedIndex].PossibleBlocks[comboBox2.SelectedIndex].Name;
            textBoxTexture.Text = colorsList[comboBox1.SelectedIndex].PossibleBlocks[comboBox2.SelectedIndex].TextureName;
            textBoxID.Text = colorsList[comboBox1.SelectedIndex].PossibleBlocks[comboBox2.SelectedIndex].ID.ToString();
            textBoxData.Text = colorsList[comboBox1.SelectedIndex].PossibleBlocks[comboBox2.SelectedIndex].Data.ToString();
            checkBox1.Checked = colorsList[comboBox1.SelectedIndex].PossibleBlocks[comboBox2.SelectedIndex].IsTransparent;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            colorsList[comboBox1.SelectedIndex].SelectedBlock = colorsList[comboBox1.SelectedIndex].PossibleBlocks[comboBox2.SelectedIndex];
            comboBox1.Text = colorsList[comboBox1.SelectedIndex].SelectedBlock.Name;
            comboBox1.Items[comboBox1.SelectedIndex] = colorsList[comboBox1.SelectedIndex].SelectedBlock.Name;
            WriteFile();
        }
       

        void WriteFile()
        {
            string save = string.Empty;
            foreach (ColorNote col in colorsList)
                save += col.DataToString() + "\r\n";
            Properties.Settings.Default.PossibleBlocks = save;
            Properties.Settings.Default.Save();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                textBoxTexture.Text = openFileDialog1.SafeFileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            colorsList[comboBox1.SelectedIndex].Use = !colorsList[comboBox1.SelectedIndex].Use;
            button4.Text = colorsList[comboBox1.SelectedIndex].Use ? "Do not use" : "Use";
            WriteFile();
        }


        private void FormSelectMaterials_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            FormSelectMaterials_HelpRequested(new object(), new HelpEventArgs(new Point(0, 0)));
        }

        private void FormSelectMaterials_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            pictureBox1.Visible = true;
            Size = new Size(549, 419);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Size = new Size(264, 187);
            pictureBox1.Visible = false;
        }
    }
}
