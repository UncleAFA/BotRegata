using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRegata.Models.Commands
{
    public class ShowOnePersoneCommand : Command
    {
        
         public override string Name => @"/showonepersone";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var keyboard = new InlineKeyboardMarkup
            (
                InlineKey()
            );

            var chatId = message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, "Выберите Фамилию",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                                                 replyMarkup: keyboard
                                                 );
            AppSettings.StateList[chatId] = AppSettings.State.ShowNamePoint;
        }

        public InlineKeyboardButton[][] InlineKey()
        {

            InlineKeyboardButton[][] g = new InlineKeyboardButton[15][];
            string result = "";
            const string conn_param = "Server=ec2-44-210-36-247.compute-1.amazonaws.com;Port=5432;UserId=yynhdunqmjakst;Password=b2eee22357bd873fb6bac4f520e7e9734caab9ec36c44d473075ee0af9649517;Database=dal8m5kgg404j1;";//Строка подключения к базе
            NpgsqlConnection con = new NpgsqlConnection(conn_param);
            NpgsqlCommand com = new NpgsqlCommand("select * from students", con);
            con.Open();
            NpgsqlDataReader reader;
            reader = com.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                try
                {

                    result = reader.GetString(0);
                    g[i] = new[]
                    {
                        InlineKeyboardButton.WithCallbackData(result,result),
                    };
                }
                catch { }
                i++;
            }
            con.Close();
            g[i] = new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Отмена","Отмена"),
                    };
            return g;
        }
    }
}
