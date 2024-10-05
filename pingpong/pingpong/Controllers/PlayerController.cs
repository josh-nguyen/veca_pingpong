using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pingpong.Data;
using pingpong.Models;

namespace pingpong.Controllers
{
    public class PlayerController : Controller
    {
        private readonly AppDbContext _context;

        public PlayerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Player
        public async Task<IActionResult> Index()
        {
            var players = await _context.Players.ToListAsync(); 
            return View(players);
        }

        // GET: Player/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Player/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Player/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Score")] Player player)
        {
            // Parameter checks
            if (player.Name.Length > 14)
            {
                ModelState.AddModelError("Name", "Name cannot exceed 14 characters.");
            }
            if (player.Score > 150)
            {
                ModelState.AddModelError("Score", "Score cannot exceed 150.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(player);
        }

        // GET: Player/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Player/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Score")] Player player)
        {
            if (id != player.Id)
            {
                return NotFound();
            }

            // Parameter checks
            if (player.Name.Length > 14)
            {
                ModelState.AddModelError("Name", "Name cannot exceed 14 characters.");
            }
            if (player.Score > 150)
            {
                ModelState.AddModelError("Score", "Score cannot exceed 150.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(player);
        }

        // GET: Player/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.Id == id);
        }

        // POST: Player/UpdateScore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateScore(int id, bool increment, bool champ = false)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            // Store the old score before updating
            var oldScore = player.Score;

            // Update the score based on whether we're incrementing, decrementing, or adding Champ points
            if (champ)
            {
                player.Score += 3; // Add 3 points for Champ
            }
            else if (increment)
            {
                player.Score++;
            }
            else
            {
                player.Score--;
            }

            // Validate score update
            if (player.Score > 150)
            {
                ModelState.AddModelError("Score", "Score cannot exceed 150.");
                // Optionally return to index or appropriate view
                return RedirectToAction(nameof(Index)); // Or render a specific view
            }

            // Add the score change to the ScoreHistory
            var scoreHistory = new ScoreHistory
            {
                Player = player, // Assuming you have a navigation property
                OldScore = oldScore,
                NewScore = player.Score,
                ChangeDate = DateTime.Now
            };

            // Add the score history entry to the context
            _context.ScoreHistories.Add(scoreHistory);

            // Save changes to the database
            _context.Update(player);
            await _context.SaveChangesAsync();

            // Clean up the ScoreHistory if necessary
            _context.CleanUpScoreHistory(30); 

            return RedirectToAction(nameof(Index));
        }
    }
}
