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
var questions = db.GetRandomQuestionsFromDb(5);
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
            await HandleMesssage(bot, update.Message);
            return;
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
        status = defaul;
        return;
    }
    if (message.Text.StartsWith("1⃣ Логин"))
    {
        status = login;

        await bot.SendTextMessageAsync(
         message.Chat.Id,
         $"<code>🤖 BOT:</code><b> Придумай логин: </b> ",
         parseMode: ParseMode.Html
         );
        return;
    }
    if (message.Text.StartsWith("2⃣ Пароль"))
    {
        status = password;
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code><b> Придумай пароль: </b> ",
        parseMode: ParseMode.Html
        );
        return;
    }
    if (message.Text.StartsWith("3⃣ Дата рождения"))
    {
        status = date;
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code><b> Введите дату рождения: </b> ",
        parseMode: ParseMode.Html
        );
        return;
    }
    if (message.Text.StartsWith("4⃣ Проверка ✅"))
    {
        string user_ID = message.Chat.Id.ToString();
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
              // добавляем объект User в список пользователей
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

               status = defaul;
               return;
            }
            else
            {
                await bot.SendTextMessageAsync(message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                "<b> Не все данные были введены, повторите попытку 🚫</b>",
                parseMode: ParseMode.Html,
                replyMarkup: Logger());

                return;
            }

        }    
    }

    if (status is login)
    {
        string login = message.Text;
        string user_ID = message.Chat.Id.ToString();

        if (users.ContainsKey(user_id))
        {
            Console.WriteLine($"login = {login}");
            await bot.SendTextMessageAsync(message.Chat.Id,
                 $"<code>🤖 BOT: </code> " +
                $"<b>Логин сохранен: {login}✅ </b>",
                 replyMarkup: Logger(),
                 parseMode: ParseMode.Html);

            users[user_id].Login = login;

            status = defaul;
            return;
        }

            
    }    // пользователь вводит логин
    if (status is password)
    {
        string password = message.Text;
        string user_ID = message.Chat.Id.ToString();

        if (users.ContainsKey(user_id))
        {
            Console.WriteLine($"password = {password}");
            await bot.SendTextMessageAsync(message.Chat.Id,
                 $"<code>🤖 BOT: </code> " +
                $"<b>Пароль сохранен: {password}✅</b>",
                 replyMarkup: Logger(),
                 parseMode: ParseMode.Html);

            users[user_id].Password = password;

            status = defaul;
            return;
        }
           
    } // пользователь вводит пароль
    if (status is date)
    {
        // преобразовываем строку из даты
        string date = message.Text;
        string format = "dd.MM.yyyy";
        string user_ID = message.Chat.Id.ToString();

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

                status = defaul;
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
                status = defaul;
                return;
            }
        }
            
    }     // пользователь вводит дату

    await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT: </code> " +
        $"<b>Привет {message.From.FirstName} 👋</b>" +
        $"\n<b>Для игры вам нужно пройти простую аторизацию</b>",
        replyMarkup: Logger(),
        parseMode: ParseMode.Html);
    status = defaul;
    return;

}
async Task HandleMesssage(ITelegramBotClient bot, Message message)
{
    if (message.Text.StartsWith("/start"))
    { 
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            empty.MainOffice,
            replyMarkup: Top_menu(),
            parseMode: ParseMode.Html
            );
            status = defaul;
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
        status = game;
        return;
    }
    if (message.Text.StartsWith("🛠 Настройки 🛠"))
    {
        status = settings;
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> Выбери раздел 🛠 </b>",
            replyMarkup: Settings_menu(),
            parseMode: ParseMode.Html);
        return;
    }
    if (message.Text.StartsWith("🔙 Назад 🔙"))
    {
        status = defaul;
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> назад  🚀 </b>",
            replyMarkup: Top_menu(),
            parseMode: ParseMode.Html);
        return;
    }

    // состояние бота 
    if (status is settings)    // пользователь в menu settings
    {
        Console.WriteLine($"Status:{status}");
        if (message.Text.StartsWith("🔧 Смена пароля 🔧"))
        {
            status = passwordChang;
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> Введите новый пароль: </b> ",
            parseMode: ParseMode.Html
            );
            return;
        }
        if (message.Text.StartsWith("👶 Смена даты рождения 👶"))
        {
            status = birthdayСhange;
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> Введите новую дату рождения: </b> ",
            parseMode: ParseMode.Html
            );
            return;
        }
    }
    if (status is birthdayСhange)
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
            db.UpdateUserDate($"{message.From.Id}", dateTime);
            status = settings;
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
            status = settings;
            return;
        }
    }
    if (status is passwordChang) // пользователь новый пароль
    {
        string newPassword = message.Text;
        db.UpdateUserPassword($"{message.From.Id}", newPassword);
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code><b> Пароль был изменен на {newPassword}✅</b> ",
        parseMode: ParseMode.Html
        );

        status = settings;
        return;
    }

    if (status is game)
    {
        if (message.Text.StartsWith("💂‍♀️ История 👩‍🚀"))
        {
            status = gameHistory;
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Вы выбрали раздел 💂‍♀️ История 👩‍🚀 </b>\n" +
            empty.Awards,
            replyMarkup: Geme_History_start(),
            parseMode: ParseMode.Html
            ) ;
            return;
        }
    }
    if (status is gameHistory)
    {
        questions = db.GetRandomQuestionsFromDb(20);

        // foreach (var item in questions)
        // {
        //     Console.WriteLine(item);
        // }
        db.UpdateQuestions($"{message.From.Id}");
        Console.WriteLine($"Status: {status}");

        foreach (var question in questions)
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Вопрос {question["question"]} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup:Geme_History_Answer
            (question["answer"].ToString(), question["badQuestion1"].ToString(),
            question["badQuestion2"].ToString(), question["badQuestion3"].ToString())
            );
            Console.WriteLine($"ID вопроса {question["id"]}");
            status = gemeAnswer;
            return;
        }
        
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b>Игра окончена! </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Game_menu()
            );
        status = game;
        return;
    }
    if (status is gemeAnswer)
    {
        string answer = message.Text;
        foreach (var question in questions)
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code>" +
            $"<b> Ответ: {answer} </b>\n",
            parseMode: ParseMode.Html,
            replyMarkup: Geme_History_process()
            );
            int indexq = (int)question["id"];
            Console.WriteLine($"ID ответа {indexq}");
            Console.WriteLine($"Вопрос {question["question"]}");

           if(db.CheckAnswer(indexq, answer))
           {
                db.UpdatePoint($"{message.From.Id}");
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b> Правильно ✅ </b>\n",
                parseMode: ParseMode.Html
                );
                status = gameHistory;
                return;
           }
           else
           {
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                $"<b>Неправильно ❌</b>" +
                $"\n<b>Правильный ответ: {question["answer"]} </b>\n",
                parseMode: ParseMode.Html
                );
                status = gameHistory;
                return;
           }
        }
        return;
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
/*
    if (message.Text == "📈 Статистика 📉")
    {
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> Выбери статистику 📊</b>",
            replyMarkup: Statistics_menu(),
            parseMode: ParseMode.Html);
    }
 */
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
    KeyboardButton button_Statistics_Back = "🔙 Назад 🔙";

    //-----------------------------//

    ReplyKeyboardMarkup Statistics_menu = new(new[]
      {
    new KeyboardButton[] { batton_Statistics_History, batton_Statistics_Geography },
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