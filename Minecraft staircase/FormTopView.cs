﻿using System;
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
    public partial class FormTopView : Form
    {
        const string BlockIDS = @"BlockIDS.txt";
        const int blockSize = 16;
        const int maxSize = 4;

        int[,] blockMap;
        Dictionary<int, Bitmap> textures;
        Dictionary<int, string> blockNames;

        Image originalImage;

        int curSize = 1;

        public FormTopView()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(WheelRolled);
        }

        internal void Show(int[,] ids)
        {
            Show();
            blockMap = ids;
            LoadTextures();
            pictureBox1.Image = new Bitmap(128 * blockSize, 128 * blockSize);
            pictureBox1.Width = 128 * blockSize;
            pictureBox1.Height = 128 * blockSize;
            CreateImage();
            originalImage = pictureBox1.Image;
            PrintMesh(pictureBox1.Image);
            PrintChunkBoundaries(pictureBox1.Image);
        }

        void LoadTextures()
        {
            textures = new Dictionary<int, Bitmap>();
            textures.Add(-1, new Bitmap(@"Textures\" + blockSize + @"\overflow.png"));
            blockNames = new Dictionary<int, string>();
            using (FileStream fs = new FileStream(BlockIDS, FileMode.Open))
            {
                StreamReader reader = new StreamReader(fs);
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line[0] != '/' && line[1] != '/')
                    {
                        textures.Add(Convert.ToInt32(line.Split(new char[] { '-' })[0]), new Bitmap($"Textures\\{blockSize}\\{line.Split(new char[] { '-' })[1]}.png"));
                        blockNames.Add(Convert.ToInt32(line.Split(new char[] { '-' })[0]), line.Split(new char[] { '-' })[2]);
                    }
                    line = reader.ReadLine();
                }
            }
        }

        void CreateImage()
        {
            Graphics graph = Graphics.FromImage(pictureBox1.Image);
            for (int i = 0; i < 128; i++)
                for (int j = 0; j < 128; j++)
                    graph.DrawImage(textures[blockMap[i, j]], i * blockSize, j * blockSize);
        }


        bool isTopMost = false;
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            TopMost = isTopMost ? false : true;
            isTopMost = !isTopMost;
            ShowInfo(isTopMost ? "TopMost enabled" : "TopMost disabled", Color.LightPink);
        }


        void PrintMesh(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            for (int i = 0; i < 127; i++)
            {
                g.DrawLine(new Pen(Color.Black), new Point((image.Width / 128 * (i + 1)), 0), new Point((image.Width / 128 * (i + 1)), image.Height));
                g.DrawLine(new Pen(Color.Black), new Point(0, (image.Height / 128 * (i + 1))), new Point(image.Width, (image.Height / 128 * (i + 1))));
            }
        }

        void PrintChunkBoundaries(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            for (int i = 0; i < 127; i++)
            {
                g.DrawLine(new Pen(Color.Red, 2), new Point((image.Width / 8 * (i + 1)), 0), new Point((image.Width / 8 * (i + 1)), image.Height));
                g.DrawLine(new Pen(Color.Red, 2), new Point(0, (image.Height / 8 * (i + 1))), new Point(image.Width, (image.Height / 8 * (i + 1))));
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
                    //&& Cursor.Position.X - cur.X + curnew.X + pictureBox1.Width >= Width)
                    newLoc.X = Cursor.Position.X - cur.X + curnew.X;
                if (Cursor.Position.Y - cur.Y + curnew.Y <= 0)
                    //&& Cursor.Position.Y - cur.Y + curnew.Y >= Height - pictureBox1.Height + blockSize)
                    newLoc.Y = Cursor.Position.Y - cur.Y + curnew.Y;

                if (newLoc.X + pictureBox1.Width <= Width)
                    newLoc.X = Width - pictureBox1.Width - 15;
                if (newLoc.Y + pictureBox1.Height <= Height)
                    newLoc.Y = Height - pictureBox1.Height - 38;

                pictureBox1.Location = newLoc;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            moveImage = false;
            curnew = pictureBox1.Location;
        }


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int k = (int)Math.Pow(2, curSize - 1);
            ShowInfo(blockNames[blockMap[e.Location.X / (blockSize * k), e.Location.Y / (blockSize * k)]].ToString(), Color.Aquamarine);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            label1.Visible = false;
        }

        void ShowInfo(string text, Color col)
        {
            label1.Text = text;
            label1.BackColor = col;
            label1.Visible = true;
            timer1.Stop();
            timer1.Start();
        }

        private void FormTopView_Resize(object sender, EventArgs e)
        {
            Point loc = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
            if (pictureBox1.Location.X + pictureBox1.Width <= Width)
                loc.X = Width - pictureBox1.Width - 15;
            if (pictureBox1.Location.Y + pictureBox1.Height <= Height)
                loc.Y = Height - pictureBox1.Height - 38;
            pictureBox1.Location = loc;
        }

        private void WheelRolled(object sender, MouseEventArgs e)
        {
            Point loc = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
            int k = (int)Math.Pow(2, curSize);
            if (e.Delta > 0 && curSize > 1)
            {
                loc = new Point(pictureBox1.Location.X /*+ ((Width - 15) / k)*/, pictureBox1.Location.Y /*+ ((Height - 38) / k)*/);
                --curSize;
                pictureBox1.Width = pictureBox1.Width / 2;
                pictureBox1.Height = pictureBox1.Height / 2;               
            }
            else if (e.Delta < 0 && curSize < maxSize)
            {
                loc = new Point(pictureBox1.Location.X /*- ((Width - 15) / k)*/, pictureBox1.Location.Y /*- ((Height - 38) / k)*/);
                ++curSize;
                pictureBox1.Width = pictureBox1.Width * 2;
                pictureBox1.Height = pictureBox1.Height * 2;
            }         
            if (pictureBox1.Location.X + pictureBox1.Width <= Width)
                loc.X = Width - pictureBox1.Width - 15;
            if (pictureBox1.Location.Y + pictureBox1.Height <= Height)
                loc.Y = Height - pictureBox1.Height - 38;
            if (pictureBox1.Location.X > 0)
                loc.X = 0;
            if (pictureBox1.Location.Y > 0)
                loc.Y = 0;
            pictureBox1.Location = loc;
        }
    }
}