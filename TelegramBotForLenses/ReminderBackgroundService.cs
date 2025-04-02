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
        var reminder = _db.Reminders.First();
        var nextChangeDate = reminder.Date;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTime.Now >= DateTime.SpecifyKind(nextChangeDate, DateTimeKind.Local))
            {
                reminder = _db.Reminders.First();
                nextChangeDate = reminder.Date;
                await _telegramBotClient.SendTextMessageAsync(
                    chatId: reminder.Id,
                    $"Прошло две недели!! пора менять линзы",
                    cancellationToken: stoppingToken);
            }
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
    
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly FileDbContext _db;
}