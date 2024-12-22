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
        public static InlineKeyboardMarkup inlineKeyboardPicture;
        public static Book book = new Book();
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
                var inlineKeyboardStart = new InlineKeyboardMarkup(
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
                    replyMarkup: inlineKeyboardStart);
            }
            else if (chatStatus != null)
            {
                switch (chatStatus)
                {
                    case "genre":
                        {
                            infoAboutBook.Add(msg.Text);//
                            book.genre = msg.Text;
                            chatStatus = "author";
                            await bot.SendMessage(
                                msg.Chat,
                                "Введите ФИО автора в формате И.О. Фамилия (например: А.С. Пушкин):");
                            break;
                        }
                    case "author":
                        {
                            infoAboutBook.Add(msg.Text);//
                            book.author = msg.Text;
                            chatStatus = "nameBook";
                            await bot.SendMessage(
                                msg.Chat,
                                "Введите название книги:");
                            break;
                        }
                    case "nameBook":
                        {
                            infoAboutBook.Add(msg.Text);//
                            book.name = msg.Text;
                            chatStatus = "picture";
                            inlineKeyboardPicture = new InlineKeyboardMarkup(
                                new List<InlineKeyboardButton[]>
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Желаю", "wish"),
                                        InlineKeyboardButton.WithCallbackData("Не желаю", "no wish")
                                    }
                                });
                            await bot.SendMessage(
                                msg.Chat,
                                "Если желаете, можете отправить обложку книги в виде картинки:",
                                replyMarkup: inlineKeyboardPicture);
                            break;
                        }
                    case "picture":
                        {
                            await bot.SendMessage(msg.Chat, "Мы находимся в case 'picture((('");  // Почему не появляется?
                            string path = @"C:\Users\demis\Desktop\Study\OTUS\ProjectWorkLitBot\";
                            string subpath = $@"{msg.From.Id}\{book.genre}\{book.author}\{book.name}\";
                            DirectoryInfo dirInfo = new DirectoryInfo(path);
                            if (dirInfo.Exists)
                            {
                                dirInfo.Create();
                            }
                            dirInfo.CreateSubdirectory(subpath);

                            string fileId;
                            if (msg.Type is MessageType.Document)
                            {
                                fileId = msg.Document.FileId;
                            }
                            else
                            {
                                fileId = msg.Photo.Last().FileId;
                            }
                            var fileInfo = await bot.GetFile(fileId);
                            var filePath = fileInfo.FilePath;
                            string destinationFilePath = path + subpath + @"обложка.jpg";
                            ;
                            await using Stream fileStream = System.IO.File.Create(destinationFilePath);
                            await bot.DownloadFile(filePath, fileStream);
                            break;
                        }
                }
            }
            else if(msg.Type == MessageType.Photo)
            {
                await bot.SendMessage(
                    msg.Chat,
                    "Отлично! Я загрузил картинку!");
                // Здес или в case "picture" должна загрузиться картинка на комп
            }
            else
            {
                await bot.SendMessage(
                    msg.Chat,
                    "Для начала работы нажмите на \"/start\"");
            }
            

            //Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");
            //await bot.SendMessage(msg.Chat, $"{msg.From} said: {msg.Text}");
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
                            "Вы выбрали \"Загрузить книгу на сервер\"\n");
                        await bot.SendMessage(
                            callbackQuery.Message.Chat,
                            "Введите жанр книги");
                        chatStatus = "genre";
                        break;

                    }
                case "wish":
                    {
                        await bot.AnswerCallbackQuery(callbackQuery.Id);
                        await bot.SendMessage(
                            callbackQuery.Message.Chat,
                            "Вы выбрали \"Желаю\"\n");
                        await bot.SendMessage(
                            callbackQuery.Message.Chat,
                            "Приложите, пожалуйста, картинку:");
                        break;
                    }
                case "no wish":
                    {
                        await bot.SendMessage(
                            update.Message.Chat,
                            "Хорошо. Теперь нужно приложить файл книги:");
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
