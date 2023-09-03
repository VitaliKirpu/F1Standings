using System.ComponentModel.DataAnnotations;

namespace ConsoleApplication1
{
    public class Racer
    {
        [Key]
        public string RacerId  { get; set; }
        
        public string Name { get; set; }
        public string Team { get; set; }
        public int Wins { get; set; }
        public int Points { get; set; }
    }
}