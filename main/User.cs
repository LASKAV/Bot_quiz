using System;
namespace main
{
    public class User
    {
        public string UserTgid { get; set; }
        public DateTime Date { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public User() { }

        public User(string userTgid, DateTime date,
            string login, string password)
        {
            this.UserTgid = userTgid;
            this.Date = new DateTime(date.Year, date.Month, date.Day);
            this.Login = login;
            this.Password = password;
        }

        public void Show_user()
        {
            Console.WriteLine($"\nuser_tgid: {this.UserTgid}");
            Console.WriteLine($"date: {this.Date.ToShortDateString()}");
            Console.WriteLine($"login: {this.Login}");
            Console.WriteLine($"password: {this.Password}");

        }
    }
}

