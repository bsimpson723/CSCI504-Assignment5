using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GameBoard_Paint(object sender, PaintEventArgs e)
        {
            var pen = new Pen(Color.Black, 4);
            var graphics = e.Graphics;
            graphics.DrawLine(pen, 126, 0, 126, 379);
            graphics.DrawLine(pen, 251, 0, 251, 379);
            graphics.DrawLine(pen, 1, 0, 1, 379);
            graphics.DrawLine(pen, 0, 1, 379, 1);
            graphics.DrawLine(pen, 375, 0, 375, 379);
            graphics.DrawLine(pen, 0, 375, 379, 375);
            graphics.DrawLine(pen, 0, 126, 379, 126);
            graphics.DrawLine(pen, 0, 251, 379, 251);
        }
    }
}
