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
            //for (int i = 0; i < listBox1.Items.Count; i++)
            //    listBox1.Items[i] = listBox1.Items[i] + Recources[i].ToString();
            using (FileStream fs = new FileStream(@"data\BlockIDS.txt", FileMode.Open))
            {
                StreamReader reader = new StreamReader(fs);
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line[0] != '/' && line[1] != '/')
                        listBox1.Items.Add($"{line.Split('-')[2]} - {Recources[Convert.ToInt32(line.Split('-')[0]) - 1].ToString()}");
                    //listBox1.Items.Add($"{line.Split('-')[1]} - {Recources[Convert.ToInt32(line.Split('-')[0]) - 1].ToString()}");
                    line = reader.ReadLine();
                }
            }
        }
    }
}
