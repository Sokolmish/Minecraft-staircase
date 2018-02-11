using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Substrate;
using Substrate.ImportExport;

namespace Minecraft_staircase
{
    public partial class NewFormMain : Form
    {
        const int blockSize = 16;
        const string BlockIDS = @"data\PossibleBlocks.txt";

        List<PixelData> colorsList;
        List<PixelData> extendedColorsList;

        Image originalImage;
        Image rawImage;
        Image convertedImage;

        SettedBlock[,] blockMap;
        int[] recourses;
        int maxHeight;

        Thread convertTask;     

        public NewFormMain()
        {
            InitializeComponent();
            LoadColors();
        }

        void LoadColors()
        {
            colorsList = new List<PixelData>();
            MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(Properties.Resources.ColorsIDS));
            StreamReader reader = new StreamReader(ms);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            colorsList = new List<PixelData>();
            while (!reader.EndOfStream)
            {
                colorsList.Add((PixelData)serializer.Deserialize(reader.ReadLine(), typeof(PixelData)));
            }
            ms.Close();

            extendedColorsList = new List<PixelData>(colorsList);
            using (FileStream fs = new FileStream(BlockIDS, FileMode.Open))
            {
                reader = new StreamReader(fs);
                string line = reader.ReadLine();
                int id = 1;
                while (line != null)
                {
                    if (line.Split('~')[0] == "False")
                        colorsList.RemoveAll((e) => { return e.ID == id; });
                    ++id;
                    line = reader.ReadLine();
                }
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
            new FormSelectMaterials().ShowDialog(extendedColorsList);
        }


        private void CreateButton_Click(object sender, EventArgs e)
        {
            LoadColors();
            progressBar1.Maximum = rawImage.Width * rawImage.Height;
            progressBar1.Value = 0;
            convertTask?.Abort();
            convertTask = new Thread(() =>
            {
                ArtGenerator gen = new ArtGenerator(colorsList, extendedColorsList);
                gen.SetProgress(progressBar1);
                convertedImage = rawImage.Clone() as Image; 
                ArtType type = ArtType.Flat;
                if (radioButton2.Checked)
                    type = ArtType.Lite;
                else if (radioButton3.Checked)
                    type = ArtType.Full;
                blockMap = gen.CreateScheme(ref convertedImage, type, out recourses, out maxHeight);
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
            //pictureBox1.Width = convertedImage.Width < 256 ? 256 : convertedImage.Width;
            //pictureBox1.Height = convertedImage.Height < 256 ? 256 : convertedImage.Height;
            panel1.Width = convertedImage.Width < 256 ? 256 : convertedImage.Width;
            panel1.Height = convertedImage.Height < 256 ? 256 : convertedImage.Height;
            Width = panel1.Width < 256 ? 256 : panel1.Width + 20;
            Height = panel1.Height < 256 ? 256 : panel1.Height + 43;
            panel1.Location = new Point(0, 0);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Size = new Size(763, 575);
            panel1.Size = new Size(512, 512);
            panel1.Location = new Point(223, 12);
        }


        private void TopViewButton_Click(object sender, EventArgs e)
        {
            int[,] ids = new int[blockMap.GetLength(0), blockMap.GetLength(1) - 1];
            for (int i = 0; i < blockMap.GetLength(0); i++)
                for (int j = 1; j < blockMap.GetLength(1); j++)
                    ids[i, j - 1] = blockMap[i, j].ID;
            new FormTopView().Show(ids);
        }

        private void CrossViewButton_Click(object sender, EventArgs e)
        {
            new FormCrossView().Show(blockMap, maxHeight + 1);
        }

        private void UsedMaterialsButton_Click(object sender, EventArgs e)
        {
            new FormMaterials().Show(recourses);
        }

        private void SchematicButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Schematic schem = new Schematic(blockMap.GetLength(0), maxHeight + 1, blockMap.GetLength(1));
                List<int[]> blockIds = new List<int[]>();
                using (FileStream fs = new FileStream(BlockIDS, FileMode.Open))
                {
                    StreamReader reader = new StreamReader(fs);
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        line = line.Split('~')[1].Split(',')[0];
                        if (line[0] != '/' && line[1] != '/')
                            blockIds.Add(new int[] { Convert.ToInt32(line.Split('-')[2]), Convert.ToInt32(line.Split('-')[3]) });
                        line = reader.ReadLine();
                    }
                }
                for (int i = 0; i < blockMap.GetLength(0); i++)
                    for (int j = 1; j < blockMap.GetLength(1); j++)
                        schem.Blocks.SetBlock(i, blockMap[i, j].Height, j - 1, new AlphaBlock(blockIds[blockMap[i, j].ID - 1][0], blockIds[blockMap[i, j].ID - 1][1]));
                schem.Export(saveFileDialog1.FileName.Contains(".schematic") ? saveFileDialog1.FileName : saveFileDialog1.FileName + ".schematic");
            }
        }


        private void button1_Click_2(object sender, EventArgs e)
        {
            new AboutBox1().Show();
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/CjErtQY");
        }
    }
}
