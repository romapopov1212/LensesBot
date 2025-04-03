using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace TelegramBotForLenses;
public class ReminderBackgroundService : BackgroundService
{
    public ReminderBackgroundService(ITelegramBotClient telegramBotClient, FileDbContext db)
    {
        _telegramBotClient = telegramBotClient;
        _db = db;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var db = new FileDbContext(); // Создаем новый контекст для каждой итерации
            var reminder = await db.Reminders.FirstOrDefaultAsync(stoppingToken);
        
            if (reminder != null)
            {
                var nextChangeDate = reminder.Date;
                Console.WriteLine($"{nextChangeDate}");
            
                if (DateTime.Now >= DateTime.SpecifyKind(nextChangeDate, DateTimeKind.Local))
                {
                    await _telegramBotClient.SendTextMessageAsync(
                        chatId: reminder.Id,
                        $"Прошло две недели!! пора менять линзы",
                        cancellationToken: stoppingToken);
                }
            }
        
            await Task.Delay(TimeSpan.FromMinutes(0,5), stoppingToken);
        }
    }
    
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly FileDbContext _db;
}