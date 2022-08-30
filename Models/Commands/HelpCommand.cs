using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRegata.Models.Commands
{
    public class HelpCommand : Command
    {
        public override string Name => @"/help";

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
            await botClient.SendTextMessageAsync(chatId, "Мои команды:\n/start \n/help", 
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, 
                                                 replyMarkup: replyKeyboardMarkup);
        }
    }
}