using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", Name, Difficulty, Start, Progress, Solution, Time);
        }
    }
}
