using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Minecraft_staircase
{
    public partial class FormCrossView : Form
    {
        const string blockIDS = @"data\BlockIDS.txt";
        const string compassImage = @"data\North.png";
        const string BlockIDS = @"data\PossibleBlocks.txt";
        const int blockSize = 16;

        SettedBlock[,] blockMap;
        int maxHeight;

        Dictionary<int, Bitmap> textures;
        Dictionary<int, string> blockNames;

        int curLayer;

        Color defMeshColor = Color.Black;
        Color chunkMeshColor = Color.Red;
        Color mapMeshColor = Color.Purple;

        public FormCrossView()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(WheelRolled);
            pictureBox2.Image = Image.FromFile(compassImage);
        }

        internal void Show(SettedBlock[,] blockMap, int maxHeight)
        {
            Show();
            this.blockMap = blockMap;
            this.maxHeight = maxHeight;
            LoadTextures();
            pictureBox1.Image = CreateLayer(0);
            label3.Text = $"Maximum height - {maxHeight}";
        }

        void LoadTextures()
        {
            textures = new Dictionary<int, Bitmap>();
            textures.Add(-1, new Bitmap(@"data\Textures\overflow.png"));
            blockNames = new Dictionary<int, string>();
            using (FileStream fs = new FileStream(BlockIDS, FileMode.Open))
            {
                StreamReader reader = new StreamReader(fs);
                string line = reader.ReadLine();
                int id = 1;
                while (line != null)
                {
                    line = line.Split('~')[1].Split(',')[0];
                    if (line[0] != '/' && line[1] != '/')
                    {
                        textures.Add(id, new Bitmap($@"data\Textures\{line.Split(new char[] { '-' })[0]}"));
                        blockNames.Add(id++, line.Split(new char[] { '-' })[1]);
                    }
                    line = reader.ReadLine();
                }
            }
        }

        Image CreateLayer(int i)
        {
            Image tempImg = new Bitmap(blockMap.GetLength(1) * blockSize, maxHeight > 3 ? maxHeight * blockSize : 3 * blockSize);
            Graphics graph = Graphics.FromImage(tempImg);
            for (int j = 0; j < (blockMap.GetLength(1)); j++)
            {
                graph.DrawImage(textures[blockMap[i, j].ID], j * blockSize, blockMap[i, j].Height * blockSize);
            }
            //tempImg.RotateFlip(RotateFlipType.Rotate270FlipNone);
            graph = Graphics.FromImage(tempImg);
            PrintMesh(tempImg);
            PrintChunkMesh(tempImg);
            PrintMapMesh(tempImg);
            tempImg.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return tempImg;
        }


        void PrintMesh(Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(1); i++)
                graph.DrawLine(new Pen(defMeshColor, 1), new Point(blockSize * (i + 1), 0), new Point(blockSize * (i + 1), image.Height));
        }

        void PrintChunkMesh(Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(1) / 16; i++)
                graph.DrawLine(new Pen(chunkMeshColor, 2), new Point(blockSize * 16 * (i + 1) + blockSize, 0), new Point(blockSize * 16 * (i + 1) + blockSize, image.Height));
        }

        void PrintMapMesh(Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(1) / 128; i++)
                graph.DrawLine(new Pen(mapMeshColor, 2), new Point(blockSize * 128 * (i + 1) + blockSize, 0), new Point(blockSize * 128 * (i + 1) + blockSize, image.Height));
        }


        private void buttonNext_Click(object sender, EventArgs e)
        {
            curLayer = curLayer != blockMap.GetLength(0) - 1 ? curLayer + 1 : 0;
            pictureBox1.Image = CreateLayer(curLayer);
            textBox1.Text = curLayer.ToString();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            curLayer = curLayer != 0 ? curLayer - 1 : blockMap.GetLength(0) - 1;
            pictureBox1.Image = CreateLayer(curLayer);
            textBox1.Text = curLayer.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            curLayer = 0;
            if (Int32.TryParse(textBox1.Text, out curLayer))
            {
                if (curLayer > blockMap.GetLength(0) - 1)
                {
                    curLayer = blockMap.GetLength(0) - 1;
                    int cursor = textBox1.SelectionStart;
                    textBox1.Text = curLayer.ToString();
                    textBox1.SelectionStart = cursor;
                }
                if (curLayer < 0)
                {
                    curLayer = 0;
                    int cursor = textBox1.SelectionStart;
                    textBox1.Text = curLayer.ToString();
                    textBox1.SelectionStart = cursor;
                }
                pictureBox1.Image = CreateLayer(curLayer);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkBox1.Checked;
        }

        bool isNotFull = true;
        Size naturalSize;
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (isNotFull)
            {
                naturalSize = pictureBox1.Size;
                isNotFull = false;
                panel1.Dock = DockStyle.Fill;
                Point loc = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
                if (pictureBox1.Location.X + pictureBox1.Width <= panel1.Width)
                    loc.X = panel1.Width - pictureBox1.Width;
                if (pictureBox1.Location.Y + pictureBox1.Height <= panel1.Height)
                    loc.Y = panel1.Height - pictureBox1.Height;
                pictureBox1.Location = loc;
                checkBox1.Visible = false;
                label1.Visible = false;
                textBox1.Visible = false;
                pictureBox2.Visible = false;
                FormBorderStyle = FormBorderStyle.Sizable;
                MaximizeBox = true;
            }
            else
            {
                pictureBox1.Size = naturalSize;
                isNotFull = true;
                panel1.Dock = DockStyle.None;
                Point loc = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
                if (pictureBox1.Location.X + pictureBox1.Width <= panel1.Width)
                    loc.X = panel1.Width - pictureBox1.Width;
                if (pictureBox1.Location.Y + pictureBox1.Height <= panel1.Height)
                    loc.Y = panel1.Height - pictureBox1.Height;
                pictureBox1.Location = loc;
                pictureBox1.Location = loc;
                checkBox1.Visible = true;
                label1.Visible = true;
                textBox1.Visible = true;
                pictureBox2.Visible = true;
                FormBorderStyle = FormBorderStyle.Fixed3D;
                MaximizeBox = false;
                Width = 728;
                Height = 584;
            }
        }

        Point cur, curnew;
        bool moveImage;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            cur = Cursor.Position;
            moveImage = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveImage)
            {
                Point newLoc = pictureBox1.Location;
                if (Cursor.Position.X - cur.X + curnew.X <= 0)
                    newLoc.X = Cursor.Position.X - cur.X + curnew.X;
                if (Cursor.Position.Y - cur.Y + curnew.Y <= 0)
                    newLoc.Y = Cursor.Position.Y - cur.Y + curnew.Y;

                if (newLoc.X + pictureBox1.Width <= panel1.Width)
                    newLoc.X = panel1.Width - pictureBox1.Width; //15
                if (newLoc.Y + pictureBox1.Height <= panel1.Height)
                    newLoc.Y = panel1.Height - pictureBox1.Height ; //38

                pictureBox1.Location = newLoc;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            moveImage = false;
            curnew = pictureBox1.Location;
        }

        int curSize = 1;
        const int maxSize = 3;

        private void WheelRolled(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0 && curSize > 1)
            {
                --curSize;
                pictureBox1.Width = pictureBox1.Width / 2;
                pictureBox1.Height = pictureBox1.Height / 2;
            }
            else if (e.Delta > 0 && curSize < maxSize)
            {
                ++curSize;
                pictureBox1.Width = pictureBox1.Width * 2;
                pictureBox1.Height = pictureBox1.Height * 2;
            }
            Point loc = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
            if (pictureBox1.Location.X + pictureBox1.Width <= panel1.Width)
                loc.X = panel1.Width - pictureBox1.Width;
            if (pictureBox1.Location.Y + pictureBox1.Height <= panel1.Height)
                loc.Y = panel1.Height - pictureBox1.Height;
            pictureBox1.Location = loc;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int k = (int)Math.Pow(2, curSize - 1);
            if (e.Location.X / (blockSize * k) == 0)
                label2.Text = "Any block";
            else
                label2.Text = blockNames[blockMap[curLayer, e.Location.X / (blockSize * k)].ID];
            label2.Visible = true;
            timer1.Stop();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            label2.Visible = false;
        }
    }
}