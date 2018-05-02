using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Minecraft_staircase
{
    unsafe class ArtGenerator
    {
        List<ColorNote> _colors;

        ProgressBar progress;
        public event Action Inc;

        public ArtGenerator(ref List<ColorNote> colors)
        {
            _colors = colors;
            Inc += () => { progress?.Increment(1); };
        }

        public SettedBlock[,] CreateScheme(ref Image sourceImage, ArtType type, out int maxHeight)
        {
            UnsettedBlock[,] RawScheme = ConvertCPP(ref sourceImage, type);
            //UnsettedBlock[,] RawScheme = OldConvert(ref sourceImage, type);
            return GenerateFlow(ref RawScheme, out maxHeight);
        }


        UnsettedBlock[,] OldConvert(ref Image sourceImage, ArtType type)
        {
            UnsettedBlock[,] RawScheme = new UnsettedBlock[sourceImage.Width, sourceImage.Height];
            Bitmap tempImage = sourceImage as Bitmap;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    int betterID = 0;
                    ColorType betterSet = ColorType.Normal;
                    double betterSimilarity = 1000;
                    foreach (ColorNote col in _colors)
                    {
                        if (!col.Use)
                            continue;
                        if (Similarity(col.NormalColor, tempImage.GetPixel(i, j))
                                < betterSimilarity)
                        {
                            betterID = col.ColorID;
                            betterSet = ColorType.Normal;
                            betterSimilarity = Similarity(col.NormalColor, tempImage.GetPixel(i, j));
                        }
                        if (type != ArtType.Flat)
                        {
                            if (Similarity(col.LightColor, tempImage.GetPixel(i, j))
                                < betterSimilarity)
                            {
                                betterID = col.ColorID;
                                betterSet = ColorType.Light;
                                betterSimilarity = Similarity(col.LightColor, tempImage.GetPixel(i, j));
                            }
                            if (type != ArtType.Lite)
                            {
                                if (Similarity(col.DarkColor, tempImage.GetPixel(i, j))
                                    < betterSimilarity)
                                {
                                    betterID = col.ColorID;
                                    betterSet = ColorType.Dark;
                                    betterSimilarity = Similarity(col.DarkColor, tempImage.GetPixel(i, j));
                                }
                            }
                        }
                    }
                    RawScheme[i, j].ID = betterID;
                    RawScheme[i, j].Set = betterSet;
                    switch (betterSet)
                    {
                        case ColorType.Dark:
                            tempImage.SetPixel(i, j, _colors.Find((e) => { return e.ColorID == betterID; }).DarkColor);
                            break;
                        case ColorType.Normal:
                            tempImage.SetPixel(i, j, _colors.Find((e) => { return e.ColorID == betterID; }).NormalColor);
                            break;
                        case ColorType.Light:
                            tempImage.SetPixel(i, j, _colors.Find((e) => { return e.ColorID == betterID; }).LightColor);
                            break;
                    }
                    _colors.Find((e) => { return e.ColorID == betterID; }).Uses++;
                    progress.BeginInvoke(Inc);
                }
            }
            sourceImage = tempImage;
            return RawScheme;
        }

        SettedBlock[,] GenerateFlow(ref UnsettedBlock[,] RawScheme, out int maxHeight) //EveryMap shift
        {
            SettedBlock[,] BlockMap = new SettedBlock[RawScheme.GetLength(0), RawScheme.GetLength(1) + 1];
            for (int i = 0; i < RawScheme.GetLength(0); i++)
                BlockMap[i, 0] = new SettedBlock() { ID = -1, Height = 0 };
            maxHeight = 0;
            for (int i = 0; i < RawScheme.GetLength(0); i++)
            {
                int minHeight = 0;
                for (int j = 1; j < RawScheme.GetLength(1) + 1; j++)
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
                    if (j != 1 && (j - 1) % 128 == 0)
                        BlockMap[i, j].Height = 0;
                    minHeight = BlockMap[i, j].Height < minHeight ? BlockMap[i, j].Height : minHeight;
                }
                for (int j = 0; j < RawScheme.GetLength(1) + 1; j++)
                {
                    BlockMap[i, j].Height = BlockMap[i, j].Height - minHeight;
                    maxHeight = BlockMap[i, j].Height > maxHeight ? BlockMap[i, j].Height : maxHeight;
                }
            }
            return BlockMap;
        }

        SettedBlock[,] GenerateMinimal(ref UnsettedBlock[,] RawScheme, out int maxHeight)
        {
            SettedBlock[,] BlockMap = new SettedBlock[RawScheme.GetLength(0), RawScheme.GetLength(1) + 1];
            for (int i = 0; i < RawScheme.GetLength(0); i++)
                BlockMap[i, 0] = new SettedBlock() { ID = -1, Height = 0 };
            maxHeight = 0;
            for (int i = 0; i < RawScheme.GetLength(0); i++)
            {
                int minHeight = 0;
                for (int j = 1; j < RawScheme.GetLength(1) + 1; j++)
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
                    if (Properties.Settings.Default.GeneratingMethod == 0 && j != 1 && (j - 1) % 128 == 0)
                        BlockMap[i, j].Height = 0;
                    minHeight = BlockMap[i, j].Height < minHeight ? BlockMap[i, j].Height : minHeight;
                }
                for (int j = 0; j < RawScheme.GetLength(1) + 1; j++)
                {
                    BlockMap[i, j].Height = BlockMap[i, j].Height - minHeight;
                    maxHeight = BlockMap[i, j].Height > maxHeight ? BlockMap[i, j].Height : maxHeight;
                }
            }
            return BlockMap;
        }

        double Similarity(Color col1, Color col2)
        {
            if (Properties.Settings.Default.ConvertingMethod == 0)
                return Math.Sqrt(Math.Pow(col2.R - col1.R, 2) + Math.Pow(col2.G - col1.G, 2) + Math.Pow(col2.B - col1.B, 2));
            else //if (Properties.Settings.Default.ConvertingMethod == 1)
            {
                double[] color1 = RGBtoXYZ(new double[] { col1.R, col1.G, col1.B });
                double[] color2 = RGBtoXYZ(new double[] { col2.R, col2.G, col2.B });
                return Math.Sqrt(Math.Pow(color2[0] - color1[0], 2) + Math.Pow(color2[1] - color1[1], 2) + Math.Pow(color2[2] - color1[2], 2));
            }
        }

        double[] RGBtoXYZ(double[] rgbColor)
        {
            return new double[] {
            0.4124564 * rgbColor[0] + 0.3575761 * rgbColor[1] + 0.1804375 * rgbColor[2],
            0.2126729 * rgbColor[0] + 0.7151522 * rgbColor[1] + 0.0721750 * rgbColor[2],
            0.0193339 * rgbColor[0] + 0.1191920 * rgbColor[1] + 0.9503041 * rgbColor[2]};
        }

        public void SetProgress(ProgressBar progress)
        {
            this.progress = progress;
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Progress();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SaveUses(int cou);
        [DllImport("ArtGenerator.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int* Convert(int* image, int imageLen, int type, bool chromatic, int* notes, int notesCount, [MarshalAs(UnmanagedType.FunctionPtr)] Progress callback, [MarshalAs(UnmanagedType.FunctionPtr)] SaveUses callback1);

        UnsettedBlock[,] ConvertCPP(ref Image sourceImage, ArtType type)
        {
            UnsettedBlock[,] result = new UnsettedBlock[sourceImage.Width, sourceImage.Height];
            List<int> image1 = new List<int>(sourceImage.Width * sourceImage.Height * 3);
            for (int i = 0; i < sourceImage.Height; i++)
                for (int j = 0; j < sourceImage.Width; j++)
                {
                    image1.Add((sourceImage as Bitmap).GetPixel(j, i).R);
                    image1.Add((sourceImage as Bitmap).GetPixel(j, i).G);
                    image1.Add((sourceImage as Bitmap).GetPixel(j, i).B);
                }
            fixed (int* image = image1.ToArray())
            {
                List<int> notes1 = new List<int>(_colors.Count * 4);
                foreach (ColorNote col in _colors)
                {
                    notes1.Add(col.ColorID);
                    notes1.Add(col.LightColor.R);
                    notes1.Add(col.LightColor.G);
                    notes1.Add(col.LightColor.B);
                }
                fixed (int* notes = notes1.ToArray())
                {
                    int* fet = 
                    Convert(image, image1.Count, 0, false, notes, notes1.Count,
                        () => { progress.BeginInvoke(Inc); }, /*MessageBox.Show("Soul eather");*/
                        (e) => { MessageBox.Show(e.ToString()); });
                    int i = 0;
                    for (int x = 0; x < sourceImage.Height; x++)
                        for (int y = 0; y < sourceImage.Width; y++)
                        {
                            result[y, x].ID = fet[i++];
                            result[y, x].Set = (ColorType)fet[i++];
                        }
                }
            }
            return result;
        }
    }
}
