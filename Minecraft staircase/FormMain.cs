using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Substrate;
using Substrate.ImportExport;

namespace Minecraft_staircase
{
    public partial class FormMain : Form
    {
        const string colorsIDS = @"data/oldColorsIDS.txt";
        const string blockIDS = @"data/BlockIDS.txt";
        const string waitingImage = @"data/Waiting.gif";
        const string helpFile = @"data/help.txt";

        List<PixelData> ColorsList;

        Image originalImage;
        UnsettedBlock[,] RawScheme;

        int[] Recources;

        Task convertTask;

        public FormMain()
        {
            InitializeComponent();
            LoadColors();
        }

        void LoadColors()
        {
            FileStream fs = new FileStream(colorsIDS, FileMode.Open);
            StreamReader reader = new StreamReader(fs);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ColorsList = new List<PixelData>();
            while (!reader.EndOfStream)
            {
                ColorsList.Add((PixelData)serializer.Deserialize(reader.ReadLine(), typeof(PixelData)));
            }
        }

        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(Image.FromFile(openFileDialog1.FileName), 128, 128);
                pictureBox1.Image = originalImage;
                label1.Visible = false;
            }
        }

        #region Converting
        private void flatToolStripMenuItem_Click(object sender, EventArgs e) => ConvertImg(true, true);

        private void liteStaircaseToolStripMenuItem_Click(object sender, EventArgs e) => ConvertImg(false, true);

        private void fullStaircaseToolStripMenuItem_Click(object sender, EventArgs e) => ConvertImg(false, false);

        private void ConvertImg(bool flat, bool upward)
        {          
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("First upload an image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            pictureBox2.Image = Image.FromFile(waitingImage);
            convertTask = new Task(() =>
            {
                Bitmap tempImg = new Bitmap(pictureBox1.Image);
                RawScheme = new UnsettedBlock[tempImg.Height, tempImg.Width];
                Recources = new int[51];
                for (int i = 0; i < tempImg.Height; i++)
                {
                    for (int j = 0; j < tempImg.Width; j++)
                    {
                        int betterID = 0;
                        ColorType betterSet = ColorType.Normal; //0-Down 1-Mid 2-Up
                        double betterSimilarity = 1000;
                        Color selectedColor = new Color();
                        foreach (PixelData col in ColorsList)
                        {
                            if (Similarity(col.NormalColor[0], col.NormalColor[1], col.NormalColor[2],
                                tempImg.GetPixel(i, j).R, tempImg.GetPixel(i, j).G, tempImg.GetPixel(i, j).B) 
                                < betterSimilarity)
                            {
                                betterID = col.ID;
                                betterSet = ColorType.Normal;
                                betterSimilarity = Similarity(col.NormalColor[0], col.NormalColor[1], col.NormalColor[2], tempImg.GetPixel(i, j).R, tempImg.GetPixel(i, j).G, tempImg.GetPixel(i, j).B);
                                selectedColor = Color.FromArgb(col.NormalColor[0], col.NormalColor[1], col.NormalColor[2]);
                            }
                            if (!flat)
                            {
                                if (Similarity(col.DarkColor[0], col.DarkColor[1], col.DarkColor[2],
                                    tempImg.GetPixel(i, j).R, tempImg.GetPixel(i, j).G, tempImg.GetPixel(i, j).B)
                                    < betterSimilarity &&
                                    !upward)
                                {
                                    betterID = col.ID;
                                    betterSet = ColorType.Dark;
                                    betterSimilarity = Similarity(col.DarkColor[0], col.DarkColor[1], col.DarkColor[2], tempImg.GetPixel(i, j).R, tempImg.GetPixel(i, j).G, tempImg.GetPixel(i, j).B);
                                    selectedColor = Color.FromArgb(col.DarkColor[0], col.DarkColor[1], col.DarkColor[2]);
                                }
                                if (Similarity(col.LightColor[0], col.LightColor[1], col.LightColor[2],
                                    tempImg.GetPixel(i, j).R, tempImg.GetPixel(i, j).G, tempImg.GetPixel(i, j).B)
                                    < betterSimilarity)
                                {
                                    betterID = col.ID;
                                    betterSet = ColorType.Light;
                                    betterSimilarity = Similarity(col.LightColor[0], col.LightColor[1], col.LightColor[2], tempImg.GetPixel(i, j).R, tempImg.GetPixel(i, j).G, tempImg.GetPixel(i, j).B);
                                    selectedColor = Color.FromArgb(col.LightColor[0], col.LightColor[1], col.LightColor[2]);
                                }
                            }
                        }
                        RawScheme[i, j].ID = betterID;
                        RawScheme[i, j].Set = betterSet;
                        tempImg.SetPixel(i, j, selectedColor);
                        Recources[betterID - 1]++;
                    }
                }
                pictureBox2.Image = tempImg;
            });
            convertTask.Start();
            label2.Visible = false;
        }

        double Similarity(int r1, int g1, int b1, int r2, int g2, int b2) => 
            Math.Sqrt(Math.Pow(r2 - r1, 2) + Math.Pow(g2 - g1, 2) + Math.Pow(b2 - b1, 2));
        #endregion

        private void materialsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RawScheme == null)
            {
                MessageBox.Show("First convert an image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            new FormMaterials().Show(Recources);
        }

        private void serviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{originalImage.Width} {originalImage.Height}");
        }

        private void crossViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            convertTask?.Wait();
            if (RawScheme == null)
            {
                MessageBox.Show("First convert an image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SettedBlock[,] BlockMap = new SettedBlock[128, 129];
            for (int i = 0; i < 128; i++)
                BlockMap[i, 0] = new SettedBlock() { ID = -1, Height = 0 };
            for (int i = 0; i < 128; i++)
            {
                int minimalHeight = 0;
                for (int j = 1; j < 129; j++)
                {
                    BlockMap[i, j].ID = RawScheme[i, j - 1].ID;
                    switch (RawScheme[i, j - 1].Set)
                    {
                        case ColorType.Normal:
                            BlockMap[i, j].Height = BlockMap[i, j - 1].Height;
                            break;
                        case ColorType.Dark:
                            BlockMap[i, j].Height = BlockMap[i, j - 1].Height - 1;
                            break;
                        case ColorType.Light:
                            BlockMap[i, j].Height = BlockMap[i, j - 1].Height + 1;
                            break;
                    }
                    if (BlockMap[i, j].Height < minimalHeight)
                        minimalHeight = BlockMap[i, j].Height;
                }
                for (int j = 0; j < 129; j++)
                    BlockMap[i, j].Height = BlockMap[i, j].Height - minimalHeight;
            }
            new FormCrossView().Show(BlockMap);
        }

        private void topViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            convertTask?.Wait();
            if (RawScheme == null)
            {
                MessageBox.Show("First convert an image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int[,] ids = new int[128, 128];
            for (int i = 0; i < 128; i++)
                for (int j = 0; j < 128; j++)
                    ids[i, j] = RawScheme[i, j].ID;
            new FormTopView().Show(ids);
        }

        private void schematicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            convertTask?.Wait();
            if (RawScheme == null)
            {
                MessageBox.Show("First convert an image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SettedBlock[,] BlockMap = new SettedBlock[128, 129];
                for (int i = 0; i < 128; i++)
                    BlockMap[i, 0] = new SettedBlock() { ID = -1, Height = 0 };
                int maximumHeigth = 0;
                for (int i = 0; i < 128; i++)
                {
                    int minimalHeight = 0;
                    for (int j = 1; j < 129; j++)
                    {
                        BlockMap[i, j].ID = RawScheme[i, j - 1].ID;
                        switch (RawScheme[i, j - 1].Set)
                        {
                            case ColorType.Normal:
                                BlockMap[i, j].Height = BlockMap[i, j - 1].Height;
                                break;
                            case ColorType.Dark:
                                BlockMap[i, j].Height = BlockMap[i, j - 1].Height - 1;
                                break;
                            case ColorType.Light:
                                BlockMap[i, j].Height = BlockMap[i, j - 1].Height + 1;
                                break;
                        }
                        if (BlockMap[i, j].Height < minimalHeight)
                            minimalHeight = BlockMap[i, j].Height;
                        if (BlockMap[i, j].Height > maximumHeigth)
                            maximumHeigth = BlockMap[i, j].Height;
                    }
                    for (int j = 0; j < 129; j++)
                        BlockMap[i, j].Height = BlockMap[i, j].Height - minimalHeight;
                }
                Schematic schem = new Schematic(128, maximumHeigth + 1, 128);
                List<int[]> blockIds = new List<int[]>();
                using (FileStream fs = new FileStream(blockIDS, FileMode.Open))
                {
                    StreamReader reader = new StreamReader(fs);
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        if (line[0] != '/' && line[1] != '/')
                            blockIds.Add(new int[] { Convert.ToInt32(line.Split('-')[3]), Convert.ToInt32(line.Split('-')[4]) });
                        line = reader.ReadLine();
                    }
                }
                for (int i = 0; i < 128; i++)
                    for (int j = 1; j < 129; j++)
                        schem.Blocks.SetBlock(i, BlockMap[i, j].Height, j - 1, new AlphaBlock(blockIds[BlockMap[i, j].ID - 1][0], blockIds[BlockMap[i, j].ID - 1][1]));
                schem.Export(saveFileDialog1.FileName.Contains(".schematic") ? saveFileDialog1.FileName : saveFileDialog1.FileName + ".schematic");
            }
        }


        private void discordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/CjErtQY");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(helpFile);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().Show();
        }
    }
}
