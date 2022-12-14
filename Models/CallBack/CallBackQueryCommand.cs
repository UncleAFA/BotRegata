using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.IO;
using static BotRegata.Models.AppSettings;
using Telegram.Bot.Types.InputFiles;

namespace BotRegata.Models.CallBack
{
    public class CallBackQueryCommand
    {
        internal async Task Execute(CallbackQuery message, TelegramBotClient botClient)
        {
            var chatId = message.Message.Chat.Id;
            try
            {
                var i = StateList[message.Message.Chat.Id];
            }
            catch (Exception)
            {
                StateList[message.Message.Chat.Id] = State.None;
            }

            if (StateList[message.Message.Chat.Id] == State.None)
            {
                StateList[message.Message.Chat.Id] = State.None;
                await botClient.SendTextMessageAsync(chatId, $"Что-то пошло не так(",
                                             parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                return;
            }

            if (message.Data == "Отмена")
            {
                await botClient.SendTextMessageAsync(chatId, $"Вы выбрали: {message.Data}\n Ввод новой записи прекращен",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                StateList[message.Message.Chat.Id] = State.None;
                return;
            }
            switch (StateList[message.Message.Chat.Id])
            {
                case State.Fio:
                    var Persone = new InserLine();
                    Persone.Fio = message.Data;
                    InsertList[message.Message.Chat.Id] = Persone;
                    StateList[message.Message.Chat.Id] = State.Points;
                    break;
                case State.ShowNamePoint:
                    await ShowNamePoint(message, botClient);
                    StateList[message.Message.Chat.Id] = State.None;
                    return;
                case State.None:
                    await botClient.SendTextMessageAsync(chatId, $"Введите /add что бы начать ввод",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    break;
                case State.Confirm:
                    if (message.Data == "Да")
                    {
                        var persone = InsertList[message.Message.Chat.Id];
                        persone.Date = DateTime.Now;
                        InsertList[message.Message.Chat.Id] = persone;

                        const string conn_param = "Server=ec2-44-210-36-247.compute-1.amazonaws.com;Port=5432;UserId=yynhdunqmjakst;Password=b2eee22357bd873fb6bac4f520e7e9734caab9ec36c44d473075ee0af9649517;Database=dal8m5kgg404j1;";//Строка подключения к базе
                        string sql = $"INSERT INTO listrecords VALUES ('{InsertList[message.Message.Chat.Id].Fio}','{InsertList[message.Message.Chat.Id].Points}','{InsertList[message.Message.Chat.Id].Details}','{InsertList[message.Message.Chat.Id].Date.Year}-{InsertList[message.Message.Chat.Id].Date.Month}-{InsertList[message.Message.Chat.Id].Date.Day}')";
                        NpgsqlConnection conn = new NpgsqlConnection(conn_param);
                        NpgsqlCommand comm = new NpgsqlCommand(sql, conn);
                        conn.Open(); //Открываем соединение.
                        var result = comm.ExecuteNonQuery().ToString(); //Выполняем нашу команду.
                        Console.WriteLine(result);
                        conn.Close(); //Закрываем соединение.
                        await botClient.SendTextMessageAsync(chatId, $"Данные занесены",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        await botClient.SendTextMessageAsync(466206177, $" #Занесенные Данные : ('{InsertList[message.Message.Chat.Id].Fio}','{InsertList[message.Message.Chat.Id].Points}','{InsertList[message.Message.Chat.Id].Details}','{InsertList[message.Message.Chat.Id].Date.Year}-{InsertList[message.Message.Chat.Id].Date.Month}-{InsertList[message.Message.Chat.Id].Date.Day}')",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        StateList[message.Message.Chat.Id] = State.None;
                        InsertList[message.Message.Chat.Id] = new InserLine();
                        return;
                    }
                    if (message.Data == "Нет")
                    {
                        await botClient.SendTextMessageAsync(chatId, $"Я отменил ввод ",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                        StateList[message.Message.Chat.Id] = State.None;
                        return;
                    }

                    break;
            }

            await botClient.SendTextMessageAsync(chatId, $"Вы выбрали: {message.Data}\n Введите количество баллов(пример:+5,9 или -5,8)",
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
       

        internal async Task ShowNamePoint(CallbackQuery message, TelegramBotClient botClient)
        {
            List<InserLine> ListData = new List<InserLine>();
            const string conn_param = "Server=ec2-44-210-36-247.compute-1.amazonaws.com;Port=5432;UserId=yynhdunqmjakst;Password=b2eee22357bd873fb6bac4f520e7e9734caab9ec36c44d473075ee0af9649517;Database=dal8m5kgg404j1;";//Строка подключения к базе
            NpgsqlConnection con = new NpgsqlConnection(conn_param);
            NpgsqlCommand com;
            if (message.Data == "Запси за все время")
            {
                com = new NpgsqlCommand($"select * from listrecords", con);
            }
            else
            {
                com = new NpgsqlCommand($"select * from listrecords where fio ='{message.Data}'", con);
                
            }
            con.Open();
            NpgsqlDataReader reader;
            reader = com.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    ListData.Add(new InserLine() { Fio = reader.GetString(0).ToString(), Points = reader.GetString(1).ToString(), Details = reader.GetString(2).ToString(), Date = Convert.ToDateTime(reader.GetDateTime(3)) });
                }
                catch (Exception x) { Console.WriteLine(x); }
            }
            con.Close();
            string OutText = "";
            foreach (var item in ListData)
            {
                OutText += $"{item.Fio}    {item.Points}  -  {item.Details}  -  {item.Date.ToShortDateString()} \n";
            }
            if (OutText == "")
            {
                OutText = "Записи отсутствуют";
            }
            System.IO.File.WriteAllText(Environment.CurrentDirectory + $@"\file{message.Message.Chat.Id}.txt", OutText);
            using (var stream = System.IO.File.OpenRead(Environment.CurrentDirectory + $@"\file{message.Message.Chat.Id}.txt"))
            {
                InputOnlineFile iof = new InputOnlineFile(stream);
                iof.FileName = $"{message.Data}{DateTime.Now.ToShortDateString()}.txt";
                await botClient.SendDocumentAsync(message.Message.Chat.Id, iof);
            }          
            
            
            System.IO.File.Delete(Environment.CurrentDirectory + $@"\file{message.Message.Chat.Id}.txt");

        }
    }
}
