using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Minecraft_staircase
{
    public partial class FormSelectMaterials : Form
    {
        List<ColorNote> _colorsList;
        Dictionary<string, Image> _images;

        System.Windows.Forms.Integration.ElementHost _host;

        SelectMaterialsControl control;

        public FormSelectMaterials()
        {
            InitializeComponent();
            control = new SelectMaterialsControl(64);
            _host = new System.Windows.Forms.Integration.ElementHost
            {
                Child = control,
                Margin = new Padding(10),
                AutoSize = true
            };
            Controls.Add(_host);
            LoadTextures();
        }

        void LoadTextures()
        {
            _images = new Dictionary<string, Image>(178);
            using (MemoryStream stream = new MemoryStream(Properties.Resources.textures))
            {
                while (stream.Position < stream.Length)
                {
                    byte[] buff = new byte[2];
                    stream.Read(buff, 0, 2);
                    buff = new byte[BitConverter.ToUInt16(buff, 0)];
                    stream.Read(buff, 0, buff.Length);
                    string name = System.Text.Encoding.UTF8.GetString(buff);
                    buff = new byte[2];
                    stream.Read(buff, 0, 2);
                    buff = new byte[BitConverter.ToUInt16(buff, 0)];
                    stream.Read(buff, 0, buff.Length);
                    Image img = Image.FromStream(new MemoryStream(buff));
                    _images.Add(name, img);
                }
            }
        }

        public void ShowDialog(ref List<ColorNote> colorsList)
        {
            _colorsList = colorsList;
            for (int i = 0; i < _colorsList.Count; ++i)
            {
                if (i == 3)
                    control.AddRow(_colorsList[i].LightColor, Color.Blue);
                else
                    control.AddRow(_colorsList[i].LightColor);
                if (!_colorsList[i].Use)
                    control.SelectItem(i, 0);
                for (int j = 0; j < _colorsList[i].PossibleBlocks.Count; ++j)
                {
                    control.AddItem(i, _colorsList[i].PossibleBlocks[j].Name, _images[_colorsList[i].PossibleBlocks[j].TextureName]);
                    if (_colorsList[i].Use && _colorsList[i].PossibleBlocks[j].Equals(_colorsList[i].SelectedBlock))
                        control.SelectItem(i, j + 1);
                }
            }
            base.ShowDialog();
        }

        void WriteFile()
        {
            string save = string.Empty;
            foreach (ColorNote col in _colorsList)
                save += col.DataToString() + "\r\n";
            Properties.Settings.Default.PossibleBlocks = save;
            Properties.Settings.Default.Save();
        }


        private void FormSelectMaterials_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < _colorsList.Count; ++i)
                if (control.Get(i) == 0)
                    _colorsList[i].Use = false;
                else
                {
                    _colorsList[i].SelectedBlock = _colorsList[i].PossibleBlocks[control.Get(i) - 1];
                    _colorsList[i].Use = true;
                }
            WriteFile();
            GC.Collect();
        }
    }
}
