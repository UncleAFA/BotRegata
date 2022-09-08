using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static BotRegata.Models.AppSettings;

namespace BotRegata.Models.Commands
{
    public class CencelCommand : Command
    {
        public override string Name => @"Отмена";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            
            var chat = message.Chat.Id;
            try
            {
                StateList[message.Chat.Id] = State.None;
            }
            catch (Exception)
            {
                StateList[message.Chat.Id] = State.None;
            }
            await botClient.SendTextMessageAsync(chatId, "Все действия отменены",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
        }
    }
}