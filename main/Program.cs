using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

async Task Update(ITelegramBotClient bot,
    Update update, CancellationToken Token)
{
    var message = update.Message;
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
                $"\n\n    <b>===</b><code>🎲ИГРА🎲</code><b>===</b>" +
                $"\n\n<b>Доступно 3 раздела викторины:</b>" +
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
                parseMode: ParseMode.Html
                );
                return;
            }
            else
            {
                // sdgs
                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"<code>🤖 BOT: </code>" +
                $"<b>Привет @{message.Chat.Username} 👋</b>" +
                $"\n\n<b>🎉 Добро пожаловать на викторину 🎉</b>" +
                $"\n\n    <b>===</b><code>🎲ИГРА🎲</code><b>===</b>" +
                $"\n\n<b>Доступно 3 раздела викторины:</b>" +
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
                parseMode: ParseMode.Html
                );
                return;
            }

        }
        if (message.Text == "photo")
        {

        }

    }
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


