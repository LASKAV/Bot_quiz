using System;
namespace main
{
    public class User
    {
        public string user_tgid;
        public DateTime date;
        public string login;
        public string password;

        public User(string user_tgid, DateTime date,
            string login, string password)
        {
            this.user_tgid = user_tgid;
            this.date = new DateTime(date.Year, date.Month, date.Day);
            this.login = login;
            this.password = password;
        }

        public void Show_user()
        {
            Console.WriteLine($"\nuser_tgid: {this.user_tgid}");
            Console.WriteLine($"date: {this.date.ToShortDateString()}");
            Console.WriteLine($"login: {this.login}");
            Console.WriteLine($"password: {this.password}");

        }
    }
}

