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
        delegate void Progress();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void SaveUses(int* cou);
        [DllImport("ArtGenerator.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int* Convert(int* image, int imageLen, int type, bool chromatic, int* notes, int notesCount, [MarshalAs(UnmanagedType.FunctionPtr)] Progress callback, [MarshalAs(UnmanagedType.FunctionPtr)] SaveUses callback1);

        List<ColorNote> _colors;

        ProgressBar progress;
        Label stateLabel;
        public event Action Done;

        public ArtGenerator(ref List<ColorNote> colors)
        {
            _colors = colors;
        }

        public SettedBlock[,] CreateScheme(ref Image sourceImage, ArtType type, out int maxHeight)
        {
            UnsettedBlock[,] RawScheme = ConvertCPP(ref sourceImage, type);
            stateLabel?.BeginInvoke(new Action(() => { stateLabel.Text = "Making image"; }));
            Bitmap tempImage = new Bitmap(RawScheme.GetLength(0), RawScheme.GetLength(1));
            int gl0 = RawScheme.GetLength(0);
            int gl1 = RawScheme.GetLength(1);
            using (FBitmap fbmp = new FBitmap(tempImage, false))
                for (int i = 0; i < gl0; ++i)
                    for (int j = 0; j < gl1; ++j)
                    {
                        switch (RawScheme[i, j].Set)
                        {
                            case ColorType.Dark:
                                fbmp.SetPixel(i, j, _colors[RawScheme[i, j].ID - 1].DarkColor);
                                break;
                            case ColorType.Normal:
                                fbmp.SetPixel(i, j, _colors[RawScheme[i, j].ID - 1].NormalColor);
                                break;
                            case ColorType.Light:
                                fbmp.SetPixel(i, j, _colors[RawScheme[i, j].ID - 1].LightColor);
                                break;
                        }
                    }
            sourceImage = tempImage;
            stateLabel?.BeginInvoke(new Action(() => { stateLabel.Text = "Generating"; }));
            SchemeGenerator gen = new SchemeGenerator(ref RawScheme);
            SettedBlock[,] result;
            switch (Properties.Settings.Default.GeneratingMethod)
            {
                case 0:
                    result = gen.GenerateFlow(out maxHeight);
                    break;
                case 1:
                    result = gen.GenerateSegmented(out maxHeight);
                    break;
                case 2:
                    result = gen.GenerateMixed(out maxHeight);
                    break;
                default:
                    throw new Exception("Unknown generating method");
            }
            stateLabel?.BeginInvoke(new Action(() => { stateLabel.Text = "Done"; }));
            Done();
            return result;
        }


        UnsettedBlock[,] ConvertCPP(ref Image sourceImage, ArtType type)
        {
            stateLabel?.BeginInvoke(new Action(() => { stateLabel.Text = "Serialization"; }));
            UnsettedBlock[,] result = new UnsettedBlock[sourceImage.Width, sourceImage.Height];
            int[] image1 = new int[sourceImage.Width * sourceImage.Height * 3];
            int h = sourceImage.Height;
            int w = sourceImage.Width;
            using (FBitmap fbmp = new FBitmap((Bitmap)sourceImage, true))
                for (int i = 0; i < h; ++i)
                    for (int j = 0; j < w; ++j)
                    {
                        Color pixel = fbmp.GetPixel(j, i);
                        image1[i * w * 3 + j * 3 + 0] = pixel.R;
                        image1[i * w * 3 + j * 3 + 1] = pixel.G;
                        image1[i * w * 3 + j * 3 + 2] = pixel.B;
                    }
            List<int> notes1 = new List<int>(_colors.Count * 4);
            foreach (ColorNote col in _colors)
                if (col.Use)
                {
                    notes1.Add(col.ColorID);
                    notes1.Add(col.LightColor.R);
                    notes1.Add(col.LightColor.G);
                    notes1.Add(col.LightColor.B);
                }
            stateLabel?.BeginInvoke(new Action(() => { stateLabel.Text = "Converting"; }));
            fixed (int* image = image1)
            fixed (int* notes = notes1.ToArray())
            {
                int* res = Convert(image, image1.Length, (int)type, Properties.Settings.Default.ConvertingMethod == 1, notes, notes1.Count,
                    () => { progress.BeginInvoke(new Action(() => { progress?.Increment(1); })); },
                    (e) =>
                    {
                        int t = 0;
                        for (int j = 0; j < _colors.Count; ++j)
                            if (_colors[j].Use)
                                _colors[j].Uses = e[t++];
                            else
                                _colors[j].Uses = 0;
                    });
                int i = 0;
                for (int x = 0; x < h; ++x)
                    for (int y = 0; y < w; ++y)
                    {
                        result[y, x].ID = res[i++];
                        result[y, x].Set = (ColorType)res[i++];
                    }
            }
            return result;
        }

        public void SetProgress(ProgressBar progress) => this.progress = progress;

        public void SetStateLabel(Label label) => this.stateLabel = label;


        unsafe class FBitmap : IDisposable
        {
            Bitmap _bitmap;
            System.Drawing.Imaging.BitmapData _data;

            int* _scan0;
            int _stride;

            public FBitmap(Bitmap bitmap, bool Read)
            {
                _bitmap = bitmap;
                _data = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, bitmap.Height), 
                    Read ? System.Drawing.Imaging.ImageLockMode.ReadOnly : System.Drawing.Imaging.ImageLockMode.WriteOnly, _bitmap.PixelFormat);
                _scan0 = (int*)_data.Scan0;
                _stride = _data.Stride / 4;
            }

            public void SetPixel(int x, int y, Color color)
            {
                *(uint*)(_scan0 + x + y * _stride) = unchecked((uint)(color.ToArgb()));
            }

            public Color GetPixel(int x, int y)
            {
                return Color.FromArgb(*(_scan0 + x + y * _stride));
            }

            public void Dispose()
            {
                _bitmap.UnlockBits(_data);
            }
        }
    }
}
