using System;
using System.Windows.Forms;
using System.IO;

namespace Minecraft_staircase
{
    public partial class FormMaterials : Form
    {
        int[] Recources;

        public FormMaterials()
        {
            InitializeComponent();
        }

        public void Show(int[] Recources)
        {
            this.Recources = Recources;
            Show();
            //using (FileStream fs = new FileStream(@"data\BlockIDS.txt", FileMode.Open))
            using (FileStream fs = new FileStream(@"data\PossibleBlocks.txt", FileMode.Open))
            {
                StreamReader reader = new StreamReader(fs);
                string line = reader.ReadLine();
                int id = 1;
                while (line != null)
                {
                    line = line.Split('~')[1].Split(',')[0];
                    if (line[0] != '/' && line[1] != '/')
                        listBox1.Items.Add($"{line.Split('-')[1]} - {Recources[id++ - 1]}");
                    line = reader.ReadLine();
                }
            }
        }
    }
}
