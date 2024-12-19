using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace LITBOT
{
    internal class Program
    {
        public static TelegramBotClient bot;
        public static string chatStatus;
        public static List<string> infoAboutBook = new List<string>();
        static async Task Main(string[] args)
        {
            using var cts = new CancellationTokenSource();
            bot = new TelegramBotClient("7870330040:AAHq0JoR2nyXDmc8oVGMonOisnv2ddv5M_M", cancellationToken: cts.Token);
            var me = await bot.GetMe();
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;

            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
            Console.ReadLine();

            foreach ( string s in infoAboutBook )
            {
                Console.WriteLine( s );
            }

            cts.Cancel();
        }
        static async Task OnMessage(Message msg, UpdateType type)
        {
            Console.WriteLine($"{msg.From} прислал вам личное сообщение: {msg.Text}");
            if(msg.Text == "/start")
            {
                var inlineKeyboard = new InlineKeyboardMarkup(
                    new List<InlineKeyboardButton[]>()
                    {
                       new InlineKeyboardButton[]
                       {
                           InlineKeyboardButton.WithCallbackData("Загрузить книгу на сервер", "put")
                       },
                       new InlineKeyboardButton[]
                       {
                           InlineKeyboardButton.WithCallbackData("Скачать книгу с сервера", "take")
                       },
                       new InlineKeyboardButton[]
                       {
                           InlineKeyboardButton.WithCallbackData("Посмотреть свою библиотеку", "watch")
                       }
                    });
                await bot.SendMessage(
                    msg.Chat,
                    $"Привет, {msg.From.FirstName}! Моё имя - LITBOT. Я умею загружать книги на сервер, выгружать их," +
                    $"а также могу показать Вашу библиотеку. Выберите, что мне нужно сделать:",
                    replyMarkup: inlineKeyboard);
            }
            else
            {
                switch (chatStatus)
                {
                    case "genre":
                        {
                            infoAboutBook.Add(msg.Text);
                            break;
                        }
                }
            }
            

            Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");
            await bot.SendMessage(msg.Chat, $"{msg.From} said: {msg.Text}");
        }
        static async Task OnUpdate(Update update)
        {
            var callbackQuery = update.CallbackQuery;
            switch (callbackQuery.Data)
            {
                case "put":
                    {
                        await bot.AnswerCallbackQuery(callbackQuery.Id);
                        await bot.SendMessage(
                            callbackQuery.Message.Chat,
                            "Вы выбрали \"Загрузить книгу на сервер\"\n" +
                            "Введите жанр книги");
                        chatStatus = "genre";
                        break;

                    }
                default:
                    {
                        Console.WriteLine("что-то пошло не так");
                        break;
                    }
            }
        }
        
    }
}
