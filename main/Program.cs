using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static main.User;
using static main.Status;
using static main.EmptyStruct;
using main;
using System.Threading;
using Telegram.Bot.Requests;
using System.Globalization;
using Microsoft.Win32;
using System.Runtime.ConstrainedExecution;
using MongoDB.Bson;
using System.Data;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Collections.Generic;


// Подключаем бота через свой API key
const string Token = "6250535845:AAG4xaJ4ls3J5gjFktEHQgr9ddI0iwTQBqU";
var bot = new TelegramBotClient(Token);
using CancellationTokenSource cts = new();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

/*
 // хелп команд 
//var commands = new List<BotCommand>
//{
//    new BotCommand { Command = "start", Description = "Запустить бота" },
//    new BotCommand { Command = "help", Description = "Показать помощь" }
//};
//
//
//bot.SetMyCommandsAsync(commands);
 */

Status status = Status.defaul;
EmptyStruct empty = new EmptyStruct();

var db = new DatabaseMongoDB();


Dictionary<string, main.User> users = new Dictionary<string, main.User>();

bot.StartReceiving(
    updateHandler: Update,
    pollingErrorHandler: Error,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await bot.GetMeAsync();
Console.WriteLine($"Запус бота @{me.Username}");

Console.ReadLine();
cts.Cancel();

async Task Update(ITelegramBotClient bot,Update update,CancellationToken Token)
{
    
    if (update.Type == UpdateType.Message &&
        update?.Message?.Text != null)
    {

        var userID = update.Message.Chat.Id;
        var messageText = update.Message.Text;
        var firstName = update.Message.From.FirstName;
        var lastName = update.Message.From.LastName;

        Console.WriteLine(
            $"\nchatID: {userID}" +
            $"\nmessageText: {messageText}" +
            $"\nfirstName: {firstName}" +
            $"\nlastName: {lastName}");


        if (db.GetUserByUserID($"{userID}") == null)
        {
            if (!users.TryAdd($"{userID}", new main.User()))
            {
                await HandleMesssageLogger(bot, update.Message, $"{userID}");
                return;
            }
        }
        else
        {
            if (!users.TryAdd($"{userID}", new main.User()))
            {
                await HandleMesssage(bot, update.Message, $"{userID}");
                return;
            }
                
        }

        Console.WriteLine("Update");
        return;
    }
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(bot, update.CallbackQuery);
        return;
    }

}
async Task HandleMesssageLogger (ITelegramBotClient bot, Message message, string user_id)
{
    if (users.ContainsKey(user_id))
    {
        users[user_id].UserTgid = $"{user_id}";
        Console.WriteLine(user_id);
        Console.WriteLine("Mои пользователи: \n");
        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"User ID: {user.Key}");
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Login: {user.Value.Login}");
            Console.WriteLine($"Password: {user.Value.Password}");
            Console.WriteLine($"Date: {user.Value.Date}\n");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }
    }
    if (message.Text.StartsWith("/start"))
    {
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT: </code> " +
        $"<b>Привет {message.From.FirstName} 👋</b>" +
        $"\n<b>Для игры вам нужно пройти простую аторизацию</b>",
        replyMarkup: Logger(),
        parseMode: ParseMode.Html);

        users[user_id].Status = Status.defaul;
        return;
    }
    if (message.Text.StartsWith("1⃣ Логин"))
    {
        await bot.SendTextMessageAsync(
         message.Chat.Id,
         $"<code>🤖 BOT:</code><b> Придумай логин: </b> ",
         parseMode: ParseMode.Html
         );
        users[user_id].Status = Status.login;
        return;
    }
    if (message.Text.StartsWith("2⃣ Пароль"))
    {
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code><b> Придумай пароль: </b> ",
        parseMode: ParseMode.Html
        );
        users[user_id].Status = Status.password;
        return;
    }
    if (message.Text.StartsWith("3⃣ Дата рождения"))
    {
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code><b> Введите дату рождения: </b> ",
        parseMode: ParseMode.Html
        );
        users[user_id].Status = Status.date;
        return;
    }
    if (message.Text.StartsWith("4⃣ Проверка ✅"))
    {
        if (users.ContainsKey(user_id))
        {
            Console.WriteLine("ПРОВЕРКА: \n");

            Console.WriteLine("_________________________");

            Console.WriteLine($"User ID: {user_id}");
            Console.WriteLine($"UserTgid: {users[user_id].UserTgid}");
            Console.WriteLine($"Login: {users[user_id].Login}");
            Console.WriteLine($"Password: {users[user_id].Password}");
            Console.WriteLine($"Date: {users[user_id].Date}\n");

            Console.WriteLine("_________________________");
            if (!string.IsNullOrEmpty(users[user_id].UserTgid) &&
                !string.IsNullOrEmpty(users[user_id].Login) &&
                !string.IsNullOrEmpty(users[user_id].Password) &&
                users[user_id].Date != DateTime.MinValue)
            {

               await bot.SendTextMessageAsync(message.Chat.Id,
                      $"<code>🤖 BOT: </code> " +
                      "<b>Регистрация прошла успешно!✅</b>",
                      parseMode: ParseMode.Html);
               await bot.SendTextMessageAsync(
                  message.Chat.Id,
                  empty.MainOffice,
                  replyMarkup: Top_menu(),
                  parseMode: ParseMode.Html
                  );

               db.InsertUser(
                   users[user_id].UserTgid,
                   users[user_id].Date,
                   users[user_id].Login,
                   users[user_id].Password);

                users[user_id].Status = Status.defaul;
                return;
            }
            else
            {
                await bot.SendTextMessageAsync(message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                "<b> Не все данные были введены, повторите попытку 🚫</b>",
                parseMode: ParseMode.Html,
                replyMarkup: Logger());

                users[user_id].Status = Status.defaul;
                return;
            }

        }    
    }

    if (users.ContainsKey(user_id) && users[user_id].Status == Status.login)
    {
        string login = message.Text;

            Console.WriteLine(status);

            Console.WriteLine($"login = {login}");
            await bot.SendTextMessageAsync(message.Chat.Id,
                 $"<code>🤖 BOT: </code> " +
                $"<b>Логин сохранен: {login}✅ </b>",
                 replyMarkup: Logger(),
                 parseMode: ParseMode.Html);

        users[user_id].Login = login;

        users[user_id].Status = Status.defaul;

        return;

            
    }
    // пользователь вводит логин
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.password)
    {
        string password = message.Text;


            Console.WriteLine($"password = {password}");
            await bot.SendTextMessageAsync(message.Chat.Id,
                 $"<code>🤖 BOT: </code> " +
                $"<b>Пароль сохранен: {password}✅</b>",
                 replyMarkup: Logger(),
                 parseMode: ParseMode.Html);

        users[user_id].Password = password;
        users[user_id].Status = Status.defaul;

         return;
        
    }
    // пользователь вводит пароль
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.date)
    {
        // преобразовываем строку из даты
        string date = message.Text;
        string format = "dd.MM.yyyy";

        if (users.ContainsKey(user_id))
        {
            DateTime dateTime;

            if (DateTime.TryParseExact(date, format, null,
                DateTimeStyles.None, out dateTime))
            {
                Console.WriteLine($"date = {dateTime}");
                await bot.SendTextMessageAsync(message.Chat.Id,
                    $"<code>🤖 BOT: </code> " +
                    $"<b>Дата сохранена: {dateTime.ToString(format)}✅</b>",
                    replyMarkup: Logger(),
                    parseMode: ParseMode.Html);

                users[user_id].Date = dateTime;
                users[user_id].Status = Status.defaul;

                return;
            }
            else
            {
                await bot.SendTextMessageAsync(message.Chat.Id,
                    $"<code>🤖 BOT: </code> " +
                    "<b>Некорректный формат даты. Попробуйте еще раз.🚫</b>" +
                    $"<b>\nПример: </b> <code> 12.12.2012 </code> ",
                    replyMarkup: Logger(),
                    parseMode: ParseMode.Html);

                users[user_id].Status = Status.defaul;

                return;
            }
        }
            
    }
    // пользователь вводит дату

    await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT: </code> " +
        $"<b>Привет {message.From.FirstName} 👋</b>" +
        $"\n<b>Для игры вам нужно пройти простую аторизацию</b>",
        replyMarkup: Logger(),
        parseMode: ParseMode.Html);

    users[user_id].Status = Status.defaul;

    return;

}
async Task HandleMesssage(ITelegramBotClient bot, Message message, string user_id)
{
    var questionsHistory = db.GetRandomQuestionsFromDb(20, 0);
    var questionsGeographies = db.GetRandomQuestionsFromDb(20, 1);
    var questionsBiology = db.GetRandomQuestionsFromDb(20, 2);
    var questionsMix = db.GetRandomQuestionsFromDb(20, 3);

    if (users.ContainsKey(user_id))
    {
        users[user_id].UserTgid = user_id;
        Console.WriteLine("User: \n");
        Console.WriteLine("_________________________");
        Console.WriteLine($"User ID: {users[user_id]}");
        Console.WriteLine("_________________________");
        Console.WriteLine("Mои пользователи: \n");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }

        Console.WriteLine("_________________________");
    }
    if (message.Text.StartsWith("/start"))
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            empty.MainOffice,
            replyMarkup: Top_menu(),
            parseMode: ParseMode.Html
            );
            users[user_id].Status = Status.defaul;

            return;
        }

    // кнопки

    if (message.Text.StartsWith("🎮 Играть 🎮"))
        {
            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code><b> Выбери викторину 🔮 </b>",
                replyMarkup: Game_menu(),
                parseMode: ParseMode.Html);
            users[user_id].Status = Status.game;

            return;
        }
    if (message.Text.StartsWith("🛠 Настройки 🛠"))
        {
            
            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code><b> Выбери раздел 🛠 </b>",
                replyMarkup: Settings_menu(),
                parseMode: ParseMode.Html);
            users[user_id].Status = Status.settings;
 
            return;
        }
    if (message.Text.StartsWith("📈 Статистика 📉"))
    {
        if (message.Text == "📈 Статистика 📉")
        {
            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code><b> Выбери статистику 📊</b>",
                replyMarkup: Statistics_menu(),
                parseMode: ParseMode.Html);
        }
        users[user_id].Status = Status.userStats;

        return;
    }
    if (message.Text.StartsWith("🔙 Назад 🔙"))
        {
            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code><b> назад  🚀 </b>",
                replyMarkup: Top_menu(),
                parseMode: ParseMode.Html);

            users[user_id].Status = Status.defaul;

            return;
        }
    if (message.Text.StartsWith("Отсановить игру"))
        {
            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b>Игра окончена! </b>\n",
                parseMode: ParseMode.Html,
                replyMarkup: Game_menu()
                );
        users[user_id].Status = Status.game;

            return;
        }

    // состояние бота

    if (users.ContainsKey(user_id) && users[user_id].Status == Status.settings)
        // пользователь в menu settings
        {
            Console.WriteLine($"Status:{status}");
            if (message.Text.StartsWith("🔧 Смена пароля 🔧"))
            {

                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code><b> Введите новый пароль: </b> ",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.passwordChang;
                return;
            }
            if (message.Text.StartsWith("👶 Смена даты рождения 👶"))
            {

                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code><b> Введите новую дату рождения: </b> ",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.birthdayСhange;
                return;
            }
        }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.birthdayСhange)
        {
            string date = message.Text;
            string format = "dd.MM.yyyy";
            DateTime dateTime;

            if (DateTime.TryParseExact(date, format, null,
                DateTimeStyles.None, out dateTime))
            {
                Console.WriteLine($"date = {dateTime}");
                await bot.SendTextMessageAsync(message.Chat.Id,
                    $"<code>🤖 BOT: </code> " +
                    $"<b>Дата сохранена: {dateTime.ToString(format)}✅</b>",
                    replyMarkup: Settings_menu(),
                    parseMode: ParseMode.Html);
                db.UpdateUserDate(users[user_id].UserTgid, dateTime);

                users[user_id].Status = Status.settings;

                return;
            }
            else
            {
                await bot.SendTextMessageAsync(message.Chat.Id,
                    $"<code>🤖 BOT: </code> " +
                    "<b>Некорректный формат даты. Попробуйте еще раз.🚫</b>" +
                    $"<b>\nПример: </b> <code> 12.12.2012 </code> ",
                    replyMarkup: Settings_menu(),
                    parseMode: ParseMode.Html);

                users[user_id].Status = Status.settings;

                return;
            }
        }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.passwordChang) // пользователь новый пароль
        {
            string newPassword = message.Text;
            db.UpdateUserPassword(users[user_id].UserTgid, newPassword);
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> Пароль был изменен на {newPassword}✅</b> ",
            parseMode: ParseMode.Html
            );

            users[user_id].Status = Status.settings;

            return;
        }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.game)
        {
            if (message.Text.StartsWith("💂‍♀️ История 👩‍🚀"))
            {
            db.InsertUser_history($"{users[user_id].UserTgid}", DateTime.UtcNow);
            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b> Вы выбрали раздел 💂‍♀️ История 👩‍🚀 </b>\n" +
                empty.Awards,
                replyMarkup: Geme_History_start(),
                parseMode: ParseMode.Html
                );
                users[user_id].Status = Status.gameHistory;
                Console.WriteLine("Тут я выбрал раздел ");
                return;
            }
            if (message.Text.StartsWith("🏛 География ✈️"))
                {
            db.InsertUser_history($"{users[user_id].UserTgid}", DateTime.UtcNow);
            await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    $"<code>🤖 BOT:</code>" +
                    $"<b> Вы выбрали раздел 🏛 География ✈️</b>\n" +
                    empty.Awards,
                    replyMarkup: Geme_History_start(),
                    parseMode: ParseMode.Html
                    );
                    users[user_id].Status = Status.gameGeographies;
                    return;
                }
            if (message.Text.StartsWith("🔬 Биология 🦠"))
                {
            db.InsertUser_history($"{users[user_id].UserTgid}", DateTime.UtcNow);
            await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    $"<code>🤖 BOT:</code>" +
                    $"<b> Вы выбрали раздел 🔬 Биология 🦠 </b>\n" +
                    empty.Awards,
                    replyMarkup: Geme_History_start(),
                    parseMode: ParseMode.Html
                    );
                    users[user_id].Status = Status.gameBiology;
                    return;
                }
            if (message.Text.StartsWith("👽 Смешанная 👀"))
                {
            db.InsertUser_history($"{users[user_id].UserTgid}", DateTime.UtcNow);
            await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    $"<code>🤖 BOT:</code>" +
                    $"<b> Вы выбрали раздел 👽 Смешанная 👀 </b>\n" +
                    empty.Awards,
                    replyMarkup: Geme_History_start(),
                    parseMode: ParseMode.Html
                    );
                    users[user_id].Status = Status.gameMix;
                    return;
                }
    }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.userStats)
    {
        if (message.Text.StartsWith("🎖 Результаты прошлых викторин 🎖"))
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> находиться в разроботке  </b>\n",
            replyMarkup: Statistics_menu(),
            parseMode: ParseMode.Html
            );

            users[user_id].Status = Status.userStats;
            return;
        }
        if (message.Text.StartsWith("🏆 ТОП - 20 по разделам 🏆"))
        {
            var stat_user = db.GetAllUserHistory();

            await bot.SendTextMessageAsync(
                 message.Chat.Id,
                 $"<code>🤖 BOT:</code>" +
                 $"<b> 🏆 ТОП - 20 🏆</b>\n",
                 replyMarkup: Statistics_menu(),
                 parseMode: ParseMode.Html
                 );
            foreach (var gameHistory in stat_user)
            {
                await bot.SendTextMessageAsync(
                 message.Chat.Id,
                 $"<b>\n👻 User :  {gameHistory["UserID"]}</b>" +
                 $"<b>\n\n💂‍♀️ История 👩‍🚀:  {gameHistory["Points_gameHistory"]} очков</b>" +
                 $"<b>\n\n🏛 География ✈️:  {gameHistory["Points_gameGeographies"]} очков</b>" +
                 $"<b>\n\n🔬 Биология 🦠:  {gameHistory["Points_gameBiology"]} очков</b>" +
                 $"<b>\n\n👽 Смешанная 👀:  {gameHistory["Points_gameMix"]} очков</b>",
                 replyMarkup: Statistics_menu(),
                 parseMode: ParseMode.Html
                 );

            }
            users[user_id].Status = Status.userStats;
            return;


        }
        if (message.Text.StartsWith("📕 Общия статистика 📕"))
        {
           
          var userHistory = db.GetUserHistory($"{users[user_id].UserTgid}");

            foreach (var gameHistory in userHistory)
            {
                await bot.SendTextMessageAsync(
                 message.Chat.Id,
                 $"<code>🤖 BOT:</code>" +
                 $"<b> Общия статистика 📊</b>\n" +
                 $"<b>\n💂‍♀️ История 👩‍🚀:  {gameHistory["Points_gameHistory"]} очков</b>" +
                 $"<b>\n\n🏛 География ✈️:  {gameHistory["Points_gameGeographies"]} очков</b>" +
                 $"<b>\n\n🔬 Биология 🦠:  {gameHistory["Points_gameBiology"]} очков</b>" +
                 $"<b>\n\n👽 Смешанная 👀:  {gameHistory["Points_gameMix"]} очков</b>",
                 replyMarkup: Statistics_menu(),
                 parseMode: ParseMode.Html
                 );
                users[user_id].Status = Status.userStats;
                return;
            }
           
        }    
    }

    if (users.ContainsKey(user_id) && users[user_id].Status == Status.gameHistory)
    {
        questionsHistory = db.GetRandomQuestionsFromDb(20, 0);

        Console.WriteLine("Тут задают вопрос !");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }

        db.UpdateQuestions($"{users[user_id].UserTgid}");
        
        foreach (var question in questionsHistory)
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Вопрос {question["question"]} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_Answer
            (question["answer"].ToString(), question["badQuestion1"].ToString(),
            question["badQuestion2"].ToString(), question["badQuestion3"].ToString())
            );
            Console.WriteLine($"ID вопроса {question["id"]}");

            users[user_id].Status = Status.gameAnswerHistory;
            users[user_id].IdQuse = (int)question["id"];

            return;
        }
             await bot.SendTextMessageAsync(
             message.Chat.Id,
             $"<code>🤖 BOT:</code>" +
             $"<b>Игра окончена! </b>\n",
             parseMode: ParseMode.Html,
             replyMarkup: Game_menu()
             );

             users[user_id].Status = Status.game;

             return;
    }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.gameGeographies)
    {
        questionsGeographies = db.GetRandomQuestionsFromDb(20, 1);

        Console.WriteLine("Тут задают вопрос !");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }

        db.UpdateQuestions($"{users[user_id].UserTgid}");

        foreach (var question in questionsGeographies)
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Вопрос {question["question"]} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_Answer
            (question["answer"].ToString(), question["badQuestion1"].ToString(),
            question["badQuestion2"].ToString(), question["badQuestion3"].ToString())
            );
            Console.WriteLine($"ID вопроса {question["id"]}");

            users[user_id].Status = Status.gameAnswerGeographies;
            users[user_id].IdQuse = (int)question["id"];

            return;
        }
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code>" +
        $"<b>Игра окончена! </b>\n",
        parseMode: ParseMode.Html,
        replyMarkup: Game_menu()
        );

        users[user_id].Status = Status.game;

        return;
    }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.gameBiology)
    {
        questionsBiology = db.GetRandomQuestionsFromDb(20, 2);

        Console.WriteLine("Тут задают вопрос !");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }

        db.UpdateQuestions($"{users[user_id].UserTgid}");

        foreach (var question in questionsBiology)
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Вопрос {question["question"]} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_Answer
            (question["answer"].ToString(), question["badQuestion1"].ToString(),
            question["badQuestion2"].ToString(), question["badQuestion3"].ToString())
            );
            Console.WriteLine($"ID вопроса {question["id"]}");

            users[user_id].Status = Status.gameAnswerBiology;
            users[user_id].IdQuse = (int)question["id"];
            return;
        }
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code>" +
        $"<b>Игра окончена! </b>\n",
        parseMode: ParseMode.Html,
        replyMarkup: Game_menu()
        );

        users[user_id].Status = Status.game;

        return;
    }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.gameMix)
    {
        questionsMix = db.GetRandomQuestionsFromDb(20, 3);

        Console.WriteLine("Тут задают вопрос !");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }

        db.UpdateQuestions($"{users[user_id].UserTgid}");

        foreach (var question in questionsMix)
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Вопрос {question["question"]} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_Answer
            (question["answer"].ToString(), question["badQuestion1"].ToString(),
            question["badQuestion2"].ToString(), question["badQuestion3"].ToString())
            );
            Console.WriteLine($"ID вопроса {question["id"]}");

            users[user_id].Status = Status.gameAnswerMix;
            users[user_id].IdQuse = (int)question["id"];
            return;
        }
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code>" +
        $"<b>Игра окончена! </b>\n",
        parseMode: ParseMode.Html,
        replyMarkup: Game_menu()
        );

        users[user_id].Status = Status.game;

        return;
    }

    if (users.ContainsKey(user_id) && users[user_id].Status == Status.gameAnswerHistory)
    {

        string answer = message.Text;
        Console.WriteLine("Тут ответ на ворос !");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}");
            Console.WriteLine($"ID ques: {user.Value.IdQuse}\n");
        }

            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Ответ: {answer} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_process()
            );
            int indexq = (int)users[user_id].IdQuse;

            Console.WriteLine($"ID ответа {indexq}");

            Console.WriteLine("_________________________");
            Console.WriteLine($"User ID: {users[user_id].UserTgid}");
            Console.WriteLine("_________________________");


            if (db.CheckRandomAnswer(indexq, answer, 0))
            {
                db.UpdatePoint(users[user_id].UserTgid,0);

                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b> Правильно ✅ </b>\n",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.gameHistory;
                return;
            }
            else
            {
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b>Неправильно ❌</b>",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.gameHistory;
                return;
            }
    }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.gameAnswerGeographies)
    {
        string answer = message.Text;
        Console.WriteLine("Тут ответ на ворос !");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }

            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Ответ: {answer} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_process()
            );
            int indexq = (int)users[user_id].IdQuse;
            Console.WriteLine($"ID ответа {indexq}");

            Console.WriteLine("_________________________");
            Console.WriteLine($"User ID: {users[user_id].UserTgid}");
            Console.WriteLine("_________________________");


            if (db.CheckRandomAnswer(indexq, answer, 1))
            {
                db.UpdatePoint(users[user_id].UserTgid,1);

                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b> Правильно ✅ </b>\n",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.gameGeographies;
                return;
            }
            else
            {
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b>Неправильно ❌</b>",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.gameGeographies;
                return;
            }
    
    }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.gameAnswerBiology)
    {
        string answer = message.Text;
        Console.WriteLine("Тут ответ на ворос !");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }

            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Ответ: {answer} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_process()
            );
            int indexq = (int)users[user_id].IdQuse;
            Console.WriteLine($"ID ответа {indexq}");

            Console.WriteLine("_________________________");
            Console.WriteLine($"User ID: {users[user_id].UserTgid}");
            Console.WriteLine("_________________________");


            if (db.CheckRandomAnswer(indexq, answer, 2))
            {
                db.UpdatePoint(users[user_id].UserTgid,2);

                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b> Правильно ✅ </b>\n",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.gameBiology;
                return;
            }
            else
            {
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b>Неправильно ❌</b>",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.gameBiology;
                return;
            }

    }
    if (users.ContainsKey(user_id) && users[user_id].Status == Status.gameAnswerMix)
    {
        string answer = message.Text;
        Console.WriteLine("Тут ответ на ворос !");

        foreach (KeyValuePair<string, main.User> user in users)
        {
            Console.WriteLine($"UserTgid: {user.Value.UserTgid}");
            Console.WriteLine($"Status: {user.Value.Status}\n");
        }

            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Ответ: {answer} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_process()
            );
            int indexq = (int)users[user_id].IdQuse;
            Console.WriteLine($"ID ответа {indexq}");

            Console.WriteLine("_________________________");
            Console.WriteLine($"User ID: {users[user_id].UserTgid}");
            Console.WriteLine("_________________________");


            if (db.CheckRandomAnswer(indexq, answer, 3))
            {
                db.UpdatePoint(users[user_id].UserTgid,3);

                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b> Правильно ✅ </b>\n",
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.gameMix;
                return;
            }
            else
            {
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b>Неправильно ❌</b>" ,
                parseMode: ParseMode.Html
                );

                users[user_id].Status = Status.gameMix;
                return;
            }
    }

    await bot.SendTextMessageAsync(message.Chat.Id,
            $"Это HandleMesssage {message.Text}");
    return;

}
async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callback)
{
    await bot.SendTextMessageAsync(callback.Message.Chat.Id, $"Нажал {callback.Data}");
    return;
}

