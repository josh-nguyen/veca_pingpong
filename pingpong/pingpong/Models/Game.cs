using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pingpong.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        public int PlayerAId { get; set; }
        [ForeignKey("PlayerAId")]
        public Player PlayerA { get; set; }

        public int PlayerBId { get; set; }
        [ForeignKey("PlayerBId")]
        public Player PlayerB { get; set; }
        public int RoundNumber { get; set; } 

        public int? ScoreA { get; set; }
        public int? ScoreB { get; set; }
    }
}