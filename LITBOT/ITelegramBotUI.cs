using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LITBOT
{
    public interface ITelegramBotUI
    {
        Task Start();
        Task UpdateHanlder(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken);
    }
}