static IReplyMarkup Top_menu()
{
    //-----------------------------//
    KeyboardButton batton_top_game = "🎮 Играть 🎮";
    KeyboardButton batton_top_stat = "📈 Статистика 📉";
    KeyboardButton batton_top_settings = "🛠 Настройки 🛠";
    //-----------------------------//

    ReplyKeyboardMarkup Top_menu = new(new[]
    {
    new KeyboardButton[] { batton_top_game, batton_top_stat },
    new KeyboardButton[] { batton_top_settings },
    }
    )
    {
        ResizeKeyboard = true
    };

    return Top_menu;
}
static IReplyMarkup Game_menu()
{
    //-----------------------------//

    KeyboardButton batton_Game_History = "💂‍♀️ История 👩‍🚀";
    KeyboardButton batton_Game_Geography = "🏛 География ✈️";
    KeyboardButton batton_Game_Biology = "🔬 Биология 🦠";
    KeyboardButton batton_Game_Mixed = "👽 Смешанная 👀";
    KeyboardButton button_Game_Back = "🔙 Назад 🔙";

    //-----------------------------//

    ReplyKeyboardMarkup Game_menu = new(new[]
    {
    new KeyboardButton[] { batton_Game_History, batton_Game_Geography },
    new KeyboardButton[] { batton_Game_Biology, batton_Game_Mixed},
     new KeyboardButton[] { button_Game_Back},
    }
    )
    {
        ResizeKeyboard = true
    };

    return Game_menu;
}
static IReplyMarkup Statistics_menu()
{
    //-----------------------------//
    KeyboardButton batton_Statistics_History
        = "🎖 Результаты прошлых викторин 🎖";
    KeyboardButton batton_Statistics_Geography
        = "🏆 ТОП - 20 по разделам 🏆";
    KeyboardButton batton_Statistics_Geog
        = "📕 Общия статистика 📕";
    KeyboardButton button_Statistics_Back = "🔙 Назад 🔙";

    //-----------------------------//

    ReplyKeyboardMarkup Statistics_menu = new(new[]
      {
    new KeyboardButton[] { batton_Statistics_History, batton_Statistics_Geography },
    new KeyboardButton[] { batton_Statistics_Geog},
     new KeyboardButton[] { button_Statistics_Back},
    }
      )
    {
        ResizeKeyboard = true
    };

    return Statistics_menu;
}
static IReplyMarkup Settings_menu()
{
    //-----------------------------//
    KeyboardButton batton_Settings_pass
        = "🔧 Смена пароля 🔧";
    KeyboardButton batton_Settings_dates
        = "👶 Смена даты рождения 👶";
    KeyboardButton batton_Settings_Back = "🔙 Назад 🔙";

    //-----------------------------//

    ReplyKeyboardMarkup Settings_menu = new(new[]
      {
    new KeyboardButton[] { batton_Settings_pass, batton_Settings_dates },
     new KeyboardButton[] { batton_Settings_Back},
    }
      )
    {
        ResizeKeyboard = true
    };

    return Settings_menu;
}
static IReplyMarkup Logger()
{
    //-----------------------------//
    KeyboardButton batton_Logger_login
        = "1⃣ Логин ";
    KeyboardButton batton_Logger_password
        = "2⃣ Пароль ";
    KeyboardButton batton_Logger_Date
        = "3⃣ Дата рождения ";
    KeyboardButton batton_Logger_Rgistry_check
         = "4⃣ Проверка ✅ ";

    //-----------------------------//

    ReplyKeyboardMarkup Logger_menu = new
        (new[]
    {
        new KeyboardButton[] { batton_Logger_login, batton_Logger_password },
        new KeyboardButton[] { batton_Logger_Date, batton_Logger_Rgistry_check},
    }
      )
    {
        ResizeKeyboard = true
    };

    return Logger_menu;
}
static IReplyMarkup Geme_History_start()
{
    KeyboardButton batton_Logger_login
        = "Начать игру ";
    KeyboardButton batton_Logger_
        = "🔙 Назад 🔙 ";

    //-----------------------------//

    ReplyKeyboardMarkup Logger_menu = new
        (new[]
    {
        new KeyboardButton[] { batton_Logger_login},
        new KeyboardButton[] { batton_Logger_, },
    }
      )
    {
        ResizeKeyboard = true
    };

    return Logger_menu;
}
static IReplyMarkup Geme_History_Answer
    (string question1, string question2, string question3, string question4)
{
    Random rnd = new Random();

    List<string> options = new List<string>() { question1, question2, question3, question4 };
    options = options.OrderBy(x => rnd.Next()).ToList(); // перемешиваем варианты ответов

    KeyboardButton button1 = $"{options[0]}";
    KeyboardButton button2 = $"{options[1]}";
    KeyboardButton button3 = $"{options[2]}";
    KeyboardButton button4 = $"{options[3]}";
    //KeyboardButton backButton = "🔙 Назад 🔙";

    ReplyKeyboardMarkup keyboard = new(new[]
    {
        new KeyboardButton[] { button1, button2 },
        new KeyboardButton[] { button3, button4 },
    })
    {
        ResizeKeyboard = true
    };

    return keyboard;
}

static IReplyMarkup Geme_History_process()
{
    KeyboardButton batton_Logger_login
        = "Дальше";
    KeyboardButton batton_Logger_
        = "Отсановить игру";

    //-----------------------------//

    ReplyKeyboardMarkup Logger_menu = new
        (new[]
    {
        new KeyboardButton[] { batton_Logger_login,batton_Logger_},
    }
      )
    {
        ResizeKeyboard = true
    };

    return Logger_menu;
}
Task Error(ITelegramBotClient botClient, Exception exception,
    CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}