using Microsoft.Extensions.Configuration;

namespace TelegramBotForLenses;

public static class GetSettings
{
    private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    public static string Token => Config["BotSettings:Token"]!;
}