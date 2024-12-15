namespace LITBOT
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TelegramBotUI telegram = new TelegramBotUI();
            telegram.Start();
            Console.ReadLine();
        }
    }
}
