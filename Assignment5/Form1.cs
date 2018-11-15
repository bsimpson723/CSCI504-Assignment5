using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Assignment5
{
    public partial class Form1 : Form
    {
        private Puzzle m_puzzle;
        private TextBox m_currentCell;
        private string m_sessionProgress;
        private bool m_paused;


        public Form1()
        {
            InitializeComponent();
            m_puzzle = new Puzzle();
            SortGameBoardControls();
        }

        #region EventHandlers
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
            if (m_sessionProgress != m_puzzle.Progress)
            {
                var response = MessageBox.Show("Would you like to save your changes?", "Unsaved changes!", MessageBoxButtons.YesNoCancel);
                if (response == DialogResult.Cancel)
                {
                    return;
                }
                if (response == DialogResult.Yes)
                {
                    SaveProgress();
                }
            }
            var button = (Button) sender;
            ClearGameBoard();
            LoadGame(button.Text);
            GameTimer.Start();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            Button pauseButton = (Button)sender;
            PauseGame(pauseButton);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearProgress();
            
        }

        private void ProgressButton_Click(object sender, EventArgs e)
        {
            CheckRowForDuplicates();
            CheckColumnsForDuplicateColumns();
            CheckForInvalidInput();
            CheckBlockForDuplicates();
        }

        private void HintButton_Click(object sender, EventArgs e)
        {
            GetHint();
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            var textBox = (TextBox) sender;
            m_currentCell = textBox;
            resetActive();
            if (!textBox.ReadOnly && !m_paused)
            {
                textBox.BackColor = SystemColors.GradientActiveCaption;
            }
            PauseButton.Focus();
        }

        private void SetCellValue(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 49 && e.KeyChar <= 57 && m_currentCell != null & m_currentCell.ReadOnly != true)
            {
                m_currentCell.Text = e.KeyChar.ToString();
                UpdateProgress();
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            m_puzzle.Time++;
            TimerLabel.Text = String.Format("{0:00}:{1:00}:{2:00}", m_puzzle.Hours, m_puzzle.Minutes, m_puzzle.Seconds);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveProgress();
        }
        #endregion

        #region UtilityFunctions
        //Clears the game board before loading a new game so that values aren't left over
        private void ClearGameBoard()
        {
            foreach (TextBox control in GameBoard.Controls)
            {
                control.Font = new Font(control.Font, FontStyle.Regular);
                control.Text = "";
                control.ReadOnly = false;
            }
        }

        //Sorts the textBoxes and assigns indexes so that they can be used in foreach loops throughout the app
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

        //one place to enable all controls when loading or unpausing a game
        private void EnableControls()
        {
            HardButton.Enabled = true;
            MediumButton.Enabled = true;
            EasyButton.Enabled = true;
            SaveButton.Enabled = true;
            HintButton.Enabled = true;
            ProgressButton.Enabled = true;
            PauseButton.Enabled = true;
            ClearButton.Enabled = true;
        }

        //one place to disable all controls when pausing a game
        private void DisableControls()
        {
            HardButton.Enabled = false;
            MediumButton.Enabled = false;
            EasyButton.Enabled = false;
            HintButton.Enabled = false;
            ProgressButton.Enabled = false;
            ClearButton.Enabled = false;
            SaveButton.Enabled = false;
        }

        //changes active cell background back to white when selecting a new cell
        private void resetActive()
        {
            foreach (TextBox control in GameBoard.Controls)
            {
                control.BackColor = Color.White;
            }
        }

        //updates the session progress string for comparison in the track progress function
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
            TrackSuccess();
        }
        #endregion

        #region CheckProgressFunctionality
        //Checks if any of the cells has in ancorrect value and highlights any that do
        private void CheckForInvalidInput()
        {
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
        }

        //Checks if any rows have a duplicate entry and highlights the entire row if it does
        private void CheckRowForDuplicates()
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
                        }
                    }
                }
            }
        }

        //Check if any columns have a duplicate entry and highlights the entire column if it does
        private void CheckColumnsForDuplicateColumns()
        {
            CheckRowForDuplicates();
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
                        }
                    }
                }
            }
        }

        //check if any blocks have a duplicate entry and highlights the entire block if it does
        private void CheckBlockForDuplicates()
        {
            var allCells = new List<TextBox>();
            foreach (TextBox textbox in GameBoard.Controls)
            {
                allCells.Add(textbox);
            }

            var blockCells = new List<TextBox>();
            for (int i = 1; i <= 9; i++)
            {
                blockCells = allCells.FindAll(x => x.Tag.ToString() == i.ToString());
                var cellValues = blockCells.SelectMany(x => x.Text);
                if (cellValues.Count() != cellValues.Distinct().Count())
                {
                    foreach (var control in blockCells)
                    {
                        control.BackColor = Color.Red;
                    }
                }
            }
        }
        #endregion

        #region LoadGameFunctionality
        //Checks if there are any games in progress
        //calls loadinprogressgame() if an in progress game exists
        //calls loadnewgame() if no in progress game exists
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

            if (string.IsNullOrEmpty(m_puzzle.Name))
            {
                MessageBox.Show("There are no unsovled " + difficulty + " puzzles remaining");
                return;
            }

            for (var i = 0; i < 81; i++)
            {
                if (m_puzzle.Start[i].ToString() != "0")
                {
                    GameBoard.Controls[i].Text = m_puzzle.Start[i].ToString();
                    GameBoard.Controls[i].Font = new Font(GameBoard.Controls[i].Font, FontStyle.Bold);
                    GameBoard.Controls[i].ForeColor = SystemColors.WindowText;
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
            HintButton.Enabled = true;
        }

        //loads in progress game if it exists
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
            m_puzzle.Cheated = puzzleFields[4] == "True";
        }

        //loads first game that isn't already solved
        private void LoadNewGame(string difficulty)
        {
            var newDirectoryPath = "./New/" + difficulty + "/";
            var solutionDirectoryPath = "./Solutions/";

            DirectoryInfo directoryInfo = new DirectoryInfo(newDirectoryPath);
            var files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                if (!Solved(file.Name))
                {
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
        }

        //checks if there are any in progress games in the given difficulty
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

        //checks if a given puzzle is already solved
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
        #endregion

        #region TrackSuccessFunctionality
        //checks whether the puzzle is completed after each entry or hint
        //saves to solved puzzles if it is complete
        //save to record file if the user didn't cheat
        //displays statistics
        private void TrackSuccess()
        {
            if (m_sessionProgress == m_puzzle.Solution)
            {
                GameTimer.Stop();
                var folderPath = "./Record/";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var recordPath = "./Record/" + m_puzzle.Difficulty;
                if (!File.Exists(recordPath))
                {
                    File.Create(recordPath).Dispose();
                }

                if (!m_puzzle.Cheated)
                {
                    File.AppendAllText(recordPath, m_puzzle.Time + Environment.NewLine);
                }

                Directory.CreateDirectory("./Solved/");
                var saveFileName = "./Solved/" + m_puzzle.Name;
                File.WriteAllText(saveFileName, m_puzzle.ToString());

                if (File.Exists("./InProgress/" + m_puzzle.Difficulty + "/" + m_puzzle.Name))
                {
                    File.Delete("./InProgress/" + m_puzzle.Difficulty + "/" + m_puzzle.Name);
                }

                var completeTimes = File.ReadAllLines(recordPath);
                int fst = 0;
                int avg = 0;
                if (completeTimes.Any())
                {
                    fst = Int32.Parse(completeTimes[0]);
                    int sum = 0;
                    foreach (var lines in completeTimes)
                    {
                        int time = Int32.Parse(lines);
                        sum += time;
                        if (time < fst)
                        {
                            fst = time;
                        }
                    }
                    avg = sum / completeTimes.Length;
                }
                MessageBox.Show("Congratulations!\nYour completion time: "
                    + String.Format("{0:00}:{1:00}:{2:00}", m_puzzle.Hours, m_puzzle.Minutes, m_puzzle.Seconds)
                    + "\nFastest completion time: "
                    + String.Format("{0:00}:{1:00}:{2:00}", fst / 3600, (fst % 3600) / 60, fst % 60)
                    + "\nAverage completion time: "
                    + String.Format("{0:00}:{1:00}:{2:00}", avg / 3600, (avg % 3600) / 60, avg % 60));

                m_puzzle.Clear();
                m_sessionProgress = string.Empty;
            }
        }
        #endregion

        #region SaveGameFunctionality
        //saves the game in its current state to be loaded later
        private void SaveProgress()
        {
            Directory.CreateDirectory("./Inprogress/" + m_puzzle.Difficulty);
            var saveFileName = "./InProgress/" + m_puzzle.Difficulty + "/" + m_puzzle.Name;
            m_puzzle.Progress = m_sessionProgress;
            File.WriteAllText(saveFileName, m_puzzle.ToString());
            MessageBox.Show("Your puzzle progress was saved successfully!");
        }
        #endregion

        #region HintFunctionality
        //provides a value to a random empty cell
        private void GetHint()
        {
            m_puzzle.Cheated = true;
            bool filled = true;
            for (var i = 0; i < 81; i++)
            {
                if (GameBoard.Controls[i].Text == "")
                {
                    filled = false;
                }
            }

            if (!filled)
            {
                Random rnd = new Random();
                var allCells = new List<TextBox>();
                foreach (TextBox control in GameBoard.Controls)
                {
                    allCells.Add(control);
                }

                var emptyCells = allCells.FindAll(x => string.IsNullOrEmpty(x.Text));
                int cell = rnd.Next(0, emptyCells.Count - 1);
                var index = GameBoard.Controls.IndexOf(emptyCells[cell]);
                emptyCells[cell].Text = m_puzzle.Solution[index].ToString();
            }
            else
            {
                for (var i = 0; i < 81; i++)
                {
                    TextBox txt = (TextBox)GameBoard.Controls[i];
                    if (!txt.ReadOnly && txt.Text != m_puzzle.Solution[i].ToString())
                    {
                        txt.Text = m_puzzle.Solution[i].ToString();
                    }
                }
            }
            UpdateProgress();
        }
        #endregion

        #region ClearProgressFunctionality
        //clears the game to its original state (including the timer and cheated flag)
        private void ClearProgress()
        {
            m_puzzle.Cheated = false;
            foreach (TextBox txt in GameBoard.Controls)
            {
                if (!txt.ReadOnly)
                {
                    txt.Text = "";
                }
            }

            m_puzzle.Time = 0;
        }
        #endregion

        #region PauseGameFunctionality
        //pauses the game, stops the timer, inactivates all of the controls but the pause button
        //changes text on pause button to resume (and back from resume to pause)
        private void PauseGame(Button pauseButton)
        {
            if (pauseButton.Text == "Pause")
            {
                DisableControls();
                m_paused = true;
                GameTimer.Stop();
                pauseButton.Text = "Resume";

                foreach (TextBox txt in GameBoard.Controls)
                {
                    if (txt.Font.Bold)
                    {
                        txt.ReadOnly = false;
                    }
                    txt.ForeColor = Color.Transparent;
                }
            }
            else if (pauseButton.Text == "Resume")
            {
                EnableControls();
                m_paused = false;
                GameTimer.Start();
                pauseButton.Text = "Pause";

                foreach (TextBox txt in GameBoard.Controls)
                {
                    if (txt.Font.Bold)
                    {
                        txt.ForeColor = Color.Black;
                        txt.ReadOnly = true;
                    }
                    else
                    {
                        txt.ForeColor = SystemColors.WindowFrame;
                    }
                }
            }
        }
        #endregion
    }
}
