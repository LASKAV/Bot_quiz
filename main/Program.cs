using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static main.User;



// Подключаем бота через свой API key
const string token = "6250535845:AAG4xaJ4ls3J5gjFktEHQgr9ddI0iwTQBqU";
var bot = new TelegramBotClient(token);

using CancellationTokenSource cts = new();
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};


bot.StartReceiving(
    updateHandler: Update,
    pollingErrorHandler: Error,
    cancellationToken: cts.Token
);

var me = await bot.GetMeAsync();

Console.WriteLine($"Запус бота @{me.Username}");
Console.ReadLine();
cts.Cancel();


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
//        if (message.Text == "🎲 Играть 🎲")
//        {
//            await bot.SendTextMessageAsync(
//                message.Chat.Id,
//                $"<code>🤖 BOT:</code><b> Выбери викторину 🔮 </b>",
//                replyMarkup: GameMenu(),
//                parseMode: ParseMode.Html);
//        }
//        if (message.Text == "📈 Статистика 📉")
//        {
//            await bot.SendTextMessageAsync(
//                message.Chat.Id,
//                $"<code>🤖 BOT:</code><b> Выбери статистику 📊</b>",
//                replyMarkup: StatisticsMenu(),
//                parseMode: ParseMode.Html);
//        }
//        if (message.Text == "⚙️ Настройки ⚙️")
//        {
//            await bot.SendTextMessageAsync(
//                message.Chat.Id,
//                $"<code>🤖 BOT:</code><b> Выбери раздел 🛠 </b>",
//                replyMarkup: SettingsMenu(),
//                parseMode: ParseMode.Html);
//        }
//        if (message.Text == "🔙 Назад 🔙")
//        {
//            await bot.SendTextMessageAsync(
//                message.Chat.Id,
//                $"<code>🤖 BOT:</code><b> назад  🚀 </b>",
//                replyMarkup: TopMenu(),
//                parseMode: ParseMode.Html);
//        }
//}

static IReplyMarkup TopMenu()
{
    //-----------------------------//
    KeyboardButton battonTopGame = "🎲 Играть 🎲";
    KeyboardButton battonTopStat = "📈 Статистика 📉";
    KeyboardButton battonTopSettings = "⚙️ Настройки ⚙️";
    //-----------------------------//

    ReplyKeyboardMarkup topMenu = new(new[]
    {
    new KeyboardButton[] { battonTopGame, battonTopStat },
    new KeyboardButton[] { battonTopSettings },
    }
    )
    {
        ResizeKeyboard = true
    };

    return topMenu;
}
static IReplyMarkup GameMenu()
{
    //-----------------------------//

    KeyboardButton battonGameHistory = "💂‍♀️ История 👩‍🚀";
    KeyboardButton battonGameGeography = "🏛 География ✈️";
    KeyboardButton battonGameBiology = "🔬 Биология 🦠";
    KeyboardButton battonGameMixed = "👽 Смешанная 👀";
    KeyboardButton buttonGameBack = "🔙 Назад 🔙";

    //-----------------------------//

    ReplyKeyboardMarkup gameMenu = new(new[]
    {
    new KeyboardButton[] { battonGameHistory, battonGameGeography },
    new KeyboardButton[] { battonGameBiology, battonGameMixed},
     new KeyboardButton[] { buttonGameBack},
    }
    )
    {
        ResizeKeyboard = true
    };

    return gameMenu;
}
static IReplyMarkup StatisticsMenu()
{
    //-----------------------------//
    KeyboardButton battonStatisticsHistory
        = "🎖 Результаты прошлых викторин 🎖";
    KeyboardButton battonStatisticsGeography
        = "🏆 ТОП - 20 по разделам 🏆";
    KeyboardButton buttonStatisticsBack = "🔙 Назад 🔙";

  //-----------------------------//

  ReplyKeyboardMarkup statisticsMenu = new(new[]
    {
    new KeyboardButton[] { battonStatisticsHistory, battonStatisticsGeography },
     new KeyboardButton[] { buttonStatisticsBack},
    }
    )
    {
        ResizeKeyboard = true
    };

    return statisticsMenu;
}
static IReplyMarkup SettingsMenu()
{
    //-----------------------------//
    KeyboardButton battonSettingsPass
        = "🔧 Смена пароля 🔧";
    KeyboardButton battonSettingsDates
        = "👶 Смена даты рождения 👶";
    KeyboardButton battonSettingsBack = "🔙 Назад 🔙";

    //-----------------------------//

    ReplyKeyboardMarkup settingsMenu = new(new[]
      {
    new KeyboardButton[] { battonSettingsPass, battonSettingsDates },
     new KeyboardButton[] { battonSettingsBack},
    }
      )
    {
        ResizeKeyboard = true
    };

    return settingsMenu;
}


Task Error(
    ITelegramBotClient botClient,
    Exception exception,
    CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}

Console.Read();


