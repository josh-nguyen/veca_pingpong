using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using pingpong.Models;

namespace pingpong.Data;


public class AppDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
        
    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    // {
    //     options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
    // }



    public DbSet<Player> Players { get; set; }


}

