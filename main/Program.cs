using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static main.User;
using static main.Status;
using main;
using System.Threading;
using Telegram.Bot.Requests;


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


int loginState = 0; // начальное состояние


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
    if (message.Text.StartsWith("login"))
    {
        loginState = 1; // установить состояние на "ожидание логина"
        await bot.SendTextMessageAsync(
         message.Chat.Id,
         $"<code>🤖 BOT:</code><b> Придумай логин: </b> ",
         parseMode: ParseMode.Html
         );
        return;
    }
    if (message.Text.StartsWith("password"))
    {
        loginState = 2;
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code><b> Придумай пароль: </b> ",
        parseMode: ParseMode.Html
        );
        return;
    }
    if (message.Text.StartsWith("date"))
    {
        loginState = 3;
        await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code><b> Введите дату рождения: </b> ",
        parseMode: ParseMode.Html
        );
        return;
    }

    // состояние бота 
    if (loginState == 1) // пользователь вводит логин
    {
        string login = message.Text;
        Console.WriteLine($"login = {login}");
        await bot.SendTextMessageAsync(message.Chat.Id, $"Логин сохранен: {login}");
        loginState = 0; // вернуться в начальное состояние
        return;
    }
    if (loginState == 2) // пользователь вводит пароль
    {
        string password = message.Text;
        Console.WriteLine($"password = {password}");
        await bot.SendTextMessageAsync(message.Chat.Id, $"Пароль сохранен: {password}");
        loginState = 0; // вернуться в начальное состояние
        return;
    }
    if (loginState == 3) // пользователь вводит дату
    {
        string date = message.Text;
        Console.WriteLine($"date = {date}");
        await bot.SendTextMessageAsync(message.Chat.Id, $"Дата сохранена: {date}");
        loginState = 0; // вернуться в начальное состояние
        return;
    }
    
        await bot.SendTextMessageAsync(message.Chat.Id, $"Это HandleMesssage {message.Text}");
    return;
}
async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callback)
{
    await bot.SendTextMessageAsync(callback.Message.Chat.Id,$"Нажал {callback.Data}");
    return;
}









async Task HandleStartCommand(ITelegramBotClient bot, long userId, string firstName)
{
    // Отправляем сообщение приветствия
    Message sentMessage = await bot.SendTextMessageAsync(
        userId,
        $"<code>🤖 BOT: </code> " +
        $"<b>Привет {firstName} 👋</b>" +
        $"\n<b>Для игры вам нужно пройти простую аторизацию</b>",
        replyMarkup: Logger(),
        parseMode: ParseMode.Html);

}
async Task HandleLogin(ITelegramBotClient bot, Update update, long userId)
{
    Message sentMessage =  await bot.SendTextMessageAsync(
         userId,
         $"<code>🤖 BOT:</code><b> Придумай логин: </b> ",
         parseMode: ParseMode.Html
         );

    // удаление сообщения  await bot.DeleteMessageAsync(userId, sentMessage.MessageId, Token);
}
async Task GetUpdates(ITelegramBotClient bot)
{
    int offset = 0;
    while (true)
    {
        var updates = await bot.GetUpdatesAsync
            (offset: offset, allowedUpdates: new[] { UpdateType.Message });
        foreach (var update in updates)
        {
            // обработка полученного сообщения
            if (update.Message != null)
            {
                Console.WriteLine($"Получил текстовое сообщение от" +
                    $" {update.Message.Chat.Id}: {update.Message.Text}");
            }

            // увеличиваем offset, чтобы не получать те же сообщения снова
            offset = update.Id + 1;
        }
    }
}
/*
 // if (message.Text == "/start")
// {
//     Console.WriteLine($"message.Text =  {message.Text}");
// 
//     await bot.SendTextMessageAsync(
//         message.Chat.Id,
//         $"<code>🤖 BOT: </code> " +
//         $"<b>Привет {message.Chat.FirstName} 👋</b>" +
//         $"\n<b>Для игры вам нужно пройти простую аторизацию</b>",
//         replyMarkup: Logger(),
//         parseMode: ParseMode.Html
//         );
// }
// if (message.Text == "login")
// {
//     await bot.SendTextMessageAsync(
//          message.Chat.Id,
//          $"<code>🤖 BOT:</code><b> Придумай логин: </b> ",
//          parseMode: ParseMode.Html
//          );
//     login = message.Text;
//     Console.WriteLine($"login: {login}");
// }
// // if (message.Text == "password")
//     {
//         await bot.SendTextMessageAsync(
//             message.Chat.Id,
//             $"<code>🤖 BOT:</code><b> Придумай пароль: </b> ",
//             parseMode: ParseMode.Html
//             );
//     password = message.Text;
//     Console.WriteLine($"login: {password}");
// }


// if (message.Text == "date")
// {
//     await bot.SendTextMessageAsync(
//         message.Chat.Id,
//         $"<code>🤖 BOT:</code><b> Введи дату рождения: </b> ",
//         parseMode: ParseMode.Html
//         );
//    
// }
// else
// {
//     string day = null;
//     day = message.Text;
//     month = 11;
//     year = 2001;
// 
//     Console.WriteLine($"date: {new DateTime(year, month, day)}");
// }

 */
