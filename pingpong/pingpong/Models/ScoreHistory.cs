namespace pingpong.Models
{
    public class ScoreHistory
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int OldScore { get; set; }
        public int NewScore { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}