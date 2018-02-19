using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Minecraft_staircase
{
    class ArtGenerator
    {
        List<PixelData> _colors;
        List<PixelData> _extendedColors;

        ProgressBar progress;
        public delegate void Del();
        public event Del Inc;

        /// <summary>
        /// Initialize new ArtGenerator
        /// </summary>
        /// <param name="colors">List of colors</param>
        public ArtGenerator(List<PixelData> colors, List<PixelData> extendedColors, bool useXYZ)
        {
            _colors = colors;
            _extendedColors = new List<PixelData>();
            for (int i = 0; i < extendedColors.Count; i++)
                _extendedColors.Add(new PixelData(extendedColors[i]));
            Inc += () => { progress?.Increment(1); };
            if (useXYZ)
                ConvertToXYZ();
        }

        /// <summary>
        /// Create array of XYZ colors
        /// </summary>
        void ConvertToXYZ()
        {
            for (int i = 0; i < _colors.Count; i++)
            {
                _colors[i].LightColor = RGBtoXYZ(_colors[i].LightColor);
                _colors[i].DarkColor = RGBtoXYZ(_colors[i].DarkColor);
                _colors[i].NormalColor = RGBtoXYZ(_colors[i].NormalColor);
            }
        }


        /// <summary>
        /// Create array of blocks
        /// </summary>
        /// <param name="sourceImage">Image to create art. After creation will be modified!</param>
        /// <param name="type">Art type</param>
        /// <param name="resources">Array of resources required</param>
        /// <returns></returns>
        public SettedBlock[,] CreateScheme(ref Image sourceImage, ArtType type, out int[] resources, out int maxHeight)
        {
            UnsettedBlock[,] RawScheme = new UnsettedBlock[sourceImage.Width, sourceImage.Height];
            Bitmap tempImage = sourceImage as Bitmap;
            resources = new int[_extendedColors.Count];
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    int betterID = 0;
                    ColorType betterSet = ColorType.Normal;
                    double betterSimilarity = 1000;
                    foreach (PixelData col in _colors)
                    {
                        if (Similarity(col.NormalColor[0], col.NormalColor[1], col.NormalColor[2],
                                tempImage.GetPixel(i, j).R, tempImage.GetPixel(i, j).G, tempImage.GetPixel(i, j).B)
                                < betterSimilarity)
                        {
                            betterID = col.ID;
                            betterSet = ColorType.Normal;
                            betterSimilarity = Similarity(col.NormalColor[0], col.NormalColor[1], col.NormalColor[2],
                                tempImage.GetPixel(i, j).R, tempImage.GetPixel(i, j).G, tempImage.GetPixel(i, j).B);
                        }
                        if (type != ArtType.Flat)
                        {
                            if (Similarity(col.LightColor[0], col.LightColor[1], col.LightColor[2],
                                tempImage.GetPixel(i, j).R, tempImage.GetPixel(i, j).G, tempImage.GetPixel(i, j).B)
                                < betterSimilarity)
                            {
                                betterID = col.ID;
                                betterSet = ColorType.Light;
                                betterSimilarity = Similarity(col.LightColor[0], col.LightColor[1], col.LightColor[2],
                                    tempImage.GetPixel(i, j).R, tempImage.GetPixel(i, j).G, tempImage.GetPixel(i, j).B);
                            }
                            if (type != ArtType.Lite)
                            {
                                if (Similarity(col.DarkColor[0], col.DarkColor[1], col.DarkColor[2],
                                    tempImage.GetPixel(i, j).R, tempImage.GetPixel(i, j).G, tempImage.GetPixel(i, j).B)
                                    < betterSimilarity)
                                {
                                    betterID = col.ID;
                                    betterSet = ColorType.Dark;
                                    betterSimilarity = Similarity(col.DarkColor[0], col.DarkColor[1], col.DarkColor[2],
                                        tempImage.GetPixel(i, j).R, tempImage.GetPixel(i, j).G, tempImage.GetPixel(i, j).B);
                                }
                            }
                        }
                    }
                    RawScheme[i, j].ID = betterID;
                    RawScheme[i, j].Set = betterSet;
                    double[] arrColor;
                    switch (betterSet)
                    {
                        case ColorType.Dark:
                            arrColor = _extendedColors.Find((e) => { return e.ID == betterID; }).DarkColor;
                            tempImage.SetPixel(i, j, Color.FromArgb((int)arrColor[0], (int)arrColor[1], (int)arrColor[2]));
                            break;
                        case ColorType.Normal:
                            arrColor = _extendedColors.Find((e) => { return e.ID == betterID; }).NormalColor;
                            tempImage.SetPixel(i, j, Color.FromArgb((int)arrColor[0], (int)arrColor[1], (int)arrColor[2]));
                            break;
                        case ColorType.Light:
                            arrColor = _extendedColors.Find((e) => { return e.ID == betterID; }).LightColor;
                            tempImage.SetPixel(i, j, Color.FromArgb((int)arrColor[0], (int)arrColor[1], (int)arrColor[2]));
                            break;
                    }
                    ++resources[betterID - 1];
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
        double Similarity(double r1, double g1, double b1, double r2, double g2, double b2) =>
            Math.Sqrt(Math.Pow(r2 - r1, 2) + Math.Pow(g2 - g1, 2) + Math.Pow(b2 - b1, 2));


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
