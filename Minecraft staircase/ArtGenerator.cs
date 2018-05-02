using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Minecraft_staircase
{
    unsafe class ArtGenerator
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Progress();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SaveUses(int* cou);
        [DllImport("ArtGenerator.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int* Convert(int* image, int imageLen, int type, bool chromatic, int* notes, int notesCount, [MarshalAs(UnmanagedType.FunctionPtr)] Progress callback, [MarshalAs(UnmanagedType.FunctionPtr)] SaveUses callback1);

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
            Bitmap tempImage = new Bitmap(RawScheme.GetLength(0), RawScheme.GetLength(1));
            for (int i = 0; i < RawScheme.GetLength(0); i++)
                for (int j = 0; j < RawScheme.GetLength(1); j++)
                    switch (RawScheme[i, j].Set)
                    {
                        case ColorType.Dark:
                            tempImage.SetPixel(i, j, _colors.Find((e) => { return e.ColorID == RawScheme[i, j].ID; }).DarkColor);
                            break;
                        case ColorType.Normal:
                            tempImage.SetPixel(i, j, _colors.Find((e) => { return e.ColorID == RawScheme[i, j].ID; }).NormalColor);
                            break;
                        case ColorType.Light:
                            tempImage.SetPixel(i, j, _colors.Find((e) => { return e.ColorID == RawScheme[i, j].ID; }).LightColor);
                            break;
                    }
            sourceImage = tempImage;
            //UnsettedBlock[,] RawScheme = OldConvert(ref sourceImage, type);
            return GenerateFlow(ref RawScheme, out maxHeight);
        }


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
                    int* fet = Convert(image, image1.Count, (int)type, Properties.Settings.Default.ConvertingMethod == 1, notes, notes1.Count,
                        () => { progress.BeginInvoke(Inc); },
                        (e) =>
                        {
                            for (int j = 0; j < _colors.Count; j++)
                                _colors[j].Uses = e[j];
                        });
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
            maxHeight = 0;
            return null;
        }


        public void SetProgress(ProgressBar progress)
        {
            this.progress = progress;
        }
    }
}
