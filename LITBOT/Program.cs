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
        public static Genre genre = new Genre();
        public static Author author = new Author();
        public static Book book = new Book();
        public static ReplyKeyboardMarkup replyKeyboard;

        static async Task Main(string[] args)
        {
            using var cts = new CancellationTokenSource();
            bot = new TelegramBotClient("7870330040:AAHq0JoR2nyXDmc8oVGMonOisnv2ddv5M_M", cancellationToken: cts.Token);
            var me = await bot.GetMe();
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;
            replyKeyboard = new ReplyKeyboardMarkup(true).AddButtons("/start");


            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
            Console.ReadLine();


            cts.Cancel();
        }
        static async Task OnMessage(Message msg, UpdateType type)
        {

            Console.WriteLine($"{msg.From} прислал вам личное сообщение: {msg.Text}");
            if (msg.Text == "/start")
            {
                var inlineKeyboardStart = new InlineKeyboardMarkup(
                    new List<InlineKeyboardButton[]>()
                    {
                       new InlineKeyboardButton[]
                       {
                           InlineKeyboardButton.WithCallbackData("Загрузить книгу в библиотеку", "put")
                       },
                       new InlineKeyboardButton[]
                       {
                           InlineKeyboardButton.WithCallbackData("Скачать книгу из библиотеки", "take")
                       },
                       new InlineKeyboardButton[]
                       {
                           InlineKeyboardButton.WithCallbackData("Посмотреть свою библиотеку", "watch")
                       }
                    });
                await bot.SendMessage(msg.Chat, $"Здравствуйте, {msg.From.FirstName}! Моё имя - LITBOT. ", replyMarkup: replyKeyboard);
                await bot.SendMessage(
                    msg.Chat,
                    "Я умею загружать книги на сервер, выгружать их, " +
                    "а также могу показать Вашу библиотеку. Выберите, что мне нужно сделать:",
                    replyMarkup: inlineKeyboardStart);
            }
            else if (chatStatus != null)
            {
                switch (chatStatus)
                {
                    case "genre":
                        {
                            infoAboutBook.Add(msg.Text);//
                            genre.name = msg.Text;
                            chatStatus = "author";
                            await bot.SendMessage(
                                msg.Chat,
                                "Введите ФИО автора (например: А.С.Пушкин):");
                            break;
                        }
                    case "author":
                        {
                            infoAboutBook.Add(msg.Text);//
                            author.name = msg.Text;
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
                            string path = @"C:\Users\demis\Desktop\Study\OTUS\ProjectWorkLitBot\";
                            string subpath = $@"{msg.From.Id}\{genre.name}\{author.name}\{book.name}\";
                            DirectoryInfo dirInfo = new DirectoryInfo(path);
                            if (!dirInfo.Exists)
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
                            var fileExtension = Path.GetExtension(filePath);
                            string destinationFilePath = path + subpath + $@"{book.name}.jpg";
                            await using Stream fileStream = System.IO.File.Create(destinationFilePath);
                            await bot.DownloadFile(filePath, fileStream);
                            await bot.SendMessage(msg.Chat, "Обложка успешна загружена");
                            chatStatus = "book";
                            book.picturePath = destinationFilePath;
                            await bot.SendMessage(msg.Chat, "Отправьте книгу");
                            break;
                        }
                    case "book":
                        {
                            string path = @"C:\Users\demis\Desktop\Study\OTUS\ProjectWorkLitBot\";
                            string subpath = $@"{msg.From.Id}\{genre.name}\{author.name}\{book.name}\";
                            DirectoryInfo dirInfo = new DirectoryInfo(path);
                            if (!dirInfo.Exists)
                            {
                                dirInfo.Create();
                            }
                            dirInfo.CreateSubdirectory(subpath);

                            if (msg.Type is not MessageType.Document)
                            {
                                await bot.SendMessage(
                                    msg.Chat,
                                    "Ошибка. Пожалуйста, отправьте файл в виде документа");
                                return;
                            }

                            var fileId = msg.Document.FileId;

                            var fileInfo = await bot.GetFile(fileId);
                            var filePath = fileInfo.FilePath;
                            var fileExtension = Path.GetExtension(filePath);
                            var destinationFilePath = path + subpath + $@"{book.name}{fileExtension}";
                            await using Stream fileStream = System.IO.File.Create(destinationFilePath);
                            await bot.DownloadFile(filePath, fileStream);


                            book.bookPath = destinationFilePath;
                            DapperBookRepository dapperBookRepository = new DapperBookRepository();
                            await dapperBookRepository.InsertToGenre(genre);
                            int genreId = await dapperBookRepository.GetId("genre", "name", genre.name);
                            Console.WriteLine($"genreId = {genreId}");
                            await dapperBookRepository.InsertToAuthor(author, genreId);
                            int authorId = await dapperBookRepository.GetId("author", "name", author.name);
                            Console.WriteLine($"authorId = {authorId}");
                            await dapperBookRepository.InsertToBook(book, genreId, authorId);
                            await bot.SendMessage(msg.Chat, "Книга успешно загружена");
                            break;
                        }
                    case "upload":
                        {
                            string nameBook = msg.Text;
                            var dapperBookRepository = new DapperBookRepository();
                            var books = await dapperBookRepository.GetBooks(nameBook);
                            foreach (var book in books)
                            {
                                string genreName = await dapperBookRepository.GetName("genre", book.genreId);
                                string authorName = await dapperBookRepository.GetName("author", book.authorId);

                                await using Stream streamPhoto = System.IO.File.OpenRead(book.picturePath);
                                int lastIndexOfPhoto = book.picturePath.LastIndexOf('\\');
                                string namePhotoWithExp = book.picturePath.Substring(lastIndexOfPhoto + 1);
                                var messagePhoto = await bot.SendPhoto(msg.Chat, photo: InputFile.FromStream(streamPhoto, namePhotoWithExp),
                                    caption: $"Жанр: {genreName}\nАвтор: {authorName}\nКнига: {book.name}👇");
                                await using Stream streamDocument = System.IO.File.OpenRead(book.bookPath);
                                int lastIndexOfDocument = book.bookPath.LastIndexOf('\\');
                                string nameBookWithExp = book.bookPath.Substring(lastIndexOfDocument + 1);
                                var messageDocument = await bot.SendDocument(msg.Chat, document: InputFile.FromStream(streamDocument, nameBookWithExp));
                            }
                            break;
                        }
                }
            }
            // По идее, надо удалить это:
            else if (msg.Type == MessageType.Photo)
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
            switch (callbackQuery!.Data)
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
                            callbackQuery.Message.Chat,
                            "Хорошо. Теперь нужно приложить файл книги:");
                        chatStatus = "book";
                        break;
                    }
                case "take":
                    {
                        await bot.SendMessage(
                            callbackQuery.Message.Chat,
                            "Напишите название книги:");
                        chatStatus = "upload";


                        break;
                    }
                default:
                    {
                        await bot.SendMessage(
                            callbackQuery.Message.Chat,
                            "Что-то пошло не так...");
                        break;
                    }
            }
        }
        // добавить методы по обработке ошибок
    }
}
