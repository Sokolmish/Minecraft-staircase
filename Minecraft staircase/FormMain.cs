using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;
using System.Threading;
using System.Resources;
using Substrate;
using Substrate.ImportExport;

namespace Minecraft_staircase
{
    public partial class FormMain : Form
    {
        const int blockSize = 16;
        const string BlockIDS = @"data\PossibleBlocks.txt";

        List<ColorNote> colorsNote;

        Image originalImage;
        Image rawImage;
        Image convertedImage;

        SettedBlock[,] blockMap;
        int maxHeight;

        Thread convertTask;

        bool noResize;

        public FormMain()
        {
            InitializeComponent();
            string see = Properties.Settings.Default.PossibleBlocks;
            LoadData();
        }


        void LoadData()
        {
            colorsNote = new List<ColorNote>();
            string[] colors = Properties.Resources.ColorsIDS.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] blocks = Properties.Settings.Default.PossibleBlocks.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (colors.Length != blocks.Length)
                throw new Exception("Length of the list of blocks and length of the list of colors did not match");

            for (int i = 0; i < colors.Length; i++)
            {
                string[] curColor = colors[i].Split('-');
                string curBlocks = blocks[i];

                Color lCol = Color.FromArgb(Convert.ToInt32(curColor[1]), Convert.ToInt32(curColor[2]), Convert.ToInt32(curColor[3]));
                Color nCol = Color.FromArgb(lCol.R * 220 / 255, lCol.G * 220 / 255, lCol.B * 220 / 255);
                Color dCol = Color.FromArgb(lCol.R * 180 / 255, lCol.G * 180 / 255, lCol.B * 180 / 255);

                ColorNote note = new ColorNote(Convert.ToInt32(curColor[0]), dCol, nCol, lCol);
                note.Use = curBlocks.Split('~')[0] == "True";

                string[] block = curBlocks.Split('~')[2].Split('-');
                note.SelectedBlock = new BlockData(block[0], block[1], Convert.ToInt32(block[2]), Convert.ToInt32(block[3]), block[4] == "True");

                curBlocks = curBlocks.Split('~')[1];
                List<BlockData> blockDatas = new List<BlockData>();
                foreach (string str in curBlocks.Split(','))
                {
                    block = str.Split('-');
                    blockDatas.Add(new BlockData(block[0], block[1], Convert.ToInt32(block[2]), Convert.ToInt32(block[3]), block[4] == "True"));
                }
                note.PossibleBlocks = blockDatas;

                colorsNote.Add(note);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                originalImage = Image.FromFile(openFileDialog1.FileName);
                if (!checkBox1.Checked)
                {
                    rawImage = new Bitmap(originalImage, originalImage.Width - (originalImage.Width % 128), originalImage.Height - (originalImage.Height % 128));
                    textBox1.Text = (rawImage.Width / 128).ToString();
                    textBox2.Text = (rawImage.Height / 128).ToString();
                }
                else
                {
                    rawImage = originalImage;
                    textBox1.Text = rawImage.Width.ToString();
                    textBox2.Text = rawImage.Height.ToString();
                }
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = rawImage;
                CreateButton.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (originalImage == null)
                return;
            if (!checkBox1.Checked)
            {
                rawImage = new Bitmap(originalImage, originalImage.Width - (originalImage.Width % 128), originalImage.Height - (originalImage.Height % 128));
                textBox1.Text = (rawImage.Width / 128).ToString();
                textBox2.Text = (rawImage.Height / 128).ToString();
            }
            else
            {
                rawImage = originalImage;
                textBox1.Text = rawImage.Width.ToString();
                textBox2.Text = rawImage.Height.ToString();
            }
            pictureBox1.Image = rawImage;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (rawImage != null && int.TryParse(textBox1.Text, out int cur))
            {
                if (!checkBox1.Checked)
                {
                    if (cur < 1)
                    {
                        cur = 1;
                        textBox1.Text = "1";
                    }
                    if (cur > 30)
                    {
                        cur = 30;
                        textBox1.Text = "30";
                    }
                    rawImage = new Bitmap(originalImage, cur * 128, rawImage.Height);
                }
                else
                {
                    if (cur < 1)
                    {
                        cur = 1;
                        textBox1.Text = "1";
                    }
                    if (cur > 3840)
                    {
                        cur = 3840;
                        textBox1.Text = "3840";
                    }
                    rawImage = new Bitmap(originalImage, cur, rawImage.Height);
                }
            }
            pictureBox1.Image = rawImage;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (rawImage != null && int.TryParse(textBox2.Text, out int cur))
            {
                if (!checkBox1.Checked)
                {
                    if (cur < 1)
                    {
                        cur = 1;
                        textBox2.Text = "1";
                    }
                    if (cur > 30)
                    {
                        cur = 30;
                        textBox2.Text = "30";
                    }
                    rawImage = new Bitmap(originalImage, rawImage.Width, cur * 128);
                }
                else
                {
                    if (cur < 1)
                    {
                        cur = 1;
                        textBox1.Text = "1";
                    }
                    if (cur > 3840)
                    {
                        cur = 3840;
                        textBox1.Text = "3840";
                    }
                    rawImage = new Bitmap(originalImage, rawImage.Width, cur);
                }
            }
            pictureBox1.Image = rawImage;
        }


