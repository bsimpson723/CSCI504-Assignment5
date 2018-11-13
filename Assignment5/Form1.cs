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
        private TextBox m_currentCell;
        private string m_sessionProgress;

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

        private void Cell_Click(object sender, EventArgs e)
        {
            var textBox = (TextBox) sender;
            m_currentCell = textBox;
            resetActive();
            if (!textBox.ReadOnly)
            {
                textBox.BackColor = SystemColors.GradientActiveCaption;
            }
            EasyButton.Focus();
        }

        private void resetActive()
        {
            foreach (TextBox control in GameBoard.Controls)
            {
                control.BackColor = Color.White;
            }
        }

        private void UpdateProgress()
        {
            var builder = new StringBuilder();
            foreach (TextBox cell in GameBoard.Controls)
            {
                
                if (string.IsNullOrEmpty(cell.Text))
                {
                    builder.Append(0);
                }
                else
                {
                    builder.Append(cell.Text);
                }
            }
            m_sessionProgress = builder.ToString();
            CheckForWinner();
        }

        private void CheckForWinner()
        {
            if (m_sessionProgress == m_puzzle.Solution)
            {
                GameTimer.Stop();
                MessageBox.Show("Congratulations, you win!");
                Directory.CreateDirectory("./Solved/");
                var saveFileName = "./Solved/" + m_puzzle.Name;
                File.WriteAllText(saveFileName, m_puzzle.ToString());
                File.Delete("./InProgress/" + m_puzzle.Difficulty + "/" + m_puzzle.Name);
            }
        }

        private void SetCellValue(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 49 && e.KeyChar <= 57 && m_currentCell != null & m_currentCell.ReadOnly != true)
            {
                m_currentCell.Text = e.KeyChar.ToString();
                UpdateProgress();
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory("./Inprogress/" + m_puzzle.Difficulty);
            var saveFileName = "./InProgress/" + m_puzzle.Difficulty + "/" + m_puzzle.Name;
            m_puzzle.Progress = m_sessionProgress;
            File.WriteAllText(saveFileName, m_puzzle.ToString());
            MessageBox.Show("Your puzzle progress was saved successfully!");
        }

        private void ClearGameBoard()
        {
            foreach (TextBox control in GameBoard.Controls)
            {
                control.Font = new Font(control.Font, FontStyle.Regular);
                control.Text = "";
                control.ReadOnly = false;
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
                    GameBoard.Controls[i].Font = new Font(GameBoard.Controls[i].Font, FontStyle.Bold);
                    ((TextBox)GameBoard.Controls[i]).ReadOnly = true;
                }
                if (m_puzzle.Progress[i].ToString() != "0")
                {
                    GameBoard.Controls[i].Text = m_puzzle.Progress[i].ToString();
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

            m_puzzle.Start = puzzleFields[0];
            m_puzzle.Progress = puzzleFields[1];
            m_sessionProgress = puzzleFields[1];
            m_puzzle.Solution = puzzleFields[2];
            m_puzzle.Time = Convert.ToInt32(puzzleFields[3]);
        }

        private void LoadNewGame(string difficulty)
        {
            var newDirectoryPath = "./New/" + difficulty + "/";
            var solutionDirectoryPath = "./Solutions/";

            DirectoryInfo directoryInfo = new DirectoryInfo(newDirectoryPath);
            var files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                if (Solved(file.Name))
                {
                    continue;
                }

                m_puzzle.Name = file.Name;
                m_puzzle.Difficulty = difficulty;

                var newFileName = newDirectoryPath + m_puzzle.Name;
                var startString = File.ReadAllText(newFileName);
                m_puzzle.Time = 0;
                m_puzzle.Start = startString.Replace(Environment.NewLine, "").Replace("\n", string.Empty);  // we want these three fields that same when loading a new puzzle
                m_puzzle.Progress = m_puzzle.Start;                             // we want these three fields that same when loading a new puzzle
                m_sessionProgress = m_puzzle.Start;                             // we want these three fields that same when loading a new puzzle

                var solutionFileName = solutionDirectoryPath + m_puzzle.Name;
                var solutionString = File.ReadAllText(solutionFileName);
                m_puzzle.Solution = solutionString.Replace(Environment.NewLine, "").Replace("\n", string.Empty);
                return;
            }
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

        private bool Solved(string puzzleName)
        {
            var solvedDirectoryPath = "./Solved/";
            if (!Directory.Exists(solvedDirectoryPath))
            {
                return false;
            }
            var directoryInfo = new DirectoryInfo(solvedDirectoryPath);
            var files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                if (file.Name == puzzleName)
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
        
        private void ClearButton_Click(object sender, EventArgs e)
        {
            foreach (TextBox txt in GameBoard.Controls)
            {
                if (!txt.ReadOnly)
                {
                    txt.Text = "";
                }
            }

            m_puzzle.Time = 0;
        }

        private void ProgressButton_Click(object sender, EventArgs e)
        {
            int[] row = new int[9];
            int[] column = new int[9];
            for (var i = 0; i < 9; i++)
            {
                for (int t = 0; t < row.Length; t++)
                {
                    row[t] = -1;
                    column[t] = -1;
                }

                for (int j = 0; j < 9; j++)
                {
                    var currentInput1 = GameBoard.Controls[i * 9 + j].Text;
                    if (currentInput1 != "")
                    {
                        if (row[Int32.Parse(currentInput1) - 1] == -1)
                        {
                            row[Int32.Parse(currentInput1) - 1] = i * 9 + j;
                        }
                        else
                        {
                            for (int t = 0; t < 9; t++)
                            {
                                GameBoard.Controls[i * 9 + t].BackColor = Color.Red;
                            }
                            continue;
                        }
                    }

                    var currentInput2 = GameBoard.Controls[j * 9 + i].Text;
                    if (currentInput2 != "")
                    {
                        if (column[Int32.Parse(currentInput2) - 1] == -1)
                        {
                            column[Int32.Parse(currentInput2) - 1] = j * 9 + i;
                        }
                        else
                        {
                            for (int t = 0; t < 9; t++)
                            {
                                GameBoard.Controls[t * 9 + i].BackColor = Color.Red;
                            }
                            continue;
                        }
                    }

                }
            }

            var result = true;
            var emptyCell = 0;
            for (var i = 0; i < 81; i++)
            {
                var currentInput = GameBoard.Controls[i].Text;
                if (currentInput == "")
                {
                    emptyCell++;
                }
                else if (m_puzzle.Solution[i].ToString() != currentInput)
                {
                    GameBoard.Controls[i].BackColor = Color.Red;
                    result = false;
                }
            }

            if (result)
            {
                MessageBox.Show("You're doing well so far! " + emptyCell.ToString() + " remaining cells need defining.");
            }

            GameBoard.Invalidate();
        }
    }
}
