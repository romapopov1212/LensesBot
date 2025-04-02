using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace TelegramBotForLenses
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var token = GetSettings.Token;
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
                    services.AddDbContext<FileDbContext>();
                    services.AddHostedService<ReminderBackgroundService>();
                });
            using var host = hostBuilder.Build();
            await host.StartAsync();
            HostBot mainBot = new HostBot(token);
            mainBot.Start();

            Console.WriteLine("Бот и напоминания запущены. Нажмите Enter для выхода...");
            Console.ReadLine();
            
            await host.StopAsync();
        }
    }
}