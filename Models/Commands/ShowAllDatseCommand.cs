using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotRegata.Models.Commands
{
    public class ShowAllDatseCommand : Command
    {
        
         public override string Name => @"/showwithdates";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, "Введите две даты в формате(ДД.MM.ГГГГ ДД.MM.ГГГГ)",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
            AppSettings.StateList[chatId] = AppSettings.State.GetDates;
        }
    }
}
