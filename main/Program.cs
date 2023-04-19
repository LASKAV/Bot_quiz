using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

// Подключаем бота через свой API key
const string Token = "6250535845:AAG4xaJ4ls3J5gjFktEHQgr9ddI0iwTQBqU";
var bot = new TelegramBotClient(Token);

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

// обработка сообщений 
async Task Update(ITelegramBotClient bot,
    Update update, CancellationToken Token)
{
    var message = update.Message;

    Console.WriteLine(
        $"user_id: {message.Chat.Id}" +
        $"\nuser_mess: {message.Text}"
        );
    
    if (message.Text is not null)
    {
        if (message.Text == "/start")
        {
            if (message.Chat.Username is null)
            {
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT: </code>" +
                $"<b>Привет {message.Chat.FirstName} 👋</b>" +
                $"\n\n<b>🎉 Добро пожаловать на викторину 🎉</b>" +
                $"\n\n    <b>🎲ИГРА🎲</b>" +
                $"\n\n<b>Доступно 4 раздела викторины:</b>" +
                $"\n\n<b>1️⃣ 💂‍♀️История👩‍🚀 </b>" +
                $"\n\n<b>2️⃣ 🏛География✈️ </b>" +
                $"\n\n<b>3️⃣ 🔬Биология🦠</b>" +
                $"\n\n<b>4️⃣ 👽Смешанная👀</b>" +
                $"\n\n<b>🏆Награды🏆</b>" +
                $"\n\n<b>🥇 20 - 18 правильных ответов</b>" +
                $"\n\n<b>🥈 17 - 11 правильных ответов</b>" +
                $"\n\n<b>🥉 10 - 1 правильных ответов</b>" +
                $"\n\n    <b>📈Статистика📉</b>" +
                $"\n\n<b>1️⃣ Результаты прошлых викторин </b>" +
                $"\n\n<b>2️⃣ ТОП - 20 по разделам </b>" +
                $"\n\n    <b>⚙️Настройки⚙️</b>" +
                $"\n\n<b>Смена пароля</b>" +
                $"\n\n<b>Смена даты рождения</b>",
                replyMarkup: Top_menu(),
                parseMode: ParseMode.Html
                );
                return;
            }
            else
            {
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT: </code>" +
                $"<b>Привет @{message.Chat.Username} 👋</b>" +
                $"\n\n<b>🎉 Добро пожаловать на викторину 🎉</b>" +
                $"\n\n    <b>🎲ИГРА🎲</b>" +
                $"\n\n<b>Доступно 4 раздела викторины:</b>" +
                $"\n\n<b>1️⃣ 💂‍♀️История👩‍🚀 </b>" +
                $"\n\n<b>2️⃣ 🏛География✈️ </b>" +
                $"\n\n<b>3️⃣ 🔬Биология🦠</b>" +
                $"\n\n<b>4️⃣ 👽Смешанная👀</b>" +
                $"\n\n<b>🏆Награды🏆</b>" +
                $"\n\n<b>🥇 20 - 18 правильных ответов</b>" +
                $"\n\n<b>🥈 17 - 11 правильных ответов</b>" +
                $"\n\n<b>🥉 10 - 1 правильных ответов</b>" +
                $"\n\n    <b>📈Статистика📉</b>" +
                $"\n\n<b>1️⃣ Результаты прошлых викторин </b>" +
                $"\n\n<b>2️⃣ ТОП - 20 по разделам </b>" +
                $"\n\n    <b>⚙️Настройки⚙️</b>" +
                $"\n\n<b>1️⃣ Смена пароля</b>" +
                $"\n\n<b>2️⃣ Смена даты рождения</b>",
                replyMarkup: Top_menu(),
                parseMode: ParseMode.Html
                );
                return;
            }
        }
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

    }
}

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


Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

Console.Read();


