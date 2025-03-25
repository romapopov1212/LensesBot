using Microsoft.EntityFrameworkCore;

namespace TelegramBotForLenses;

public class FileDbContext : DbContext
{
    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       optionsBuilder.UseSqlite("Data Source=/Users/user/Documents/GitHub/LensesBot/Reminders.db");
    }
}