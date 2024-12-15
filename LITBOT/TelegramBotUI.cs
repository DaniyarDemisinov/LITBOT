using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace LITBOT
{
    public class TelegramBotUI: ITelegramBotUI
    {
        private ITelegramBotClient _botClient;
        private ReceiverOptions _receiverOptions;
        private string _mytoken;
        public TelegramBotUI()
        {
            _mytoken = "7870330040:AAHq0JoR2nyXDmc8oVGMonOisnv2ddv5M_M";
            _botClient = new TelegramBotClient(_mytoken);
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                },
                DropPendingUpdates = true
            };
        }

        

        public async Task Start()
        {
            using var cts = new CancellationTokenSource();
            _botClient.StartReceiving(UpdateHanlder, ErrorHandler, _receiverOptions, cts.Token);
            var me = await _botClient.GetMe();
            Console.WriteLine($"Бот {me.FirstName} запущен");
        }

        public async Task UpdateHanlder(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var message = update.Message;
                            var user = message.From;
                            // Здесь бы создать БД для библиотеки пользователя...
                            Console.WriteLine($"{user.FirstName} ({user.Id}) прислал вам сообщение: {message.Text}");
                            var chat = message.Chat;

                            switch (message.Type)
                            {
                                case MessageType.Text:
                                    {
                                        if(message.Text == "/start") 
                                        {
                                            await botClient.SendMessage(
                                                chat.Id,
                                                $"Привет, {user.FirstName}! " +
                                                $"Меня зовут LITBOT. Весь смысл моего существования состоит в том, чтобы читатели не теряли свои книги. " +
                                                $"Ты можешь использовать меня как хранилище своих любимых книг. Я могу загружать и выгружать их, " +
                                                $"а также могу показать твою библиотеку. Приятного пользования!");

                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                new List<InlineKeyboardButton[]>()
                                                {
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Поместить книгу в библиотеку", "upload")
                                                    },
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Скачать книгу из библиотеки")
                                                    },
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Посмотреть на библиотеку", "library")
                                                    }
                                                });
                                            await botClient.SendMessage(
                                                chat.Id,
                                                "Укажи, что тебе нужно: ",
                                                replyMarkup: inlineKeyboard);
                                        }
                                        break;
                                    }
                            }

                            break;  //
                        }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
        public Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
