using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pingpong.Data;
using pingpong.Models;
using System.Collections.Generic;
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
            // Query the database and order players by Score in descending order
            var players = await _context.Players.OrderByDescending(p => p.Score).ToListAsync();
            return View(players); // Return the view with the ordered player list
        }
    }
}