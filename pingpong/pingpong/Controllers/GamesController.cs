using Microsoft.AspNetCore.Mvc;
using pingpong.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pingpong.Models;

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
            // Retrieve all games from the database
            var games = await _context.Games
                .Include(g => g.PlayerA)
                .Include(g => g.PlayerB)
                .ToListAsync();

            // Pass the games to the view
            return View(games);
        }

        public async Task<IActionResult> SelectPlayers()
        {
            // Check if there are existing games in the database
            var existingGames = await _context.Games
                .Include(g => g.PlayerA)
                .Include(g => g.PlayerB)
                .ToListAsync();

            if (existingGames.Any())
            {
                // If there are existing games, delete them from the database
                _context.Games.RemoveRange(existingGames); // Remove existing games
                await _context.SaveChangesAsync(); // Save changes to the database
            }

            // Retrieve all players from the database
            var players = await _context.Players.OrderByDescending(p => p.Score).ToListAsync();
            return View(players); // Pass players to the view if no games exist
        }




        [HttpPost]
        public async Task<IActionResult> GenerateGames(List<int> selectedPlayers)
        {
            if (selectedPlayers == null || !selectedPlayers.Any())
            {
                // No players were selected
                return RedirectToAction("SelectPlayers");
            }

            // Retrieve the selected players from the database using the IDs and order by score
            var players = await _context.Players
                .Where(p => selectedPlayers.Contains(p.Id))
                .OrderByDescending(p => p.Score)
                .ToListAsync();

            // Generate the games
            var matchups = GenerateRoundRobin(players);

            // Save the generated games to the database, including round information
            int roundCounter = 1; // Initialize round counter
            foreach (var round in matchups)
            {
                foreach (var matchup in round)
                {
                    var game = new Game
                    {
                        PlayerAId = matchup.PlayerA.Id,
                        PlayerBId = matchup.PlayerB.Id,
                        RoundNumber = roundCounter // Set the round number for the game
                    };
                    _context.Games.Add(game);
                }

                roundCounter++; // Increment the round counter for the next round
            }

            await _context.SaveChangesAsync();

            // Redirect to the Index action to display the games
            return RedirectToAction("Index");
        }

        private List<List<(Player PlayerA, Player PlayerB)>> GenerateRoundRobin(List<Player> players)
        {
            var rounds = new List<List<(Player, Player)>>();
            var totalPlayers = players.Count;

            // Shuffle the players list
            Random rng = new Random();
            int n = players.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (players[n], players[k]) = (players[k], players[n]);
            }

            // Generate the rounds
            for (int round = 0; round < totalPlayers - 1; round++)
            {
                var roundMatchups = new List<(Player PlayerA, Player PlayerB)>();
        
                // Create matchups for the current round
                for (int i = 0; i < totalPlayers / 2; i++)
                {
                    var playerA = players[i];
                    var playerB = players[totalPlayers - 1 - i];
                    roundMatchups.Add((playerA, playerB));
                }

                // Sort matchups by combined scores
                roundMatchups = roundMatchups
                    .OrderBy(pair => pair.PlayerA.Score + pair.PlayerB.Score) 
                    .ToList();

                // Add the sorted matchups to rounds
                rounds.Add(roundMatchups); 

                // Rotate players for the next round
                players.Insert(1, players.Last());
                players.RemoveAt(players.Count - 1);
            }

            return rounds;
        }
    }
}
