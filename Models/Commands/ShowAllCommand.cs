using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotRegata.Models.Commands
{
    public class ShowAllCommand : Command
    {
        
         public override string Name => @"/showall";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
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
            

            List<InserLine> UserScore = new List<InserLine>();
            {
                const string conn_param = "Server=ec2-44-210-36-247.compute-1.amazonaws.com;Port=5432;UserId=yynhdunqmjakst;Password=b2eee22357bd873fb6bac4f520e7e9734caab9ec36c44d473075ee0af9649517;Database=dal8m5kgg404j1;";//Строка подключения к базе
                NpgsqlConnection con = new NpgsqlConnection(conn_param);
                NpgsqlCommand com = new NpgsqlCommand("select fio,points from listrecords", con);
                con.Open();
                NpgsqlDataReader reader;
                reader = com.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    try
                    {
                        UserScore.Add(new InserLine() {
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
            result += $"Всего === {total}|хххх";
            var chatId = message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, result,
                                                 parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                                                 );
        }
    }
}
