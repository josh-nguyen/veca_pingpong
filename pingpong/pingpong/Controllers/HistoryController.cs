using Microsoft.AspNetCore.Mvc;
using pingpong.Data;
using pingpong.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class HistoryController : Controller
{
    private readonly AppDbContext _context;

    public HistoryController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var history = _context.ScoreHistories
            .Include(sh => sh.Player)
            .OrderByDescending(sh => sh.ChangeDate)
            .ToList();

        return View(history);
    }
}