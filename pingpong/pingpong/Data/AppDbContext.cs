using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using pingpong.Models;

namespace pingpong.Data;


public class AppDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<ScoreHistory> ScoreHistories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasOne(g => g.PlayerA)
            .WithMany()
            .HasForeignKey(g => g.PlayerAId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.PlayerB)
            .WithMany()
            .HasForeignKey(g => g.PlayerBId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ScoreHistory>()
            .HasOne(sh => sh.Player)
            .WithMany()
            .HasForeignKey(sh => sh.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(modelBuilder);
    }
    public void CleanUpScoreHistory(int maxRecords)
    {
        // Get the total number of records
        var count = ScoreHistories.Count();
        
        // If it exceeds maxRecords, remove the oldest entries
        if (count > maxRecords)
        {
            var recordsToDelete = ScoreHistories.OrderBy(s => s.ChangeDate)
                .Take(count - maxRecords);

            ScoreHistories.RemoveRange(recordsToDelete);
            SaveChanges(); // Commit the changes to the database
        }
    }

}

