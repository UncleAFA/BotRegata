using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static BotRegata.Models.AppSettings;

namespace BotRegata.Models.Commands
{
    public class AddingLine
    {
        public async Task Execute(Message message, TelegramBotClient botClient)
        {

            switch (StateList[message.Chat.Id])
            {
                case State.None:
                    break;
                case State.Fio:
                    break;
                case State.Points:
                    await AddPoints(message, botClient);
                    break;
                case State.Details:
                    await AddDetails(message, botClient);
                    break;
            }
        }
        public async Task AddDetails(Message message, TelegramBotClient botClient)
        {
            if (message.Text != null)
            {
                    var Persone = InsertList[message.Chat.Id];
                    Persone.Details = message.Text;
                    InsertList[message.Chat.Id] = Persone;
                    StateList[message.Chat.Id] = State.Confirm;
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Входные данные: \n{InsertList[message.Chat.Id].Fio} \n{InsertList[message.Chat.Id].Points} \n{InsertList[message.Chat.Id].Details}",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );

                var keyboard = new InlineKeyboardMarkup
                (
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Да","Да"),
                        InlineKeyboardButton.WithCallbackData("Нет","Нет")
                    }
                );
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Все верно?",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                                                 replyMarkup: keyboard
                                                 );
                    return;
            }
        }
        public async Task AddPoints(Message message, TelegramBotClient botClient)
        {
            string MessText = message.Text;
            if (MessText[0] == '+' || MessText[0] == '-')
            {
                if (float.TryParse(MessText, out float res))
                {
                    var Persone = InsertList[message.Chat.Id];
                    Persone.Points = message.Text;
                    InsertList[message.Chat.Id] = Persone;
                    StateList[message.Chat.Id] = State.Details;
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Входные данные: \n{InsertList[message.Chat.Id].Fio} \n{InsertList[message.Chat.Id].Points} ",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Теперь введите детали:",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
                    return;
                }
            }

            await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка! \n Введите количество баллов(пример:+5,6 или -5,0)",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
        }
    }
}
