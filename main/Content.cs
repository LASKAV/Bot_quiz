﻿using System;
using main;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace main
{
    public struct EmptyStruct
    {
       public string MainOffice = $"\n\n<b>🎉 Добро пожаловать на викторину 🎉</b>" +
            $"\n\n    <b>🎲ИГРА🎲</b>" +
            $"\n\n<b>Доступно 4 раздела викторины:</b>" +
            $"\n\n<b>1️⃣ 💂‍♀️История👩‍🚀 </b>" +
            $"\n\n<b>2️⃣ 🏛География✈️ </b>" +
            $"\n\n<b>3️⃣ 🔬Биология🦠</b>" +
            $"\n\n<b>4️⃣ 👽Смешанная👀</b>" +
            $"\n\n<b>🏆Награды🏆</b>" +
            $"\n\n<b>🥇 18 - 20 правильных ответов</b>" +
            $"\n\n<b>🥈 11 - 17 правильных ответов</b>" +
            $"\n\n<b>🥉 1  - 10 правильных ответов</b>" +
            $"\n\n    <b>📈Статистика📉</b>" +
            $"\n\n<b>1️⃣ Результаты прошлых викторин </b>" +
            $"\n\n<b>2️⃣ ТОП - 20 по разделам </b>" +
            $"\n\n    <b>⚙️Настройки⚙️</b>" +
            $"\n\n<b>Смена пароля</b>" +
            $"\n\n<b>Смена даты рождения</b>";

        public string Awards = $"\n<b>🏆Награды🏆</b>" +
            $"\n\n<b>🥇 18 - 20  правильных ответов</b>" +
            $"\n\n<b>🥈 11 - 17  правильных ответов</b>" +
            $"\n\n<b>🥉 1  - 10  правильных ответов</b>";



        public EmptyStruct()
        {
        }
    }
}
