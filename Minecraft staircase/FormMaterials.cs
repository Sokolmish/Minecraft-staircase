using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Minecraft_staircase
{
    public partial class FormMaterials : Form
    {
        public FormMaterials()
        {
            InitializeComponent();
        }

        public void Show(ref List<ColorNote> colorsNote)
        {
            Show();
            foreach (ColorNote col in colorsNote)
                listBox1.Items.Add(col.ResourcesToString());
        }
    }
}
