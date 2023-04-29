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

//int loginState = 0; // начальное состояние

Status status = Status.defaul;
EmptyStruct empty = new EmptyStruct();
List<main.User> users = new List<main.User>();
var db = new DatabaseMongoDB();


// Создаем новый экземпляр класса User
main.User user = new main.User();

// удаление сообщения  await bot.DeleteMessageAsync(userId,
// sentMessage.MessageId, Token);

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

/*
 //async Task CallbackQueryHandler(ITelegramBotClient bot, CallbackQuery query)
//{
//    string buttonText = query.Data;
//    string name = $"{query.From.FirstName} {query.From.LastName}";
//    Console.WriteLine($"{name} нажал кнопку {buttonText}");
//
//    await bot.AnswerCallbackQueryAsync
//        (query.Id, $"Вы нажали кнопку {buttonText}");
//}
//
 */


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

        // обработка сообщений
        await HandleMesssage(bot, update.Message);
        user.UserTgid = $"{userID}";
       
        return;
    }
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(bot, update.CallbackQuery);
        return;
    }

}
async Task HandleMesssage(ITelegramBotClient bot, Message message)
{
    var user_cek = db.GetUserByUserID(user.UserTgid);

    if (user_cek is null)
    {
        
        if (message.Text == "/start")
        {
            await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT: </code> " +
            $"<b>Привет {message.From.FirstName} 👋</b>" +
            $"\n<b>Для игры вам нужно пройти простую аторизацию</b>",
            replyMarkup: Logger(),
            parseMode: ParseMode.Html);
            return;
        }
    }
    else
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
    if (message.Text.StartsWith("1⃣ Логин"))
    {
        status = login;

        //loginState = 1; // установить состояние на "ожидание логина"
        await bot.SendTextMessageAsync(
         message.Chat.Id,
         $"<code>🤖 BOT:</code><b> Придумай логин: </b> ",
         parseMode: ParseMode.Html
         );
        return;
    }
    if (message.Text.StartsWith("2⃣ Пароль"))
    {
        //loginState = 2;
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
        //loginState = 3;
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

        if (string.IsNullOrEmpty(user.UserTgid) ||
            string.IsNullOrEmpty(user.Login) ||
            string.IsNullOrEmpty(user.Password))
        {
            await bot.SendTextMessageAsync(message.Chat.Id,
                $"<code>🤖 BOT:</code>" +
                "<b> Не все данные были введены, повторите попытку 🚫</b>",
                 parseMode: ParseMode.Html,
                 replyMarkup: Logger());
            return;
        }
        else
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
            users.Add(user);
            db.InsertUser(user.UserTgid, user.Date, user.Login, user.Password);
            status = defaul;
            return;
        }
    }
    // состояние бота 
    if (status is login) 
    {
        string login = message.Text;

        user.Login = login;

        Console.WriteLine($"login = {login}");
        await bot.SendTextMessageAsync(message.Chat.Id,
             $"<code>🤖 BOT: </code> " +
            $"<b>Логин сохранен: {login}✅ </b>",
             replyMarkup: Logger(),
             parseMode: ParseMode.Html);
        //loginState = 0; // вернуться в начальное состояние
        status = defaul;
        return;
    }// пользователь вводит логин
    if (status is password) 
    {
        string password = message.Text;
        user.Password = password;
        Console.WriteLine($"password = {password}");
        await bot.SendTextMessageAsync(message.Chat.Id,
             $"<code>🤖 BOT: </code> " +
            $"<b>Пароль сохранен: {password}✅</b>",
             replyMarkup: Logger(),
             parseMode: ParseMode.Html);
        // loginState = 0; // вернуться в начальное состояние
        status = defaul;
        return;
    }// пользователь вводит пароль
    if (status is date) 
    {
        // преобразовываем строку из даты
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
                replyMarkup: Logger(),
                parseMode: ParseMode.Html);
            user.Date = dateTime;
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
    }// пользователь вводит дату

    await bot.SendTextMessageAsync(message.Chat.Id,
        $"Это HandleMesssage {message.Text}");
    status = defaul;
    return;
}
async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callback)
{
    await bot.SendTextMessageAsync(callback.Message.Chat.Id, $"Нажал {callback.Data}");
    return;
}

/*
    if (message.Text == "🎲 Играть 🎲")
    {
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> Выбери викторину 🔮 </b>",
            replyMarkup: Game_menu(),
            parseMode: ParseMode.Html);
    }
    if (message.Text == "📈 Статистика 📉")
    {
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> Выбери статистику 📊</b>",
            replyMarkup: Statistics_menu(),
            parseMode: ParseMode.Html);
    }
    if (message.Text == "⚙️ Настройки ⚙️")
    {
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> Выбери раздел 🛠 </b>",
            replyMarkup: Settings_menu(),
            parseMode: ParseMode.Html);
    }
    if (message.Text == "🔙 Назад 🔙")
    {
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            $"<code>🤖 BOT:</code><b> назад  🚀 </b>",
            replyMarkup: Top_menu(),
            parseMode: ParseMode.Html);
    }
 */

static IReplyMarkup Top_menu()
{
    //-----------------------------//
    KeyboardButton batton_top_game = "🎲 Играть 🎲";
    KeyboardButton batton_top_stat = "📈 Статистика 📉";
    KeyboardButton batton_top_settings = "⚙️ Настройки ⚙️";
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