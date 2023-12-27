using Lab_5.Moduls;
using Microsoft.EntityFrameworkCore;

namespace Lab_5
{
    public class AppContext : DbContext
    {
        public DbSet<Song> Songs { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=songs.db");
        }
    }
}