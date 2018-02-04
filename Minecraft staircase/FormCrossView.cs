using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using static Minecraft_staircase.FormMain;

namespace Minecraft_staircase
{
    public partial class FormCrossView : Form
    {
        const string compassFile = @"data\North.png";
        const string blockIDS = @"data\BlockIDS.txt";
        const int blockSize = 16;

        SettedBlock[,] blockMap;

        Bitmap[] LeftImage;

        Dictionary<int, Bitmap> textures;
        Dictionary<int, string> blockNames;

        int curLayer;

        public FormCrossView()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(WheelRolled);
            pictureBox2.Image = Image.FromFile(compassFile);
        }

        internal void Show(SettedBlock[,] data)
        {
            Show();
            this.blockMap = data;
            LoadTextures();
            //pictureBox1.Image = new Bitmap(129 * blockSize, 128 * blockSize); //?
            pictureBox1.Image = CreateLayer(0);
            //CreateLayers();
        }

        void LoadTextures()
        {
            textures = new Dictionary<int, Bitmap>();
            blockNames = new Dictionary<int, string>();
            textures.Add(-1, new Bitmap(@"data\Textures\" + blockSize + @"\overflow.png"));
            using (FileStream fs = new FileStream(blockIDS, FileMode.Open))
            {
                StreamReader reader = new StreamReader(fs);
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line[0] != '/' && line[1] != '/')
                    {
                        textures.Add(Convert.ToInt32(line.Split(new char[] { '-' })[0]), new Bitmap($@"data\Textures\{blockSize}\{line.Split(new char[] { '-' })[1]}.png"));
                        blockNames.Add(Convert.ToInt32(line.Split(new char[] { '-' })[0]), line.Split(new char[] { '-' })[2]);
                    }
                    line = reader.ReadLine();
                }
            }
        }

        void CreateLayers()
        {
            LeftImage = new Bitmap[128];
            for (int i =0; i < 128; i++)
            {
                CreateLayer(i);
                LeftImage[i].RotateFlip(RotateFlipType.Rotate270FlipNone);
                PrintMesh(LeftImage[i]);
            }
            pictureBox1.Image = LeftImage[0];
            curLayer = 0;
        }




        //void CreateLayer(int i)
        Image CreateLayer(int i)
        {
            //LeftImage[i] = new Bitmap(128 * blockSize, 129 * blockSize);
            Image tempImg = new Bitmap(128 * blockSize, 129 * blockSize);
            //Graphics graph = Graphics.FromImage(LeftImage[i]);
            Graphics graph = Graphics.FromImage(tempImg);
            for (int j = 0; j < 129; j++)
            {
                graph.DrawImage(textures[blockMap[i, j].ID], blockMap[i, j].Height * blockSize, j * blockSize);
            }
            tempImg.RotateFlip(RotateFlipType.Rotate270FlipNone);
            PrintMesh(tempImg);
            PrintChunkBoundaries(tempImg, true);
            return tempImg;
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            curLayer = curLayer != 127 ? curLayer + 1 : 0;
            //pictureBox1.Image = LeftImage[curLayer];
            pictureBox1.Image = CreateLayer(curLayer);
            textBox1.Text = curLayer.ToString();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            curLayer = curLayer != 0 ? curLayer - 1 : 127;
            //pictureBox1.Image = LeftImage[curLayer];
            pictureBox1.Image = CreateLayer(curLayer);
            textBox1.Text = curLayer.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            curLayer = 0;
            if (Int32.TryParse(textBox1.Text, out curLayer))
            {
                if (curLayer > 127)
                {
                    curLayer = 127;
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
                //pictureBox1.Image = LeftImage[curLayer];
                pictureBox1.Image = CreateLayer(curLayer);
            }
        }


        void PrintMesh(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            for (int i = 0; i < 127; i++)
            {
                g.DrawLine(new Pen(Color.Black), new Point((image.Width / 129 * (i + 1)), 0), new Point((image.Width / 129 * (i + 1)), image.Height));
                g.DrawLine(new Pen(Color.Black), new Point(0, (image.Height / 128 * (i + 1))), new Point(image.Width, (image.Height / 128 * (i + 1))));
            }
            g.DrawLine(new Pen(Color.Black), new Point((image.Width / 129 * 129), 0), new Point((image.Width / 129 * 129), image.Height));
        }

        void PrintChunkBoundaries(Image image, bool set)
        {
            Graphics g = Graphics.FromImage(image);
            for (int i = 0; i < 8; i++)
            {
                int pureWidth = image.Width - (image.Width / 129);
                g.DrawLine(new Pen(set ? Color.Red : Color.Black), new Point(pureWidth / 8 * i + image.Width / 129, 0), new Point(pureWidth / 8 * i + image.Width / 129, image.Height));
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkBox1.Checked;
        }

        bool isNotFull = true;
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (isNotFull)
            {
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