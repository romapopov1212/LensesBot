namespace TelegramBotForLenses
{
    class Program
    {
        private static void Main(string[] args)
        {
            var token = GetSettings.Token;
            Host newBot = new Host(token);
            newBot.Start();
            Console.ReadLine();
        }
    }
}