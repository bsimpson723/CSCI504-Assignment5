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
        private Puzzle m_puzzle;

        public Form1()
        {
            InitializeComponent();
            m_puzzle = new Puzzle();
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

        private void Save_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory("./Inprogress/" + m_puzzle.Difficulty);
            var saveFileName = "./InProgress/" + m_puzzle.Difficulty + "/" + m_puzzle.Name;
            File.WriteAllText(saveFileName, m_puzzle.ToString());
            MessageBox.Show("Your puzzle progress was saved successfully!");
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
            m_puzzle.Name = fileInfo.Name;

            var newFileName = newDirectoryPath + m_puzzle.Name;
            var startString = File.ReadAllText(newFileName);
            m_puzzle.Start = startString.Replace(Environment.NewLine, "");

            m_puzzle.Progress = m_puzzle.Start; //Might need to change this, haven't decided yet

            var solutionFileName = solutionDirectoryPath + m_puzzle.Name;
            var solutionString = File.ReadAllText(solutionFileName);
            m_puzzle.Solution = solutionString.Replace(Environment.NewLine, "");

            SortGameBoardControls();

            for (var i = 0; i < 81; i++)
            {
                if (m_puzzle.Start[i].ToString() != "0")
                {
                    GameBoard.Controls[i].Text = m_puzzle.Start[i].ToString();
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
