using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Minecraft_staircase
{
    class ArtGenerator
    {
        List<ColorNote> _colors;

        ProgressBar progress;
        public delegate void Del();
        public event Del Inc;

        /// <summary>
        /// Initialize new ArtGenerator
        /// </summary>
        /// <param name="colors">List of colors</param>
        public ArtGenerator(ref List<ColorNote> colors)
        {
            _colors = colors;
            Inc += () => { progress?.Increment(1); };
        }


        /// <summary>
        /// Create array of blocks
        /// </summary>
        /// <param name="sourceImage">Image to create art. After creation will be modified!</param>
        /// <param name="type">Art type</param>
        /// <param name="resources">Array of resources required</param>
        /// <returns></returns>
        public SettedBlock[,] CreateScheme(ref Image sourceImage, ArtType type, out int maxHeight)
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
            SettedBlock[,] BlockMap = new SettedBlock[sourceImage.Width, sourceImage.Height + 1];
            for (int i = 0; i < sourceImage.Width; i++)
                BlockMap[i, 0] = new SettedBlock() { ID = -1, Height = 0 };
            maxHeight = 0;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                int minHeight = 0;
                for (int j = 1; j < sourceImage.Height + 1; j++)
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
                    if (j != 1 && j - 1 % 128 == 0) BlockMap[i, j].Height = 0;
                    minHeight = BlockMap[i, j].Height < minHeight ? BlockMap[i, j].Height : minHeight;
                }
                for (int j = 0; j < sourceImage.Height + 1; j++)
                {
                    BlockMap[i, j].Height = BlockMap[i, j].Height - minHeight;
                    maxHeight = BlockMap[i, j].Height > maxHeight ? BlockMap[i, j].Height : maxHeight;
                }
            }
            return BlockMap;
        }


        /// <summary>
        /// Get similarity of 2 colors
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="g1"></param>
        /// <param name="b1"></param>
        /// <param name="r2"></param>
        /// <param name="g2"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        double Similarity(Color col1, Color col2) =>
            Math.Sqrt(Math.Pow(col2.R - col1.R, 2) + Math.Pow(col2.G - col1.G, 2) + Math.Pow(col2.B - col1.B, 2));


        /// <summary>
        /// Get chromatics coordinates of color
        /// </summary>
        /// <param name="rgbColor">Color in RGB</param>
        /// <returns>Color in XYZ scheme</returns>
        double[] RGBtoXYZ(double[] rgbColor)
        {
            return new double[] {
            0.4124564 * rgbColor[0] + 0.3575761 * rgbColor[1] + 0.1804375 * rgbColor[2],
            0.2126729 * rgbColor[0] + 0.7151522 * rgbColor[1] + 0.0721750 * rgbColor[2],
            0.0193339 * rgbColor[0] + 0.1191920 * rgbColor[1] + 0.9503041 * rgbColor[2]};
        }


        /// <summary>
        /// Add progressBar
        /// </summary>
        /// <param name="progress">ProgressBar</param>
        public void SetProgress(ProgressBar progress)
        {
            this.progress = progress;
        }
    }
}
