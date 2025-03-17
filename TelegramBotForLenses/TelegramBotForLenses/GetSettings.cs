using Microsoft.Extensions.Configuration;

namespace TelegramBotForLenses;

public static class GetSettings
{
    private static readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    public static string Token => _config["BotSettings:Token"];
}