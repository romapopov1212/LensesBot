using System.Globalization;
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
                if (DateTime.TryParseExact(messageText,
                        "dd.MM.yy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var lastChangeData))
                {
                    _userStates.Remove(chatId);
                    await bot.SendTextMessageAsync(chatId,
                        $"Запомнил! Ты последний раз меняла линзы {lastChangeData:d}. Буду напоминать!",
                        cancellationToken: token);
                    await using var db = new FileDbContext();
                    db.Add(new Reminder()
                    {
                        Date = lastChangeData,
                    });
                    await db.SaveChangesAsync(cancellationToken: token);
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