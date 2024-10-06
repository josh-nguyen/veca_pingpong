using Microsoft.AspNetCore.Mvc;
using pingpong.Data;
using pingpong.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

public class HistoryController : Controller
{
    private readonly AppDbContext _context;

    public HistoryController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var ausEasternZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");

        // Fetch score histories with players included
        var history = _context.ScoreHistories
            .Include(sh => sh.Player)
            .AsNoTracking() // Improve performance by not tracking the entities
            .ToList() // Load data into memory
            .Select(sh => {
                sh.ChangeDate = TimeZoneInfo.ConvertTimeFromUtc(sh.ChangeDate, ausEasternZone);
                return sh;
            })
            .OrderByDescending(sh => sh.ChangeDate)
            .ToList();

        return View(history);
    }

}