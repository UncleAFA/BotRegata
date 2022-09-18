using Npgsql;
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
                case State.GetDates:
                    await ShowDatesStat(message, botClient);
                    return;
            }
        }
        internal async Task ShowDatesStat(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            string[] Dates = message.Text.Split(" ");
            if (Dates.Length != 2)
            {
                await botClient.SendTextMessageAsync(chatId, "Даты должны быть через пробел",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
                return;
            }
            if (!DateTime.TryParse(Dates[0], out DateTime res1) & !DateTime.TryParse(Dates[1], out DateTime res2))
            {
                await botClient.SendTextMessageAsync(chatId, "Введите две даты в формате(ММ.ДД.ГГГГ ММ.ДД.ГГГГ)",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
                return;
            }

            List<string> Students = new List<string>();
            {
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
                        Students.Add(reader.GetString(0));
                    }
                    catch { }
                }
                con.Close();
            }
            Students.Add("Весь класс");

            List<InserLine> UserScore = new List<InserLine>();
            {
                var date1 = DateTime.Parse(Dates[0]);
                var date2 = DateTime.Parse(Dates[1]);
                const string conn_param = "Server=ec2-44-210-36-247.compute-1.amazonaws.com;Port=5432;UserId=yynhdunqmjakst;Password=b2eee22357bd873fb6bac4f520e7e9734caab9ec36c44d473075ee0af9649517;Database=dal8m5kgg404j1;";//Строка подключения к базе
                NpgsqlConnection con = new NpgsqlConnection(conn_param);
                NpgsqlCommand com = new NpgsqlCommand($"select fio,points from listrecords WHERE date BETWEEN '{date1.Month}.{date1.Day}.{date1.Year}' AND '{date2.Month}.{date2.Day}.{date2.Year}'", con);
                con.Open();
                NpgsqlDataReader reader;
                reader = com.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    try
                    {
                        UserScore.Add(new InserLine()
                        {
                            Fio = reader.GetString(0),
                            Points = reader.GetString(1)
                        });
                    }
                    catch { }
                }
                con.Close();
            }
            string result = "Все результаты: \n";
            float[] Score = new float[Students.Count];
            foreach (var us in UserScore)
            {
                for (int i = 0; i < Students.Count; i++)
                {
                    if (us.Fio == Students[i])
                    {
                        Score[i] += float.Parse(us.Points);
                        //if (us.Points[0] == '+' )
                        //{
                        //    Score[i] += float.Parse( us.Points.Remove(0, 1));
                        //}
                        //if (us.Points[0] == '-')
                        //{
                        //    Score[i] -= float.Parse(us.Points.Remove(0, 1));
                        //}
                    }
                }
            }
            for (int i = 0; i < Students.Count; i++)
            {
                result += $"{Students[i]} === {Score[i]}\n";
            }
            float total = 0;
            for (int i = 0; i < Score.Length; i++)
            {
                total += Score[i];
            }
            result += $"Всего === {total}|{13 * 90}";//TODO:надо сделать так что бы {13*90} считалось так что 13(кол детей) бралось из таблицы детей и 90(путь регаты туда обратно) тоже брался из таблицы сетингс(в будужем надо сделать)
            
            await botClient.SendTextMessageAsync(chatId, result,
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
            StateList[message.Chat.Id] = State.None;
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
