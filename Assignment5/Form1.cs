using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment5
{
    public partial class Form1 : Form
    {
        private string puzzleName;
        private string puzzleStart;
        private string puzzleSolution;

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

        private void LoadGame_Click(object sender, EventArgs e)
        {
            var button = (Button) sender;
            ClearGameBoard();
            LoadGame(button.Text);
        }

        private void ClearGameBoard()
        {
            foreach (TextBox control in GameBoard.Controls)
            {
                control.Text = "";
                control.Enabled = true;
            }
        }

        private void LoadGame(string folder)
        {
            var newDirectoryPath = "./New/" + folder + "/";
            var solutionDirectoryPath = "./Solutions/";

            DirectoryInfo directoryInfo = new DirectoryInfo(newDirectoryPath);
            var fileInfo = directoryInfo.GetFiles().FirstOrDefault();
            puzzleName = fileInfo.Name;

            var newFileName = newDirectoryPath + puzzleName;
            puzzleStart = File.ReadAllText(newFileName);

            var solutionFileName = solutionDirectoryPath + puzzleName;
            puzzleSolution = File.ReadAllText(solutionFileName);

            SortGameBoardControls();

            for (var i = 0; i < 81; i++)
            {
                if (puzzleStart[i].ToString() != "0" && puzzleStart[i].ToString() != "\r" && puzzleStart[i].ToString() != "\n")
                {
                    GameBoard.Controls[i].Text = puzzleStart[i].ToString();
                    GameBoard.Controls[i].Enabled = false;
                }
            }
        }

        public void SortGameBoardControls()
        {

            IEnumerable<TextBox> sortedlist =
                from muc in GameBoard.Controls.Cast<TextBox>()
                orderby muc.Name
                select muc;

            int counter = 0;
            foreach (TextBox muc in sortedlist)
            {
                GameBoard.Controls.SetChildIndex(muc, counter);
                counter++;
            }

        }
    }
}
