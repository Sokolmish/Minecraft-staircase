using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Minecraft_staircase
{
    public partial class FormMain : Form
    {
        const int blockSize = 16;
        const int maxSize = 10;

        List<ColorNote> colorsNote;

        Image originalImage;
        Image rawImage;
        Image convertedImage;

        SettedBlock[,] blockMap;
        int maxHeight;

        Thread convertTask;

        bool noResize;

        TextBox textBoxHint = new TextBox
        {
            BackColor = Color.WhiteSmoke,
            Name = "textBox3",
            ReadOnly = true,
            Multiline = true,
            WordWrap = true,
            MaximumSize = new Size(267, 114),
            Visible = false
        };

        public FormMain()
        {
            InitializeComponent();
            NewFormMain_Resize(null, null);  //?
            LoadData();
            Controls.Add(textBoxHint);
            Controls.SetChildIndex(textBoxHint, 0);
#if DEBUG
            label4.Visible = true;
#endif
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
                note.SelectedBlock = new BlockData(block[0], block[1], Convert.ToByte(block[2]), Convert.ToByte(block[3]), block[4] == "True");

                curBlocks = curBlocks.Split('~')[1];
                List<BlockData> blockDatas = new List<BlockData>();
                foreach (string str in curBlocks.Split(','))
                {
                    block = str.Split('-');
                    blockDatas.Add(new BlockData(block[0], block[1], Convert.ToByte(block[2]), Convert.ToByte(block[3]), block[4] == "True"));
                }
                note.PossibleBlocks = blockDatas;
                colorsNote.Add(note);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try { originalImage = Image.FromFile(openFileDialog1.FileName); }
                catch (Exception ex)
                {
                    if (MessageBox.Show(Lang.GetHint("NoImage"), "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK);
                    return;
                }
                if (!checkBox1.Checked)
                {
                    if (originalImage.Width < 128 || originalImage.Height < 128)
                        rawImage = new Bitmap(originalImage, 128, 128);
                    else
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
                GC.Collect();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (originalImage == null)
                return;
            if (!checkBox1.Checked)
            {
                if (originalImage.Width < 128 || originalImage.Height < 128)
                    rawImage = new Bitmap(originalImage, 128, 128);
                else
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
                    if (cur > maxSize)
                    {
                        cur = maxSize;
                        textBox1.Text = maxSize.ToString();
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
                    if (cur > maxSize * 128)
                    {
                        cur = maxSize * 128;
                        textBox1.Text = (maxSize * 128).ToString();
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
                    if (cur > maxSize)
                    {
                        cur = maxSize;
                        textBox2.Text = maxSize.ToString();
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
                    if (cur > maxSize * 128)
                    {
                        cur = maxSize * 128;
                        textBox1.Text = (maxSize * 128).ToString();
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


        DateTime startTime;
        private void CreateButton_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = (rawImage.Width * rawImage.Height) / 128;
            progressBar1.Value = 0;
            startTime = DateTime.Now;
            convertTask?.Abort();
            convertTask = new Thread(() =>
            {
                ArtGenerator gen = new ArtGenerator(ref colorsNote);
                gen.SetProgress(progressBar1);
                gen.SetStateLabel(label4);
                gen.Done += () => 
                {
                    label4.BeginInvoke(new Action(() => { label4.Text = DateTime.Now.Subtract(startTime).ToString(); }));
                    FinalImageButton.BeginInvoke(new Action(() => { FinalImageButton.Enabled = true; }));
                    TopViewButton.BeginInvoke(new Action(() => { TopViewButton.Enabled = true; }));
                    CrossViewButton.BeginInvoke(new Action(() => { CrossViewButton.Enabled = true; }));
                    UsedMaterialsButton.BeginInvoke(new Action(() => { UsedMaterialsButton.Enabled = true; }));
                    SchematicButton.BeginInvoke(new Action(() => { SchematicButton.Enabled = true; }));
                };
                convertedImage = (Image)rawImage.Clone();
                ArtType type = ArtType.Flat;
                if (radioButton2.Checked)
                    type = ArtType.Lite;
                else if (radioButton3.Checked)
                    type = ArtType.Full;
                blockMap = gen.CreateScheme(ref convertedImage, type, out maxHeight);
                pictureBox1.Image = convertedImage;
                GC.Collect();
                if (maxHeight > 250)
                    MessageBox.Show(Lang.GetHint("UnbuildArt").Replace("{0}", maxHeight.ToString()), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (maxHeight > 150)
                    MessageBox.Show(Lang.GetHint("TooHeightArt").Replace("{0}", maxHeight.ToString()), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                label4.BeginInvoke(new Action(() => label4.Text += $" Max={maxHeight}"));
            });
            convertTask.Start();
        }


        private void FinalImageButton_Click(object sender, EventArgs e)
        {
            noResize = true;
            MinimumSize = new Size(0, 0);
            pictureBox1.Width = convertedImage.Width < 256 ? 256 : convertedImage.Width;
            pictureBox1.Height = convertedImage.Height < 256 ? 256 : convertedImage.Height;
            panel1.Width = convertedImage.Width < 256 ? 256 : convertedImage.Width;
            panel1.Height = convertedImage.Height < 256 ? 256 : convertedImage.Height;
            Width = panel1.Width + 15;
            Height = panel1.Height + 38;
            panel1.Location = new Point(0, 0);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            label4.Visible = false;
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
                FormBorderStyle = FormBorderStyle.Sizable;
            }
        }


        private void TopViewButton_Click(object sender, EventArgs e)
        {
            int[,] ids = new int[blockMap.GetLength(0), blockMap.GetLength(1) - 1];
            for (int i = 0; i < blockMap.GetLength(0); i++)
                for (int j = 1; j < blockMap.GetLength(1); j++)
                    ids[i, j - 1] = blockMap[i, j].ID;
            new FormTopView().Show(ref ids, ref colorsNote);
        }

        private void CrossViewButton_Click(object sender, EventArgs e) => new FormCrossView().Show(ref blockMap, maxHeight + 1, ref colorsNote);

        private void UsedMaterialsButton_Click(object sender, EventArgs e) => new FormMaterials().Show(ref colorsNote);

        private void SchematicButton_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            convertTask?.Abort();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Schematic schem = new Schematic(blockMap.GetLength(0), maxHeight + 1, blockMap.GetLength(1));
                label4.Text = "Exporting";
                convertTask = new Thread(() =>
                {
                    for (short i = 0; i < blockMap.GetLength(0); ++i)
                        for (short j = 1; j < blockMap.GetLength(1); ++j)
                        {
                            ColorNote col = colorsNote[blockMap[i, j].ID - 1];
                            schem.SetBlock(i, (short)blockMap[i, j].Height, (short)(j - 1), col.SelectedBlock.ID, col.SelectedBlock.Data);
                        }
                    using (System.IO.FileStream fs = new System.IO.FileStream(saveFileDialog1.FileName.Contains(".schematic") ? 
                        saveFileDialog1.FileName : saveFileDialog1.FileName + ".schematic", System.IO.FileMode.Create))
                        schem.WriteToStream(fs);
                    label4.BeginInvoke(new Action(() => { label4.Text = DateTime.Now.Subtract(startTime).ToString(); }));
                    schem = null;
                    GC.Collect();
                    MessageBox.Show("Complete", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information); //-V3038
                });
                convertTask.Start();
            }
        }


        private void button1_Click_2(object sender, EventArgs e) => new AboutBox1().Show();

        private void OptionsButton_Click(object sender, EventArgs e) => new FormSettings().ShowDialog();


        private void NewFormMain_Resize(object sender, EventArgs e)
        {
            if (!noResize)
            {
                progressBar1.Location = new Point(progressBar1.Location.X, Height - 74);
                label4.Location = new Point(label4.Location.X, Height - 0b1011010);
                panel1.Width = Width - 251;
                panel1.Height = Height - 63;
            }
        }

#region Hints
        bool isOnControl = false;

        private void Hint_MouseLeave(object sender, EventArgs e)
        {
            isOnControl = false;
            try { textBoxHint.BeginInvoke(new Action(() => { textBoxHint.Visible = false; })); }
            catch { }
        }

        private void ShowHint()
        {
            if (!Properties.Settings.Default.HideTips)
                new System.Threading.Timer((x) =>
                {
                    if (isOnControl)
                        try
                        {
                            textBoxHint.BeginInvoke(new Action(() =>
                            {
                                textBoxHint.Location = new Point(MousePosition.X - Location.X, MousePosition.Y - Location.Y);
                                textBoxHint.Size = textBoxHint.GetPreferredSize(new Size());
                                textBoxHint.Visible = true;
                            }));
                        }
                        catch { }
                }, null, 1000, Timeout.Infinite);
        }


        private void OpenButton_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("OpenButton");
            ShowHint();
        }

        private void textBox2_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("HeightTextBox");
            ShowHint();
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("WidthTextBox");
            ShowHint();
        }

        private void checkBox1_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("SizeCheckBox");
            ShowHint();
        }

        private void MaterialsButton_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("MatOptionsButton");
            ShowHint();
        }

        private void radioButton1_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("FlatRadio");
            ShowHint();
        }

        private void radioButton2_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("LiteRadio");
            ShowHint();
        }

        private void radioButton3_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("FullRadio");
            ShowHint();
        }

        private void CreateButton_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("GenerateButton");
            ShowHint();
        }

        private void FinalImageButton_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("ViewImgButton");
            ShowHint();
        }

        private void TopViewButton_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("TopViewButton");
            ShowHint();
        }

        private void CrossViewButton_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("CrossViewButton");
            ShowHint();
        }

        private void UsedMaterialsButton_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("MatButton");
            ShowHint();
        }

        private void SchematicButton_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("SchemButton");
            ShowHint();
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            isOnControl = true;
            textBoxHint.Text = Lang.GetHint("ProgressTimer");
            ShowHint();
        }
#endregion

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move))
                e.Effect = DragDropEffects.Move;
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Effect == DragDropEffects.Move)
            {
                try { originalImage = Image.FromFile(((string[])e.Data.GetData(DataFormats.FileDrop))[0]); }
                catch (Exception ex)
                {
                    if (MessageBox.Show(Lang.GetHint("NoImage"), "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK);
                    return;
                }
            }
            if (!checkBox1.Checked)
            {
                if (originalImage.Width < 128 || originalImage.Height < 128)
                    rawImage = new Bitmap(originalImage, 128, 128);
                else
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


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            convertTask?.Abort();
        }
    }
}