using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

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
            var chatId = message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, "Мои команды:\n Пока только \n /start \n /help", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}