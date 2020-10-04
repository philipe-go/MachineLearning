using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageClassificationProgram
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        private void loadImagebtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openImg = new OpenFileDialog();
            openImg.ShowDialog();
            pictureBox.ImageLocation = openImg.FileName != null ? openImg.FileName : null;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            pictureBox.ImageLocation = null;
        }

        private void classifyBtn_Click(object sender, EventArgs e)
        {
            classificationTB.Text = "unknown";
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
