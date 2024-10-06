using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pingpong.Data;
using pingpong.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pingpong.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly AppDbContext _context;

        public LeaderboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch all players with their corresponding score history
            var playersWithScores = await _context.Players
                .Select(player => new
                {
                    Player = player,
                    IncrementCount = _context.ScoreHistories.Count(sh => sh.PlayerId == player.Id && sh.NewScore > sh.OldScore),
                    TotalCount = _context.ScoreHistories.Count(sh => sh.PlayerId == player.Id)
                })
                .ToListAsync();

            // Calculate the form score for each player
            foreach (var item in playersWithScores)
            {
                double playerFormScore = item.TotalCount > 0
                    ? (item.IncrementCount / (double)item.TotalCount) * 10
                    : 0;

                // Ensure the form score is clamped between 0 and 10
                playerFormScore = Math.Clamp(playerFormScore, 0, 10);
                playerFormScore = Math.Round(playerFormScore, 2);
                item.Player.Form = playerFormScore; // Assign the calculated form score back to the player
            }

            // Order players by Score in descending order
            var orderedPlayers = playersWithScores
                .Select(x => x.Player) // Extract the Player object back
                .OrderByDescending(p => p.Score)
                .ToList();

            return View(orderedPlayers);
        }

    }
}