namespace pingpong.Services;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Microsoft.Extensions.Logging;

public class PlayerService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(AppDbContext context, ILogger<PlayerService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    
    public void AddPlayer(Player player)
    {
        try
        {
            _context.Players.Add(player); // Add the player to the context
            _context.SaveChanges(); // Save changes to the database
            _logger.LogInformation($"Player {player.Name} added successfully."); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding player to database."); 
            throw; // Optionally rethrow to handle higher up
        }
    }

    
    public async Task UpdateScoreAsync(int playerID, int newScore)
    {
        Player player = await _context.Players.FindAsync(playerID);
        if (player != null)
        {
            player.Score = newScore;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteUserAsync(int playerID)
    {
        var player = await _context.Players.FindAsync(playerID);
        if (player != null)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<Player> GetPlayerByIdAsync(int playerID)
    {
        return await _context.Players.FindAsync(playerID);
    }


    public async Task<List<Player>> GetLeaderBoardAsync()
    {
        return await _context.Players.OrderByDescending(p => p.Score).ToListAsync();
    }
}