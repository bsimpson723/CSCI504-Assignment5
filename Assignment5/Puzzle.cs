namespace Assignment5
{
    public class Puzzle
    {
        public string Name { get; set; }
        public string Difficulty { get; set; }
        public string Start { get; set; }
        public string Progress { get; set; }
        public string Solution { get; set; }
        public int Time { get; set; }
        public bool Cheated { get; set; }
        public int Hours => Time / 3600;
        public int Minutes => (Time % 3600) / 60;
        public int Seconds => Time % 60;


        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", Start, Progress, Solution, Time, Cheated);
        }

        public void Clear()
        {
            Name = string.Empty;
            Difficulty = string.Empty;
            Start = string.Empty;
            Progress = string.Empty;
            Solution = string.Empty;
            Time = 0;
        }
    }
}