        private void MaterialsButton_Click(object sender, EventArgs e)
        {
            new FormSelectMaterials().ShowDialog(ref colorsNote);
        }


        private void CreateButton_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = rawImage.Width * rawImage.Height;
            progressBar1.Value = 0;
            convertTask?.Abort();
            convertTask = new Thread(() =>
            {
                ArtGenerator gen = new ArtGenerator(ref colorsNote);
                gen.SetProgress(progressBar1);
                convertedImage = rawImage.Clone() as Image; 
                ArtType type = ArtType.Flat;
                if (radioButton2.Checked)
                    type = ArtType.Lite;
                else if (radioButton3.Checked)
                    type = ArtType.Full;
                blockMap = gen.CreateScheme(ref convertedImage, type, out maxHeight);
                pictureBox1.Image = convertedImage;
            });
            convertTask.Start();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (convertTask.ThreadState == ThreadState.Stopped)
            {
                timer1.Stop();
                FinalImageButton.Enabled = true;
                TopViewButton.Enabled = true;
                CrossViewButton.Enabled = true;
                UsedMaterialsButton.Enabled = true;
                SchematicButton.Enabled = true;
                MessageBox.Show("Complete", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }


        private void FinalImageButton_Click(object sender, EventArgs e)
        {
            noResize = true;
            MinimumSize = new Size(0, 0);
            pictureBox1.Width = convertedImage.Width < 256 ? 256 : convertedImage.Width;
            pictureBox1.Height = convertedImage.Height < 256 ? 256 : convertedImage.Height;
            panel1.Width = convertedImage.Width < 256 ? 256 : convertedImage.Width;
            panel1.Height = convertedImage.Height < 256 ? 256 : convertedImage.Height;    
            Width = panel1.Width + 20;
            Height = panel1.Height + 43;
            panel1.Location = new Point(0, 0);
            noResize = false;

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            {
                Size = new Size(763, 575);
                MinimumSize = new Size(763, 575);
                panel1.Size = new Size(512, 512);
                panel1.Location = new Point(223, 12);
            }
        }


        private void TopViewButton_Click(object sender, EventArgs e)
        {
            int[,] ids = new int[blockMap.GetLength(0), blockMap.GetLength(1) - 1];
            for (int i = 0; i < blockMap.GetLength(0); i++)
                for (int j = 1; j < blockMap.GetLength(1); j++)
                    ids[i, j - 1] = blockMap[i, j].ID;
            new FormTopView().Show(ids, ref colorsNote);
        }

        private void CrossViewButton_Click(object sender, EventArgs e)
        {
            new FormCrossView().Show(blockMap, maxHeight + 1, ref colorsNote);
        }

        private void UsedMaterialsButton_Click(object sender, EventArgs e)
        {
            new FormMaterials().Show(ref colorsNote);
        }

        private void SchematicButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Schematic schem = new Schematic(blockMap.GetLength(0), maxHeight + 1, blockMap.GetLength(1));
                for (int i = 0; i < blockMap.GetLength(0); i++)
                    for (int j = 1; j < blockMap.GetLength(1); j++)
                    {
                        ColorNote col = colorsNote.Find((x) => { return x.ColorID == blockMap[i, j].ID; });
                        schem.Blocks.SetBlock(i, blockMap[i, j].Height, j - 1, 
                            new AlphaBlock(col.SelectedBlock.ID, col.SelectedBlock.Data));
                    }                     
                schem.Export(saveFileDialog1.FileName.Contains(".schematic") ? saveFileDialog1.FileName : saveFileDialog1.FileName + ".schematic");
            }
        }


        private void button1_Click_2(object sender, EventArgs e)
        {
            new AboutBox1().Show();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            new FormSettings().ShowDialog();
        }

        private void NewFormMain_Resize(object sender, EventArgs e)
        {
            if (!noResize)
            {
                progressBar1.Location = new Point(progressBar1.Location.X, Height - 74);
                panel1.Width = Width - 251;
                panel1.Height = Height - 63;
            }
        }
    }
}
    