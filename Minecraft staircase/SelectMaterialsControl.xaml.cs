using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minecraft_staircase
{
    /// <summary>
    /// Логика взаимодействия для SelectMaterialsControl.xaml
    /// </summary>
    public partial class SelectMaterialsControl : UserControl
    {
        int _textureSize;

        List<StackPanel> _rows;
        List<int> _values;
        List<List<Border>> _borders;
        List<Color> _borderColors;

        /// <summary>
        /// First arg - row, second arg - column
        /// </summary>
        public event Action<int, int> ItemChanged;

        public SelectMaterialsControl(int textureSize)
        {
            InitializeComponent();
            _rows = new List<StackPanel>();
            _values = new List<int>();
            _borders = new List<List<Border>>();
            _borderColors = new List<Color>();
            _textureSize = textureSize;
        }

        public void AddRow(System.Drawing.Color lightColor) => AddRow(lightColor, System.Drawing.Color.Red);

        public void AddRow(System.Drawing.Color lightColor, System.Drawing.Color borderColor)
        {
            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 3),
                Background = new LinearGradientBrush(new GradientStopCollection
                {
                    new GradientStop(Color.FromRgb(lightColor.R, lightColor.G, lightColor.B), 0),
                    new GradientStop(Color.FromRgb((byte)(lightColor.R * 220 / 255), (byte)(lightColor.G * 220 / 255), (byte)(lightColor.B * 220 / 255)), 0.5),
                    new GradientStop(Color.FromRgb((byte)(lightColor.R * 180 / 255), (byte)(lightColor.G * 180 / 255), (byte)(lightColor.B * 180 / 255)), 1),
                }, new Point(0, 0), new Point(0, 1))
            };
            _rows.Add(sp);
            _mainStack.Children.Add(sp);
            _values.Add(0);
            _borders.Add(new List<Border>());
            _borderColors.Add(Color.FromRgb(borderColor.R, borderColor.G, borderColor.B));
            AddItem(_borders.Count - 1, "None", Properties.Resources.None);
            _mainStack.Height = Height;
        }

        public void AddItem(int row, string name, System.Drawing.Image texture)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            texture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = ms;
            imageSource.EndInit();
            Image img = new Image
            {
                Width = _textureSize,
                Height = _textureSize,
                Source = imageSource,
                Tag = _borders[row].Count,
                ToolTip = name
            };
            img.MouseUp += (sender, args) => SelectItem(row, ((int)((Image)sender).Tag));
            Border border = new Border
            {
                BorderThickness = new Thickness(3),
                Child = img
            };
            _borders[row].Add(border);
            _rows[row].Children.Add(border);
        }

        public void SelectItem(int row, int column)
        {
            _borders[row][_values[row]].BorderBrush = null;
            _values[row] = column;
            _borders[row][column].BorderBrush = new SolidColorBrush(_borderColors[row]);
            ItemChanged?.Invoke(row, column);
        }

        public int Get(int row) => _values[row];
    }
}
