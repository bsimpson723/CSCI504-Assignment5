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
            SortGameBoardControls();
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
            GameTimer.Start();
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

        private void LoadGame(string difficulty)
        {
            if (AnyInProgress(difficulty))
            {
                LoadInProgressGame(difficulty);
            }
            else
            {
                LoadNewGame(difficulty);
            }

            for (var i = 0; i < 81; i++)
            {
                if (m_puzzle.Start[i].ToString() != "0")
                {
                    GameBoard.Controls[i].Text = m_puzzle.Start[i].ToString();
                    ((TextBox)GameBoard.Controls[i]).ReadOnly = true;
                }
            }

            PauseButton.Enabled = true;
            ProgressButton.Enabled = true;
            SaveButton.Enabled = true;
            ClearButton.Enabled = true;
        }

        private void LoadInProgressGame(string difficulty)
        {
            var progressDirectoryPath = "./InProgress/" + difficulty + "/";

            DirectoryInfo directoryInfo = new DirectoryInfo(progressDirectoryPath);
            var fileInfo = directoryInfo.GetFiles().FirstOrDefault();
            m_puzzle.Name = fileInfo.Name;
            m_puzzle.Difficulty = difficulty;

            var newFileName = progressDirectoryPath + m_puzzle.Name;
            var puzzle = File.ReadAllText(newFileName);
            var puzzleFields = puzzle.Split('\t');

            m_puzzle.Start = puzzleFields[1];
            m_puzzle.Progress = puzzleFields[1]; //yes, we want the start and progress to be the same on initial load
            m_puzzle.Solution = puzzleFields[2];
            m_puzzle.Time = Convert.ToInt32(puzzleFields[3]);
        }

        private void LoadNewGame(string difficulty)
        {
            var newDirectoryPath = "./New/" + difficulty + "/";
            var solutionDirectoryPath = "./Solutions/";

            DirectoryInfo directoryInfo = new DirectoryInfo(newDirectoryPath);
            var fileInfo = directoryInfo.GetFiles().FirstOrDefault();
            m_puzzle.Name = fileInfo.Name;
            m_puzzle.Difficulty = difficulty;

            var newFileName = newDirectoryPath + m_puzzle.Name;
            var startString = File.ReadAllText(newFileName);
            m_puzzle.Start = startString.Replace(Environment.NewLine, "");

            m_puzzle.Progress = m_puzzle.Start; //yes, we want the start and progress to be the same on initial load

            var solutionFileName = solutionDirectoryPath + m_puzzle.Name;
            var solutionString = File.ReadAllText(solutionFileName);
            m_puzzle.Solution = solutionString.Replace(Environment.NewLine, "");
        }

        private bool AnyInProgress(string difficulty)
        {
            var progressDirectoryPath = "./InProgress/" + difficulty + "/";
            if (Directory.Exists(progressDirectoryPath))
            {
                var directoryInfo = new DirectoryInfo(progressDirectoryPath);
                if (directoryInfo.GetFiles().Any())
                {
                    return true;
                }
            }
            return false;
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
        
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            m_puzzle.Time++;
            TimerLabel.Text = String.Format("{0:00}:{1:00}:{2:00}", m_puzzle.Hours, m_puzzle.Minutes, m_puzzle.Seconds);
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            Button pauseButton = (Button)sender;
            if (pauseButton.Text == "Pause")
            {
                GameTimer.Stop();
                pauseButton.Text = "Resume";

                foreach (TextBox txt in GameBoard.Controls)
                {
                    if (txt.ReadOnly)
                    {
                        txt.ReadOnly = false;
                        txt.BackColor = Color.FromArgb(240, 240, 240);
                        txt.ForeColor = Color.FromArgb(240, 240, 240);
                    }
                    else
                    {
                        txt.ForeColor = Color.Transparent;
                    }
                }
            } else if (pauseButton.Text == "Resume")
            {
                GameTimer.Start();
                pauseButton.Text = "Pause";

                foreach (TextBox txt in GameBoard.Controls)
                {
                    if (txt.ForeColor != Color.Transparent)
                    {
                        txt.ForeColor = Color.Black;
                        txt.ReadOnly = true;
                    }
                    else
                    {
                        txt.ForeColor = Color.Black;
                    }
                }
            }
        }
    }
}
