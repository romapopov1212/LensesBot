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
            if (messageText.StartsWith("/start"))
            {
                await bot.SendTextMessageAsync(update.Message.Chat.Id,
                    "Лерочка! Этот бот что бы напоминать тебе о линзах! Введи /reminder что бы создать напоминание",
                    cancellationToken: token);
            }
        } 
    }

    private Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
    
    
    private TelegramBotClient _bot;
    private FileDbContext _dbContext;
}