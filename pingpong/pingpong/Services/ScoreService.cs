using pingpong.Data;
using pingpong.Models;

public class ScoreService
{
    private readonly AppDbContext _context;

    public ScoreService(AppDbContext context)
    {
        _context = context;
    }

    public void UpdateScore(int playerId, bool increment)
    {
        var player = _context.Players.Find(playerId);
        if (player != null)
        {
            // Record the old score
            int oldScore = player.Score;

            // Update the score
            player.Score += increment ? 1 : -1;

            // Record the new score
            int newScore = player.Score;

            // Log the history
            var scoreHistory = new ScoreHistory
            {
                PlayerId = playerId,
                OldScore = oldScore,
                NewScore = newScore,
                ChangeDate = DateTime.UtcNow
            };

            _context.ScoreHistories.Add(scoreHistory);
            _context.SaveChanges();
        }
    }
}