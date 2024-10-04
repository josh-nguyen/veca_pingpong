using Microsoft.AspNetCore.Mvc;
using pingpong.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pingpong.Models; // Make sure to include your Player model

namespace pingpong.Controllers
{
    public class GamesController : Controller
    {
        private readonly AppDbContext _context;

        public GamesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Display the main page
            return View();
        }

        public async Task<IActionResult> SelectPlayers()
        {
            // Retrieve all players from the database
            var players = await _context.Players.OrderByDescending(p => p.Score).ToListAsync();
            return View(players); // Pass players to the view
        }

        [HttpPost]
        public async Task<IActionResult> GenerateGames(List<int> selectedPlayers)
        {
            if (selectedPlayers == null || !selectedPlayers.Any())
            {
                // Handle the case where no players were selected
                return RedirectToAction("SelectPlayers"); // Redirect back to the selection page
            }

            // Retrieve the selected players from the database using the IDs
            var players = await _context.Players
                .Where(p => selectedPlayers.Contains(p.Id))
                .OrderByDescending(p => p.Score)
                .ToListAsync();


            // Generate the games using round-robin logic
            var matchups = GenerateRoundRobin(players);

            // Pass the generated matchups to the view
            return View("Generated", matchups);
        }

        private List<List<(Player PlayerA, Player PlayerB)>> GenerateRoundRobin(List<Player> players)
        {
            var rounds = new List<List<(Player, Player)>>();
            var totalPlayers = players.Count;

            // Generate the rounds
            for (int round = 0; round < totalPlayers - 1; round++)
            {
                var roundMatchups = new List<(Player, Player)>();
                for (int i = 0; i < totalPlayers / 2; i++)
                {
                    var playerA = players[i];
                    var playerB = players[totalPlayers - 1 - i];
                    roundMatchups.Add((playerA, playerB));
                }

                rounds.Add(roundMatchups);

                // Rotate players for the next round
                players.Insert(1, players.Last());
                players.RemoveAt(players.Count - 1);
            }

            return rounds;
        }
    }
}
