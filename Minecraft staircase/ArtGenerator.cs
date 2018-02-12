using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Minecraft_staircase
{
    class ArtGenerator
    {
        List<PixelData> _colors;
        List<PixelData> _extendedColor;

        ProgressBar progress;
        public delegate void Del();
        public event Del Inc;

        /// <summary>
        /// Initialize new ArtGenerator
        /// </summary>
        /// <param name="colors">List of colors</param>
        public ArtGenerator(List<PixelData> colors, List<PixelData> extendedColors)
        {
            _colors = colors;
            _extendedColor = extendedColors;
            progress = null;
            Inc += () => { progress?.Increment(1); };
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
            resources = new int[_extendedColor.Count];
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    int betterID = 0;
                    ColorType betterSet = ColorType.Normal;
                    double betterSimilarity = 1000;
                    Color selectedColor = new Color();
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
                            selectedColor = Color.FromArgb(col.NormalColor[0], col.NormalColor[1], col.NormalColor[2]);
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
                                selectedColor = Color.FromArgb(col.LightColor[0], col.LightColor[1], col.LightColor[2]);
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
                                    selectedColor = Color.FromArgb(col.DarkColor[0], col.DarkColor[1], col.DarkColor[2]);
                                }
                            }
                        }
                    }
                    RawScheme[i, j].ID = betterID;
                    RawScheme[i, j].Set = betterSet;
                    tempImage.SetPixel(i, j, selectedColor);
                    resources[betterID - 1]++;
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
        /// Create array of blocks
        /// </summary>
        /// <param name="sourceImage">Image to create art. After creation will be modified!</param>
        /// <param name="type">Art type</param>
        /// <returns></returns>
        public SettedBlock[,] CreateScheme(ref Image sourceImage, ArtType type, out int maxHeight) => 
            (CreateScheme(ref sourceImage, type, out int[] temp, out maxHeight));

        /// <summary>
        /// Add progressBar
        /// </summary>
        /// <param name="progress">ProgressBar</param>
        public void SetProgress(ProgressBar progress)
        {
            this.progress = progress;
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
        double Similarity(int r1, int g1, int b1, int r2, int g2, int b2) =>
            Math.Sqrt(Math.Pow(r2 - r1, 2) + Math.Pow(g2 - g1, 2) + Math.Pow(b2 - b1, 2));

        #region Types
        
        #endregion
    }
}
