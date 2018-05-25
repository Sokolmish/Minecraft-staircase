using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Minecraft_staircase
{
    public partial class FormTopView : Form
    {
        const int blockSize = 16;
        const int maxSize = 3;
        const int maxImage = 5;

        int[,] blockMap;
        Dictionary<int, Bitmap> textures;
        List<ColorNote> colors;

        Image originalImage;

        int curSize = 1;

        Color defMeshColor = Properties.Settings.Default.defMeshColor;
        Color chunkMeshColor = Properties.Settings.Default.chunkMeshColor;
        Color mapMeshColor = Properties.Settings.Default.mapMeshColor;

        public FormTopView()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(WheelRolled);
        }

        internal void Show(ref int[,] ids, ref List<ColorNote> colors)
        {
            if (ids.GetLength(0) > 128 * maxImage || ids.GetLength(1) > 128 * maxImage)
            {
                MessageBox.Show(Lang.GetHint("BigImageError"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            this.colors = colors;
            blockMap = ids;
            LoadTextures();
            pictureBox1.Image = new Bitmap(ids.GetLength(0) * blockSize, ids.GetLength(1) * blockSize);
            pictureBox1.Width = ids.GetLength(0) * blockSize;
            pictureBox1.Height = ids.GetLength(1) * blockSize;
            CreateImage();
            originalImage = pictureBox1.Image;
            PrintMesh(pictureBox1.Image);
            PrintChunkMesh(pictureBox1.Image);
            PrintMapMesh(pictureBox1.Image);
            Show();
            if (!Properties.Settings.Default.HideTips)
                MessageBox.Show(Lang.GetHint("TopTopMostHint"), "Hint", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void LoadTextures()
        {
            textures = new Dictionary<int, Bitmap>();
            textures.Add(-1, new Bitmap(@"data\Textures\overflow.png"));
            foreach (ColorNote col in colors)
                textures.Add(col.ColorID, Image.FromFile(@"data\Textures\" + col.SelectedBlock.TextureName) as Bitmap);
        }

        void CreateImage()
        {
            Graphics graph = Graphics.FromImage(pictureBox1.Image);
            for (int i = 0; i < blockMap.GetLength(0); ++i)
                for (int j = 0; j < blockMap.GetLength(1); ++j)
                    graph.DrawImage(textures[blockMap[i, j]], i * blockSize, j * blockSize);
            graph.Dispose();
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
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(0) - 1; ++i)
                graph.DrawLine(new Pen(defMeshColor, 1), new Point(blockSize * (i + 1), 0), new Point(blockSize * (i + 1), image.Height));
            for (int i = 0; i < blockMap.GetLength(1) - 1; ++i)
                graph.DrawLine(new Pen(defMeshColor, 1), new Point(0, blockSize * (i + 1)), new Point(image.Width, blockSize * (i + 1)));
            graph.Dispose();
        }

        void PrintChunkMesh(Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(0) / 16 - 1; ++i)
                graph.DrawLine(new Pen(chunkMeshColor, 2), new Point(blockSize * 16 * (i + 1), 0), new Point(blockSize * 16 * (i + 1), image.Height));
            for (int i = 0; i < blockMap.GetLength(1) - 1; ++i)
                graph.DrawLine(new Pen(chunkMeshColor, 2), new Point(0, blockSize * 16 * (i + 1)), new Point(image.Width, blockSize * 16 * (i + 1)));
            graph.Dispose();
        }

        void PrintMapMesh(Image image)
        {
            Graphics graph = Graphics.FromImage(image);
            for (int i = 0; i < blockMap.GetLength(0) / 128 - 1; ++i)
                graph.DrawLine(new Pen(mapMeshColor, 2), new Point(blockSize * 128 * (i + 1), 0), new Point(blockSize * 128 * (i + 1), image.Height));
            for (int i = 0; i < blockMap.GetLength(1) / 128 - 1; ++i)
                graph.DrawLine(new Pen(mapMeshColor, 2), new Point(0, blockSize * 128 * (i + 1)), new Point(image.Width, blockSize * 128 * (i + 1)));
            graph.Dispose();
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
            ShowInfo(colors.Find((x) => { return blockMap[e.Location.X / (blockSize * k), e.Location.Y / (blockSize * k)] == x.ColorID; }).SelectedBlock.Name, Color.Aquamarine);
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
            if (e.Delta < 0 && curSize > 1)
            {
                loc = new Point(pictureBox1.Location.X /*+ ((Width - 15) / k)*/, pictureBox1.Location.Y /*+ ((Height - 38) / k)*/);
                --curSize;
                pictureBox1.Width = pictureBox1.Width / 2;
                pictureBox1.Height = pictureBox1.Height / 2;               
            }
            else if (e.Delta > 0 && curSize < maxSize)
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


        private void FormTopView_FormClosing(object sender, FormClosingEventArgs e)
        {
            pictureBox1.Image = null;
            originalImage = null;
            textures = null;
            GC.Collect();
        }
    }
}