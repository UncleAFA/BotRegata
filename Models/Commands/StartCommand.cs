using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRegata.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "One", "Two" },
                new KeyboardButton[] { "Three", "Four" },
            })
            {
                ResizeKeyboard = true
            };
            var chatId = message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, "Привет я бот для ведения Регаты", 
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                                                 replyMarkup: replyKeyboardMarkup);
        }
    }
}