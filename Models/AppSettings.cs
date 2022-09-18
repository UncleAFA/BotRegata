using System.Collections.Generic;

namespace BotRegata.Models
{
    public static class AppSettings
    {
        //Test URL
        //ngrok http https://localhost:5001
        //public static string Url { get; set; } = "https://b2d8-90-154-71-80.eu.ngrok.io" + "/{0}";
        //public static string Name { get; set; } = "AFA_BOT_TODO_BOT";
        //public static string Key { get; set; } = "2044514869:AAF9wjbsxGXl0SgkMSyeNxpKFRcL28OfFXQ";

        //Pulish URL
        public static string Url { get; set; } = "https://appnameafa-heroku-20.herokuapp.com" + "/{0}";
        public static string Name { get; set; } = "RegtaBotHellper";
        public static string Key { get; set; } = "5655536306:AAEiZF4wJ5ZPNoo67EtzbkF6DLzgTB7sjVQ";

        public static Dictionary<long, State> StateList = new Dictionary<long, State>();
        public static Dictionary<long, InserLine> InsertList = new Dictionary<long, InserLine>();
        public enum State
        {
            None,
            Fio,
            Points,
            Details,
            Confirm,
            ShowNamePoint
        }
    }
}