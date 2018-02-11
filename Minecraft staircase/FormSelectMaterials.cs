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

        List<PixelData> colorsList;
        List<BlockData[]> possibleBlocks;
        List<bool> use;

        public FormSelectMaterials()
        {
            InitializeComponent();
            Height = 187;
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
        #endregion

        public void ShowDialog(List<PixelData> colorsList)
        {
            this.colorsList = colorsList;
            possibleBlocks = new List<BlockData[]>();
            use = new List<bool>();
            int ColorID = 0;
            using (FileStream fs = new FileStream(BlockIDS, FileMode.Open))
            {
                StreamReader reader = new StreamReader(fs);
                string line = reader.ReadLine();
                while (line != null)
                {
                    string[] temp = line.Split('~');
                    use.Add(temp[0] == "True");
                    temp = temp[1].Split(',');
                    possibleBlocks.Add(new BlockData[temp.Length]);
                    ++ColorID;
                    for (int i = 0; i < temp.Length; i++)
                    {
                        string[] tempBlock = temp[i].Split('-');
                        possibleBlocks[possibleBlocks.Count - 1][i] = new BlockData
                            (ColorID, tempBlock[0], tempBlock[1], Convert.ToInt32(tempBlock[2]), Convert.ToInt32(tempBlock[3]), tempBlock[4] == "True");
                    }
                    line = reader.ReadLine();
                }
            }

            foreach (BlockData[] bd in possibleBlocks)
                comboBox1.Items.Add(bd[0].Name);
            foreach (BlockData block in possibleBlocks[0])
                comboBox2.Items.Add(block.Name);
            comboBox1.Text = comboBox1.Items[0].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();
            button4.Text = use[0] ? "Do not use" : "Use";

            textBoxName.Text = possibleBlocks[0][0].Name;
            textBoxTexture.Text = possibleBlocks[0][0].TextureName;
            textBoxID.Text = possibleBlocks[0][0].ID.ToString();
            textBoxData.Text = possibleBlocks[0][0].Data.ToString();
            checkBox1.Checked = possibleBlocks[0][0].IsTransparent;
            pictureBoxTexture.Image = Image.FromFile($@"data\Textures\{possibleBlocks[0][0].TextureName}");

            pictureBoxColors.Image = new Bitmap(pictureBoxColors.Width, pictureBoxColors.Height);
            Graphics graph = Graphics.FromImage(pictureBoxColors.Image);
            Color dark = Color.FromArgb(colorsList[0].DarkColor[0], colorsList[0].DarkColor[1], colorsList[0].DarkColor[2]);
            Color norm = Color.FromArgb(colorsList[0].NormalColor[0], colorsList[0].NormalColor[1], colorsList[0].NormalColor[2]);
            Color light = Color.FromArgb(colorsList[0].LightColor[0], colorsList[0].LightColor[1], colorsList[0].LightColor[2]);
            graph.FillRectangle(new SolidBrush(dark), new Rectangle(0, 0, 50, 32));
            graph.FillRectangle(new SolidBrush(norm), new Rectangle(50, 0, 50, 32));
            graph.FillRectangle(new SolidBrush(light), new Rectangle(100, 0, 50, 32));
            Show();
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            comboBox2.Items.Clear();
            foreach (BlockData block in possibleBlocks[index])
                comboBox2.Items.Add(block.Name);
            comboBox2.Text = comboBox2.Items[0].ToString();

            textBoxName.Text = possibleBlocks[index][0].Name;
            textBoxTexture.Text = possibleBlocks[index][0].TextureName;
            textBoxID.Text = possibleBlocks[index][0].ID.ToString();
            textBoxData.Text = possibleBlocks[index][0].Data.ToString();
            checkBox1.Checked = possibleBlocks[index][0].IsTransparent;
            button4.Text = use[index] ? "Do not use" : "Use";
            pictureBoxTexture.Image = Image.FromFile($@"data\Textures\{possibleBlocks[index][0].TextureName}");

            pictureBoxColors.Image = new Bitmap(pictureBoxColors.Width, pictureBoxColors.Height);
            Graphics graph = Graphics.FromImage(pictureBoxColors.Image);
            Color dark = Color.FromArgb(colorsList[index].DarkColor[0], colorsList[index].DarkColor[1], colorsList[index].DarkColor[2]);
            Color norm = Color.FromArgb(colorsList[index].NormalColor[0], colorsList[index].NormalColor[1], colorsList[index].NormalColor[2]);
            Color light = Color.FromArgb(colorsList[index].LightColor[0], colorsList[index].LightColor[1], colorsList[index].LightColor[2]);
            graph.FillRectangle(new SolidBrush(dark), new Rectangle(0, 0, 50, 32));
            graph.FillRectangle(new SolidBrush(norm), new Rectangle(50, 0, 50, 32));
            graph.FillRectangle(new SolidBrush(light), new Rectangle(100, 0, 50, 32));
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxTexture.Image = Image.FromFile($@"data\Textures\{possibleBlocks[comboBox1.SelectedIndex][comboBox2.SelectedIndex].TextureName}");
            textBoxName.Text = possibleBlocks[comboBox1.SelectedIndex][comboBox2.SelectedIndex].Name;
            textBoxTexture.Text = possibleBlocks[comboBox1.SelectedIndex][comboBox2.SelectedIndex].TextureName;
            textBoxID.Text = possibleBlocks[comboBox1.SelectedIndex][comboBox2.SelectedIndex].ID.ToString();
            textBoxData.Text = possibleBlocks[comboBox1.SelectedIndex][comboBox2.SelectedIndex].Data.ToString();
            checkBox1.Checked = possibleBlocks[comboBox1.SelectedIndex][comboBox2.SelectedIndex].IsTransparent;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Swap(ref possibleBlocks[comboBox1.SelectedIndex][0], ref possibleBlocks[comboBox1.SelectedIndex][comboBox2.SelectedIndex]);
            comboBox1.Text = possibleBlocks[comboBox1.SelectedIndex][0].Name;
            comboBox1.Items[comboBox1.SelectedIndex] = possibleBlocks[comboBox1.SelectedIndex][0].Name;
            WriteFile();
        }

        void Swap (ref BlockData obj1, ref BlockData obj2)
        {
            BlockData temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text != null && textBoxTexture.Text != null && int.TryParse(textBoxID.Text, out int id) && int.TryParse(textBoxData.Text, out int data))
            {
                BlockData[] temp = possibleBlocks[comboBox1.SelectedIndex];
                possibleBlocks[comboBox1.SelectedIndex] = new BlockData[temp.Length + 1];
                temp.CopyTo(possibleBlocks[comboBox1.SelectedIndex], 0);
                possibleBlocks[comboBox1.SelectedIndex][possibleBlocks[comboBox1.SelectedIndex].Length - 1] = 
                    new BlockData(comboBox1.SelectedIndex, textBoxTexture.Text, textBoxName.Text, id, data, checkBox1.Checked);
                comboBox2.Items.Clear();
                foreach (BlockData block in possibleBlocks[comboBox1.SelectedIndex])
                    comboBox2.Items.Add(block.Name);
                comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
                WriteFile();
            }
        }

        void WriteFile()
        {
            using (FileStream fs = new FileStream(BlockIDS, FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(fs);
                int index = 0;
                foreach (BlockData[] bd in possibleBlocks)
                {
                    writer.Write($"{use[index++]}~{bd[0].TextureName}-{bd[0].Name}-{bd[0].ID}-{bd[0].Data}-{bd[0].IsTransparent.ToString()}");
                    bool skip = true;
                    foreach (BlockData block in bd)
                    {
                        if (skip)
                        {
                            skip = false;
                            continue;
                        }
                        writer.Write($",{block.TextureName}-{block.Name}-{block.ID}-{block.Data}-{block.IsTransparent.ToString()}");
                    }
                    writer.Write("\r\n");
                    writer.Flush();
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                textBoxTexture.Text = openFileDialog1.SafeFileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            use[comboBox1.SelectedIndex] = !use[comboBox1.SelectedIndex];
            button4.Text = use[comboBox1.SelectedIndex] ? "Do not use" : "Use";
            WriteFile();
        }
    }
}