async Task Login(ITelegramBotClient bot, Update update, CancellationToken Token)
{
    string login = null;
    var user = new main.User();

    var message = update.Message;


    login = message.Text;
    Console.WriteLine($"login = {login}");


    user.Login = login;
    user.Show_user();

    Password(bot, update, Token);
}
async Task Password
    (ITelegramBotClient bot, Update update, CancellationToken Token)
{
    var user = new main.User();
    string password = null;

    var message = update.Message;

    await bot.SendTextMessageAsync(
        message.Chat.Id,
        $"<code>🤖 BOT:</code><b> Придумай пароль: </b> ",
        parseMode: ParseMode.Html
        );

    password = message.Text;
    Console.WriteLine($"password =  {password}");

    user.Password = password;
    user.Show_user();
}
/*
 *  var message = update.Message;

    Console.WriteLine(
        $"user_id: {message.Chat.Id}" +
        $"\nuser_mess: {message.Text}"
        );

    int day = 21;
    int month = 11;
    int year = 2001;
    string login = null;
    string password = null;
    
    if (message.Text is not null)
    {
        if (message.Text == "/start")
        {
            Console.WriteLine($"message.Text =  {message.Text}");

            await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT: </code> " +
                $"<b>Привет {message.Chat.FirstName} 👋</b>" +
                $"\n<b>Для игры вам нужно пройти простую аторизацию</b>",
                parseMode: ParseMode.Html
                );
            await bot.SendTextMessageAsync(
                 message.Chat.Id,
                 $"<code>🤖 BOT:</code><b> Придумай логин: </b> ",
                 parseMode: ParseMode.Html
                 );
        }
        else
        {
            Console.WriteLine($"message.Text =  {message.Text}");

            login = message.Text;


         //   await bot.SendTextMessageAsync(
         //       message.Chat.Id,
         //       $"<code>🤖 BOT:</code><b> Придумай пароль: </b> ",
         //       parseMode: ParseMode.Html
         //       );

            if (login is not null)
            {
                Console.WriteLine($"message.Text =  {message.Text}");

                password = message.Text;

                var user = new main.User
                (message.Chat.Id.ToString(),
                new DateTime(year, month, day),
                login,
                "523asd");

                user.Show_user();

            }
        }
    }

    //if (message.Chat.Username is null)
    //{
    //    await bot.SendTextMessageAsync(
    //    message.Chat.Id,
    //    $"\n\n<b>🎉 Добро пожаловать на викторину 🎉</b>" +
    //    $"\n\n    <b>🎲ИГРА🎲</b>" +
    //    $"\n\n<b>Доступно 4 раздела викторины:</b>" +
    //    $"\n\n<b>1️⃣ 💂‍♀️История👩‍🚀 </b>" +
    //    $"\n\n<b>2️⃣ 🏛География✈️ </b>" +
    //    $"\n\n<b>3️⃣ 🔬Биология🦠</b>" +
    //    $"\n\n<b>4️⃣ 👽Смешанная👀</b>" +
    //    $"\n\n<b>🏆Награды🏆</b>" +
    //    $"\n\n<b>🥇 20 - 18 правильных ответов</b>" +
    //    $"\n\n<b>🥈 17 - 11 правильных ответов</b>" +
    //    $"\n\n<b>🥉 10 - 1 правильных ответов</b>" +
    //    $"\n\n    <b>📈Статистика📉</b>" +
    //    $"\n\n<b>1️⃣ Результаты прошлых викторин </b>" +
    //    $"\n\n<b>2️⃣ ТОП - 20 по разделам </b>" +
    //    $"\n\n    <b>⚙️Настройки⚙️</b>" +
    //    $"\n\n<b>Смена пароля</b>" +
    //    $"\n\n<b>Смена даты рождения</b>",
    //    replyMarkup: Top_menu(),
    //    parseMode: ParseMode.Html
    //    );
    //    return;
    //}
    //else
    //{
    //    await bot.SendTextMessageAsync(
    //    message.Chat.Id,
    //    $"\n\n<b>🎉 Добро пожаловать на викторину 🎉</b>" +
    //    $"\n\n    <b>🎲ИГРА🎲</b>" +
    //    $"\n\n<b>Доступно 4 раздела викторины:</b>" +
    //    $"\n\n<b>1️⃣ 💂‍♀️История👩‍🚀 </b>" +
    //    $"\n\n<b>2️⃣ 🏛География✈️ </b>" +
    //    $"\n\n<b>3️⃣ 🔬Биология🦠</b>" +
    //    $"\n\n<b>4️⃣ 👽Смешанная👀</b>" +
    //    $"\n\n<b>🏆Награды🏆</b>" +
    //    $"\n\n<b>🥇 20 - 18 правильных ответов</b>" +
    //    $"\n\n<b>🥈 17 - 11 правильных ответов</b>" +
    //    $"\n\n<b>🥉 10 - 1 правильных ответов</b>" +
    //    $"\n\n    <b>📈Статистика📉</b>" +
    //    $"\n\n<b>1️⃣ Результаты прошлых викторин </b>" +
    //    $"\n\n<b>2️⃣ ТОП - 20 по разделам </b>" +
    //    $"\n\n    <b>⚙️Настройки⚙️</b>" +
    //    $"\n\n<b>1️⃣ Смена пароля</b>" +
    //    $"\n\n<b>2️⃣ Смена даты рождения</b>",
    //    replyMarkup: Top_menu(),
    //    parseMode: ParseMode.Html
    //    );
    //    return;
    //}
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
        = "login";
    KeyboardButton batton_Logger_password
        = "password";
    KeyboardButton batton_Logger_Date
        = "date";

    //-----------------------------//

    ReplyKeyboardMarkup Logger_menu = new
        (new[]
    {
        new KeyboardButton[] { batton_Logger_login, batton_Logger_password },
        new KeyboardButton[] { batton_Logger_Date},
    }
      )
    {
        ResizeKeyboard = true
    };

    return Logger_menu;
}

Task Error(ITelegramBotClient botClient,Exception exception,
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