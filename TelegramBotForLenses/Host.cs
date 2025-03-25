using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotForLenses;

public class Host
{
    public Host(string token)
    {
        _bot = new TelegramBotClient(token);
    }

    public void Start()
    {
        _bot.StartReceiving(UpdateHandler, ErrorHandler);
    }
    
    private async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update.Message?.Text is string messageText)
        {
            long chatId = update.Message.Chat.Id;
            if (messageText.StartsWith("/start"))
            {
                await bot.SendTextMessageAsync(chatId,
                    "Лерочка! Этот бот что бы напоминать тебе о линзах! Введи /reminder что бы создать напоминание",
                    cancellationToken: token);
            }
            else if (messageText.StartsWith("/reminder"))
            {
                _userStates[chatId] = "awaiting_date";
                await bot.SendTextMessageAsync(chatId, 
                    "Лерочка, напиши когда ты в последний раз меняла линзы(дд.мм.гг)",
                    cancellationToken: token);
            }
            else if (_userStates.TryGetValue(chatId, out var state) && state == "awaiting_date")
            {
                if ((DateTime.TryParseExact(messageText,
                        "dd.MM.yy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var lastChangeData)) && (lastChangeData <= DateTime.Now) && (lastChangeData <= DateTime.Now.AddDays(-14)))
                {
                    if (lastChangeData <= DateTime.Now.AddDays(-14))
                    {
                        await bot.SendTextMessageAsync(chatId, "Лерочка, уже прошло 2 недели, пора менять лизы!");
                    }
                    else
                    {
                        var nextDayToChangeLenses = lastChangeData.AddDays(14);
                        _userStates.Remove(chatId);
                        await bot.SendTextMessageAsync(chatId,
                            $"Запомнил! Напомню {nextDayToChangeLenses}",
                            cancellationToken: token);
                        await using var db = new FileDbContext();
                        db.Database.ExecuteSqlRaw("DELETE FROM Reminders");
                        db.Add(new Reminder()
                        {
                            Date = nextDayToChangeLenses,
                        });
                        await db.SaveChangesAsync(cancellationToken: token);
                    }
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId, 
                        $"Введи дату верно(день.месяц.год(2025->25), также дата не должна быть позже сегодняшней(сегодня {DateTime.Now:dd.MM.yy}))",
                        cancellationToken: token);
                }
            }
        }
    }

    private Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
    
    private Dictionary<long, string> _userStates = new Dictionary<long, string>();
    private readonly TelegramBotClient _bot;
}