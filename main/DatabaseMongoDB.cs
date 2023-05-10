﻿using System;
using System.Collections.ObjectModel;
using DnsClient;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using Telegram.Bot.Types;
using ThirdParty.Json.LitJson;

namespace main
{
    public class DatabaseMongoDB
    {
        private readonly IMongoCollection<BsonDocument> _usersCollection;
        private readonly IMongoCollection<BsonDocument> _usersHistory;
        private readonly IMongoCollection<BsonDocument> _history_questions;
        private readonly IMongoCollection<BsonDocument> _geographies_questions;
        private readonly IMongoCollection<BsonDocument> _biology_questions;
        private readonly IMongoCollection<BsonDocument> _mix_questions;

        public DatabaseMongoDB()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("mydatabase");
            _usersCollection = database.GetCollection<BsonDocument>("users");
            _usersHistory = database.GetCollection<BsonDocument>("users_history");
            _history_questions = database.GetCollection<BsonDocument>("history_questions");
            _geographies_questions = database.GetCollection<BsonDocument>("geographies_questions");
            _biology_questions = database.GetCollection<BsonDocument>("biology_questions");
            _mix_questions = database.GetCollection<BsonDocument>("mix_questions");
            // HistoryQuestions();
            // GeographiesQuestions();
            // BiologyQuestions();
            MixQuestions();
        }

        public void Iuser(string UserTgid)
        {
            var user = new BsonDocument {
            { "UserID", UserTgid },
            { "Login",  "" },
            { "Password ", "" },
            { "Age", 0},
            { "Points", 0},
            { "Questions", 0 }
            };

            _usersCollection.InsertOne(user);
        }
        // добавить user
        public void InsertUser(string UserTgid, DateTime
            Date, string Login, string Password)
        {
            BsonDateTime bsonDate = new BsonDateTime(Date);

            var user = new BsonDocument {
            { "UserID", UserTgid },
            { "Login",  Login },
            { "Password", Password },
            { "Age", Date},
            { "Points", 0},
            { "Questions", 0 }
            };

            _usersCollection.InsertOne(user);
            // index для UserID 
            var indexKeysDefinition =
                Builders<BsonDocument>.IndexKeys.Ascending("UserID");
            var indexModel =
                new CreateIndexModel<BsonDocument>(indexKeysDefinition);
            _usersCollection.Indexes.CreateOne(indexModel);

        }
        // проверка userID
        public BsonDocument GetUserByUserID(string userID)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UserID", userID);
            var user = _usersCollection.Find(filter).FirstOrDefault();

            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"UserID: {userID} не найден");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"UserID: {userID} найден");
                Console.ResetColor();
            }

            return user;
        }
        // новый пароль
        public void UpdateUserPassword(string userID, string newPassword)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UserID", userID);
            var update = Builders<BsonDocument>.Update.Set("Password", newPassword);

            var result = _usersCollection.UpdateOne(filter, update);

            if (result.ModifiedCount == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Не удалось обновить пароль для" +
                    $" пользователя с идентификатором UserID {userID}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Обновление пароля для" +
                    $" пользователя с идентификатором UserID {userID}");
                Console.ResetColor();
            }
        }
        // новая дата
        public void UpdateUserDate(string userID, DateTime date)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UserID", userID);
            var update = Builders<BsonDocument>.Update.Set("Age", date);
            var result = _usersCollection.UpdateOne(filter, update);
            if (result.ModifiedCount == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Не удалось обновить дату для" +
                    $" пользователя с идентификатором UserID {userID}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Обновление даты для" +
                    $" пользователя с идентификатором UserID {userID}");
                Console.ResetColor();
            }
        }
        // вопросы для History
        public void HistoryQuestions()
        {
            var questions = new BsonDocument[]
            {
                new BsonDocument
                {
                    {"id", 1},
                    {"question", "Когда произошло крещение Киевской Руси?"},
                    {"answer", "988 год"},
                    {"badQuestion1", "1054 год" },
                    {"badQuestion2", "1037 год" },
                    {"badQuestion3", "1015 год" },
                },
                new BsonDocument
                {
                    {"id", 2},
                    {"question", "Кто был лидером украинской национальной революции 1917-1921 годов?"},
                    {"answer", "Симон Петлюра"},
                    {"badQuestion1", "Степан Бандера" },
                    {"badQuestion2", "Іван Франко" },
                    {"badQuestion3", "Петро Дорошенко" },
                },
                new BsonDocument
                {
                    {"id", 3},
                    {"question", "Какое государство оккупировало Западную Украину в 1939 году?"},
                    {"answer", "СССР"},
                    {"badQuestion1", "Германия" },
                    {"badQuestion2", "Польша" },
                    {"badQuestion3", "Румыния" },
                },
                new BsonDocument
                {
                    {"id", 4},
                    {"question", "Кто стал первым президентом независимой Украины?"},
                    {"answer", "Леонид Кравчук"},
                    {"badQuestion1", "Виктор Ющенко" },
                    {"badQuestion2", "Леонид Кучма" },
                    {"badQuestion3", "Виктор Янукович" },
                },
                new BsonDocument
                {
                    {"id", 5},
                    {"question", "Какой была длительность Отечественной войны в Украине?"},
                    {"answer", "1941-1945"},
                    {"badQuestion1", "1939-1941" },
                    {"badQuestion2", "1945-1948" },
                    {"badQuestion3", "1941-1943" },
                },
                new BsonDocument
                {
                    {"id", 6},
                    {"question", "Какое государство аннексировало Крым в 2014 году?"},
                    {"answer", "Российская Федерация"},
                    {"badQuestion1", "США" },
                    {"badQuestion2", "Украина" },
                    {"badQuestion3", "Казахстан" },
                },
                new BsonDocument
                {
                    {"id", 7},
                    {"question", "Кто был автором гимна Украины?"},
                    {"answer", "Михайло Вербицький"},
                    {"badQuestion1", "Іван Франко"},
                    {"badQuestion2", "Тарас Шевченко"},
                    {"badQuestion3", "Леся Українка"},
                },
                new BsonDocument
                {
                    {"id", 8},
                    {"question", "Когда произошло Хмельницкое восстание?"},
                    {"answer", "1648 год"},
                    {"badQuestion1", "1661 год"},
                    {"badQuestion2", "1709 год"},
                    {"badQuestion3", "1775 год"},
                },
                new BsonDocument
                {
                    {"id", 9},
                    {"question", "Какая династия правила на Руси в IX-XIII веках?"},
                    {"answer", "Рюриковичи"},
                    {"badQuestion1", "Гедиминовичи"},
                    {"badQuestion2", "Ольговичи"},
                    {"badQuestion3", "Юрьевичи"},
                },
                new BsonDocument
                {
                    {"id", 10},
                    {"question", "Кто стал лидером Украинской революции 2014 года?"},
                    {"answer", "Арсений Яценюк"},
                    {"badQuestion1", "Петр Порошенко"},
                    {"badQuestion2", "Виктор Янукович"},
                    {"badQuestion3", "Юлия Тимошенко"},
                },
                new BsonDocument
                {
                    {"id", 11},
                    {"question", "В каком году Украина присоединилась к ООН?"},
                    {"answer", "1945 год"},
                    {"badQuestion1", "1950 год" },
                    {"badQuestion2", "1960 год" },
                    {"badQuestion3", "1935 год" },
                },
                new BsonDocument
                {
                    {"id", 12},
                    {"question", "Когда состоялась битва за Крутные ворота?"},
                    {"answer", "29 января 1918 года"},
                    {"badQuestion1", "15 августа 1917 года" },
                    {"badQuestion2", "2 марта 1919 года" },
                    {"badQuestion3", "7 декабря 1916 года" },
                },
                new BsonDocument
                { 
                    {"id", 13},
                    {"question", "Кто был первым князем государства Киевской Руси?"},
                    {"answer", "Князь Олег"},
                    {"badQuestion1", "Князь Владимир" },
                    {"badQuestion2", "Князь Ярослав" },
                    {"badQuestion3", "Князь Игорь" },
                },
                new BsonDocument
                {
                    {"id", 14},
                    {"question", "Какое событие стало началом Оранжевой революции в Украине?"},
                    {"answer", "Выборы президента 2004 года"},
                    {"badQuestion1", "Подписание Ассоциации с ЕС"},
                    {"badQuestion2", "Отставка премьер-министра Ющенко"},
                    {"badQuestion3", "Отказ Украины присоединиться к СНГ"},
                },
                new BsonDocument
                {
                    {"id", 15},
                    {"question", "В каком году Украина присоединилась к ООН?"},
                    {"answer", "1945 год"},
                    {"badQuestion1", "1991 год" },
                    {"badQuestion2", "1954 год" },
                    {"badQuestion3", "1986 год" },
                },
                new BsonDocument
                {
                    {"id", 16},
                    {"question", "Какое событие в истории Украины произошло в период 1932-1933 годов и стало одним  из       наиболее      трагических в ее истории?"},
                    {"answer", "Голодомор"},
                    {"badQuestion1", "Восстание Крестьянской войны" },
                    {"badQuestion2", "Начало Второй мировой войны" },
                    {"badQuestion3", "Разгром Бату-ханом Восточной Европы" },
                },
                new BsonDocument
                {
                    {"id", 17},
                    {"question", "Какой год стал началом Голодомора на Украине?"},
                    {"answer", "1932"},
                    {"badQuestion1", "1917" },
                    {"badQuestion2", "1945" },
                    {"badQuestion3", "1954" },
                },
                new BsonDocument
                {
                    {"id", 18},
                    {"question", "Кто возглавлял Народную Республику Украина в 1918 году?"},
                    {"answer", "Нестор Махно"},
                    {"badQuestion1", "Петро Скоропадский" },
                    {"badQuestion2", "Владимир Ленин" },
                    {"badQuestion3", "Степан Бандера" },
                },
                new BsonDocument
                {
                    {"id", 19},
                    {"question", "Какая была основная цель восстания Батькивщины в 1990 году?"},
                    {"answer", "Независимость Украины"},
                    {"badQuestion1", "Социальные изменения" },
                    {"badQuestion2", "Экономические реформы" },
                    {"badQuestion3", "Смена правительства" },
                },
                new BsonDocument
                {
                    {"id", 20},
                    {"question", "В каком году произошла Холодная война?"},
                    {"answer", "1947 год"},
                    {"badQuestion1", "1917 год" },
                    {"badQuestion2", "1939 год" },
                    {"badQuestion3", "1956 год" },
                },
            };

            _history_questions.InsertMany(questions);
            var indexKeysDefinition =
               Builders<BsonDocument>.IndexKeys.Ascending("id");
            var indexModel =
                new CreateIndexModel<BsonDocument>(indexKeysDefinition);
            _history_questions.Indexes.CreateOne(indexModel);
        }
        // вопросы 
        public List<BsonDocument> GetRandomQuestionsFromDb(int count)
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            var questions = _history_questions.Find(filter).ToList();

            var random = new Random();
            var selectedQuestions = questions.OrderBy(q => random.Next()).Take(count).ToList();

            var randomCollection = new List<BsonDocument>();
            foreach (var question in selectedQuestions)
            {
                var randomQuestion = new BsonDocument();
                randomQuestion.Add("id", question["id"]);
                randomQuestion.Add("question", question["question"]);
                randomQuestion.Add("answer", question["answer"]);
                randomQuestion.Add("badQuestion1", question["badQuestion1"]);
                randomQuestion.Add("badQuestion2", question["badQuestion2"]);
                randomQuestion.Add("badQuestion3", question["badQuestion3"]);
                randomCollection.Add(randomQuestion);
            }

            return randomCollection;
        }
        // провкрка ответа 
        public bool CheckAnswer(int questionId, string userAnswer)
        {
            // Создаем фильтр для поиска вопроса по id
            var filter = Builders<BsonDocument>.Filter.Eq("id", questionId);

            // Выполняем запрос к базе данных и извлекаем первый результат
            var question = _history_questions.Find(filter).FirstOrDefault();

            // Проверяем, совпадает ли ответ пользователя с правильным ответом в базе данных
            if (question != null && userAnswer.ToLower() == question["answer"].ToString().ToLower())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // добовляем очки вопросов
        public void UpdateQuestions(string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UserID", userId);
            var update = Builders<BsonDocument>.Update.Inc("Questions", 1);
            _usersCollection.UpdateOne(filter, update);
        }
        // добовляем очки за правильный ответ
        public void UpdatePoint(string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UserID", userId);
            var update = Builders<BsonDocument>.Update.Inc("Points", 1);
            _usersCollection.UpdateOne(filter, update);
        }
        // вопросы для Географии
        public void GeographiesQuestions()
        {
            var questions = new BsonDocument[]
            {
                new BsonDocument
                {
                    {"id", 1},
                    {"question", "Какой город является столицей Австралии?"},
                    {"answer", "Канберра"},
                    {"badQuestion1", "Сидней" },
                    {"badQuestion2", "Мельбурн" },
                    {"badQuestion3", "Перт" },
                },
                new BsonDocument
                {
                    {"id", 2},
                    {"question", "Какой океан находится между Северной Америкой и Азией?"},
                    {"answer", "Тихий"},
                    {"badQuestion1", "Индийский" },
                    {"badQuestion2", "Атлантический" },
                    {"badQuestion3", "Северный Ледовитый" },
                },
                new BsonDocument
                {
                    {"id", 3},
                    {"question", "В какой стране находится гора Эверест?"},
                    {"answer", "Непал"},
                    {"badQuestion1", "Китай" },
                    {"badQuestion2", "Индия" },
                    {"badQuestion3", "Пакистан" },
                },
                new BsonDocument
                {
                    {"id", 4},
                    {"question", "Какой город является столицей Нидерландов?"},
                    {"answer", "Амстердам"},
                    {"badQuestion1", "Роттердам" },
                    {"badQuestion2", "Гаага" },
                    {"badQuestion3", "Утрехт" },
                },
                new BsonDocument
                {
                    {"id", 5},
                    {"question", "Какое озеро является самым крупным в мире по площади?"},
                    {"answer", "Каспийское море"},
                    {"badQuestion1", "Озеро Виктория" },
                    {"badQuestion2", "Озеро Гурон" },
                    {"badQuestion3", "Озеро Байкал" },
                },
                new BsonDocument
                {
                    {"id", 6},
                    {"question", "Какое государство занимает территорию между Францией и Испанией?"},
                    {"answer", "Андорра"},
                    {"badQuestion1", "Люксембург" },
                    {"badQuestion2", "Монако" },
                    {"badQuestion3", "Сан-Марино" },
                },
                new BsonDocument
                {
                    {"id", 7},
                    {"question", "Какая река является самой длинной в мире?"},
                    {"answer", "Амазонка"},
                    {"badQuestion1", "Нил" },
                    {"badQuestion2", "Янцзы" },
                    {"badQuestion3", "Миссисипи" },
                },
                new BsonDocument
                {
                    {"id", 8},
                    {"question", "Какая самая высокая гора в мире?"},
                    {"answer", "Эверест"},
                    {"badQuestion1", "К2" },
                    {"badQuestion2", "Макалу" },
                    {"badQuestion3", "Чогори" },
                },
                new BsonDocument
                {
                     {"id", 9},
                     {"question", "Какое самое крупное озеро в мире?"},
                     {"answer", "Каспийское море"},
                     {"badQuestion1", "Байкал" },
                     {"badQuestion2", "Онтарио" },
                     {"badQuestion3", "Супериор" },
                },
                new BsonDocument
                {
                    {"id", 10},
                    {"question", "В какой стране находится Гималаи?"},
                    {"answer", "Непал"},
                    {"badQuestion1", "Китай" },
                    {"badQuestion2", "Индия" },
                    {"badQuestion3", "Бутан" },
                },
                new BsonDocument
                {
                    {"id", 11},
                    {"question", "Какая самая длинная река в мире?"},
                    {"answer", "Амазонка"},
                    {"badQuestion1", "Нил" },
                    {"badQuestion2", "Янцзы" },
                    {"badQuestion3", "Миссисипи" },
                },
                new BsonDocument
                {
                    {"id", 12},
                    {"question", "Какая самая большая по площади страна в мире?"},
                    {"answer", "Россия"},
                    {"badQuestion1", "Канада" },
                    {"badQuestion2", "Китай" },
                    {"badQuestion3", "США" },
                },
                new BsonDocument
                {
                    {"id", 13},
                    {"question", "Какая самая высокая горная система в мире?"},
                    {"answer", "Гималаи"},
                    {"badQuestion1", "Альпы" },
                    {"badQuestion2", "Анды" },
                    {"badQuestion3", "Кавказ" },
                },
                new BsonDocument
                {
                    {"id", 14},
                    {"question", "Какой город является столицей Южной Кореи?"},
                    {"answer", "Сеул"},
                    {"badQuestion1", "Пусан" },
                    {"badQuestion2", "Тэгу" },
                    {"badQuestion3", "Инчхон" },
                },
                new BsonDocument
                {
                    {"id", 15},
                    {"question", "Какое государство занимает большую часть Скандинавского полуострова?"},
                    {"answer", "Норвегия"},
                    {"badQuestion1", "Дания" },
                    {"badQuestion2", "Финляндия" },
                    {"badQuestion3", "Швеция" },
                },
                new BsonDocument
                {
                    {"id", 16},
                    {"question", "В какой стране находится Канберра?"},
                    {"answer", "Австралия"},
                    {"badQuestion1", "Новая Зеландия" },
                    {"badQuestion2", "Индонезия" },
                    {"badQuestion3", "Малайзия" },
                },
                new BsonDocument
                {
                    {"id", 17},
                    {"question", "Какой океан окаймляет южную часть Австралии?"},
                    {"answer", "Индийский океан"},
                    {"badQuestion1", "Тихий океан" },
                    {"badQuestion2", "Атлантический океан" },
                    {"badQuestion3", "Северный Ледовитый океан" },
                },
                new BsonDocument
                {
                    {"id", 18},
                    {"question", "Какое море расположено между Испанией и Марокко?"},
                    {"answer", "Средиземное море"},
                    {"badQuestion1", "Красное море" },
                    {"badQuestion2", "Черное море" },
                    {"badQuestion3", "Карибское море" },
                },
                new BsonDocument
                {
                    {"id", 19},
                    {"question", "В какой стране находится самая большая сахарная пустыня в мире?"},
                    {"answer", "Мали"},
                    {"badQuestion1", "Египет" },
                    {"badQuestion2", "Саудовская Аравия" },
                    {"badQuestion3", "Китай" },
                },
                new BsonDocument
                {
                    {"id", 20},
                    {"question", "Какое озеро является самым глубоким в мире?"},
                    {"answer", "озеро Байкал"},
                    {"badQuestion1", "озеро Виктория" },
                    {"badQuestion2", "озер"},
                }
            };

            _geographies_questions.InsertMany(questions);
            var indexKeysDefinition =
               Builders<BsonDocument>.IndexKeys.Ascending("id");
            var indexModel =
                new CreateIndexModel<BsonDocument>(indexKeysDefinition);
            _geographies_questions.Indexes.CreateOne(indexModel);
        }
        public void BiologyQuestions()
        {
            var questions = new BsonDocument[]
            {
                new BsonDocument
                {
                    {"id", 1},
                    {"question", "Какой вид крови является" +
                    " универсальным донором?" },
                    {"answer", "0(I) Rh-" },
                    {"badQuestion1", "AB(IV) Rh-" },
                    {"badQuestion2", "A(II) Rh+" },
                    {"badQuestion3", "B(III) Rh+" },
                },
                new BsonDocument
                {
                    {"id", 2},
                    {"question", "Какая часть растения отвечает" +
                    " за поглощение воды и минеральных веществ?" },
                    {"answer", "корневая система" },
                    {"badQuestion1", "листовая пластинка" },
                    {"badQuestion2", "стебель" },
                    {"badQuestion3", "цветок" },
                },
                new BsonDocument
                {
                    {"id", 3},
                    {"question", "Как называется процесс, при котором" +
                    " зеленые растения превращают" +
                    " углекислый газ и воду в глюкозу и кислород?" },
                    {"answer", "фотосинтез" },
                    {"badQuestion1", "фототаксис" },
                    {"badQuestion2", "хемосинтез" },
                    {"badQuestion3", "респирация" },
                },
                new BsonDocument
                {
                    {"id", 4},
                    {"question", "Как называется наследственный материал в клетке?" },
                    {"answer", "ДНК" },
                    {"badQuestion1", "РНК" },
                    {"badQuestion2", "белок" },
                    {"badQuestion3", "углеводы" },
                },
                new BsonDocument
                {
                    {"id", 5},
                    {"question", "Какой орган человеческого тела отвечает за выработку инсулина?"},
                    {"answer", "Поджелудочная железа"},
                    {"badQuestion1", "Печень" },
                    {"badQuestion2", "Селезенка" },
                    {"badQuestion3", "Желудок" },
                },
                new BsonDocument
                {
                    {"id", 6},
                    {"question", "Что такое ДНК?"},
                    {"answer", "молекула наследственности"},
                    {"badQuestion1", "молекула белка" },
                    {"badQuestion2", "молекула воды" },
                    {"badQuestion3", "молекула углекислого газа" },
                },
                new BsonDocument
                {
                    {"id", 7},
                    {"question", "Какой тип клеток не имеет ядра?"},
                    {"answer", "эритроциты"},
                    {"badQuestion1", "лейкоциты" },
                    {"badQuestion2", "тромбоциты" },
                    {"badQuestion3", "нейроны" },
                },
                new BsonDocument
                {
                    {"id", 8},
                    {"question", "Что такое фотосинтез?"},
                    {"answer", "процесс преобразования световой энергии в химическую энергию"},
                    {"badQuestion1", "процесс выделения углекислого газа" },
                    {"badQuestion2", "процесс образования воды" },
                    {"badQuestion3", "процесс окисления глюкозы" },
                },
                new BsonDocument
                {
                    {"id", 9},
                    {"question", "Что такое митоз?"},
                    {"answer", "процесс деления клеток, в результате которого образуются" +
                    " две клетки-дочери с одинаковым набором хромосом"},
                    {"badQuestion1", "процесс образования спермы" },
                    {"badQuestion2", "процесс образования яйцеклетки" },
                    {"badQuestion3", "процесс образования крови" },
                },
                new BsonDocument
                {
                    {"id", 10},
                    {"question", "Какое название носит научная дисциплина," +
                    " изучающая живых организмов и их взаимодействие с окружающей средой?"},
                    {"answer", "экология"},
                    {"badQuestion1", "анатомия" },
                    {"badQuestion2", "физиология" },
                    {"badQuestion3", "генетика" },
                },
                new BsonDocument
                {
                    {"id", 11},
                    {"question", "Какая кровь светится в темноте?"},
                    {"answer", "У гренландской акулы"},
                    {"badQuestion1", "У морских звезд" },
                    {"badQuestion2", "У кальмаров" },
                    {"badQuestion3", "У морских котиков" },
                },
                new BsonDocument
                {
                    {"id", 12},
                    {"question", "Какой орган у геккона выполняет функцию дыхания?"},
                    {"answer", "Ротовая полость"},
                    {"badQuestion1", "Легкие" },
                    {"badQuestion2", "Жабры" },
                    {"badQuestion3", "Трахеи" },
                },
                new BsonDocument
                {
                    {"id", 13},
                    {"question", "Как называется процесс," +
                    " при котором растения превращают световую" +
                    " энергию в химическую?"},
                    {"answer", "Фотосинтез"},
                    {"badQuestion1", "Дыхание" },
                    {"badQuestion2", "Брожение" },
                    {"badQuestion3", "Окисление" },
                },
                new BsonDocument
                {
                    {"id", 14},
                    {"question", "Какой процесс обеспечивает движение" +
                    " воды из корней растения в его стебель и листья?"},
                    {"answer", "Капиллярное действие"},
                    {"badQuestion1", "Капиллярность" },
                    {"badQuestion2", "Коагуляция" },
                    {"badQuestion3", "Диффузия" },
                },
                new BsonDocument
                {
                    {"id", 15},
                    {"question", "Какой вид клеток в организме" +
                    " человека является ответственным за производство инсулина?"},
                    {"answer", "Бета-клетки поджелудочной железы"},
                    {"badQuestion1", "Красные кровяные тельца" },
                    {"badQuestion2", "Нейроны" },
                    {"badQuestion3", "Хрящевые клетки" },
                },
                new BsonDocument
                {
                    {"id", 16},
                    {"question", "Что происходит с длиной волны зеленого" +
                    " света, когда он попадает на лист растения?"},
                    {"answer", "Он отражается"},
                    {"badQuestion1", "Он поглощается" },
                    {"badQuestion2", "Он искривляется" },
                    {"badQuestion3", "Он растягивается" },
                },
                new BsonDocument
                {
                    {"id", 17},
                    {"question", "Какие две основные типы клеток существуют на Земле?"},
                    {"answer", "прокариоты и эукариоты"},
                    {"badQuestion1", "живые и мертвые" },
                    {"badQuestion2", "животные и растения" },
                    {"badQuestion3", "одноклеточные и многоклеточные" },
                },
                new BsonDocument
                {
                    {"id", 18},
                    {"question", "Как называется наука, изучающая бактерии?"},
                    {"answer", "бактериология"},
                    {"badQuestion1", "зоология" },
                    {"badQuestion2", "вирусология" },
                    {"badQuestion3", "микология" },
                },
                new BsonDocument
                {
                    {"id", 19},
                    {"question", "Какой орган человеческого тела является самым большим?"},
                    {"answer", "кожа"},
                    {"badQuestion1", "мозг" },
                    {"badQuestion2", "сердце" },
                    {"badQuestion3", "печень" },
                },
                new BsonDocument
                {
                    {"id", 20},
                    {"question", "Какое вещество отвечает за передачу нервных импульсов в организме?"},
                    {"answer", "нейромедиаторы"},
                    {"badQuestion1", "гормоны" },
                    {"badQuestion2", "ферменты" },
                    {"badQuestion3", "антибиотики" },
                },
                /*
                 new BsonDocument
                {
                {"id", 25},
                {"question", "Какой вид ткани отвечает за передачу кислорода и питательных веществ в организме?"},
                {"answer", "кровь"},
                {"badQuestion1", "мышечная" },
                {"badQuestion2", "нервная" },
                {"badQuestion3", "жировая" },
                },
                new BsonDocument
                {
                {"id", 26},
                {"question", "Какой процесс отвечает за производство энергии в клетках?"},
                {"answer", "клеточное дыхание"},
                {"badQuestion1", "фотосинтез" },
                {"badQuestion2", "митоз" },
                {"badQuestion3", "мейоз" },
                },
                 */
       
            };

            _biology_questions.InsertMany(questions);
            var indexKeysDefinition =
               Builders<BsonDocument>.IndexKeys.Ascending("id");
            var indexModel =
                new CreateIndexModel<BsonDocument>(indexKeysDefinition);
            _biology_questions.Indexes.CreateOne(indexModel);
        }
        public void MixQuestions()
        {
            var questions = new BsonDocument[]
           {
                new BsonDocument
                {
                    {"id", 1},
                    {"question", "Какой город является столицей Австралии?"},
                    {"answer", "Канберра"},
                    {"badQuestion1", "Сидней" },
                    {"badQuestion2", "Мельбурн" },
                    {"badQuestion3", "Перт" },
                },
                new BsonDocument
                {
                    {"id", 2},
                    {"question", "Какой океан находится между Северной Америкой и Азией?"},
                    {"answer", "Тихий"},
                    {"badQuestion1", "Индийский" },
                    {"badQuestion2", "Атлантический" },
                    {"badQuestion3", "Северный Ледовитый" },
                },
                new BsonDocument
                {
                    {"id", 3},
                    {"question", "В какой стране находится гора Эверест?"},
                    {"answer", "Непал"},
                    {"badQuestion1", "Китай" },
                    {"badQuestion2", "Индия" },
                    {"badQuestion3", "Пакистан" },
                },
                new BsonDocument
                {
                    {"id", 4},
                    {"question", "Какой город является столицей Нидерландов?"},
                    {"answer", "Амстердам"},
                    {"badQuestion1", "Роттердам" },
                    {"badQuestion2", "Гаага" },
                    {"badQuestion3", "Утрехт" },
                },
                new BsonDocument
                {
                    {"id", 5},
                    {"question", "Какое озеро является самым крупным в мире по площади?"},
                    {"answer", "Каспийское море"},
                    {"badQuestion1", "Озеро Виктория" },
                    {"badQuestion2", "Озеро Гурон" },
                    {"badQuestion3", "Озеро Байкал" },
                },
                new BsonDocument
                {
                    {"id", 6},
                    {"question", "Какое государство занимает территорию между Францией и Испанией?"},
                    {"answer", "Андорра"},
                    {"badQuestion1", "Люксембург" },
                    {"badQuestion2", "Монако" },
                    {"badQuestion3", "Сан-Марино" },
                },
                new BsonDocument
                {
                    {"id", 7},
                    {"question", "Какая река является самой длинной в мире?"},
                    {"answer", "Амазонка"},
                    {"badQuestion1", "Нил" },
                    {"badQuestion2", "Янцзы" },
                    {"badQuestion3", "Миссисипи" },
                },
                new BsonDocument
                {
                    {"id", 8},
                    {"question", "Как называется процесс," +
                    " при котором растения превращают световую" +
                    " энергию в химическую?"},
                    {"answer", "Фотосинтез"},
                    {"badQuestion1", "Дыхание" },
                    {"badQuestion2", "Брожение" },
                    {"badQuestion3", "Окисление" },
                },
                new BsonDocument
                {
                    {"id", 9},
                    {"question", "Какой процесс обеспечивает движение" +
                    " воды из корней растения в его стебель и листья?"},
                    {"answer", "Капиллярное действие"},
                    {"badQuestion1", "Капиллярность" },
                    {"badQuestion2", "Коагуляция" },
                    {"badQuestion3", "Диффузия" },
                },
                new BsonDocument
                {
                    {"id", 10},
                    {"question", "Какой вид клеток в организме" +
                    " человека является ответственным за производство инсулина?"},
                    {"answer", "Бета-клетки поджелудочной железы"},
                    {"badQuestion1", "Красные кровяные тельца" },
                    {"badQuestion2", "Нейроны" },
                    {"badQuestion3", "Хрящевые клетки" },
                },
                new BsonDocument
                {
                    {"id", 11},
                    {"question", "Что происходит с длиной волны зеленого" +
                    " света, когда он попадает на лист растения?"},
                    {"answer", "Он отражается"},
                    {"badQuestion1", "Он поглощается" },
                    {"badQuestion2", "Он искривляется" },
                    {"badQuestion3", "Он растягивается" },
                },
                new BsonDocument
                {
                    {"id", 12},
                    {"question", "Какие две основные типы клеток существуют на Земле?"},
                    {"answer", "прокариоты и эукариоты"},
                    {"badQuestion1", "живые и мертвые" },
                    {"badQuestion2", "животные и растения" },
                    {"badQuestion3", "одноклеточные и многоклеточные" },
                },
                new BsonDocument
                {
                    {"id", 13},
                    {"question", "Как называется наука, изучающая бактерии?"},
                    {"answer", "бактериология"},
                    {"badQuestion1", "зоология" },
                    {"badQuestion2", "вирусология" },
                    {"badQuestion3", "микология" },
                },
                new BsonDocument
                {
                    {"id", 14},
                    {"question", "Какой орган человеческого тела является самым большим?"},
                    {"answer", "кожа"},
                    {"badQuestion1", "мозг" },
                    {"badQuestion2", "сердце" },
                    {"badQuestion3", "печень" },
                },
                new BsonDocument
                {
                    {"id", 15},
                    {"question", "Какое вещество отвечает за передачу нервных импульсов в организме?"},
                    {"answer", "нейромедиаторы"},
                    {"badQuestion1", "гормоны" },
                    {"badQuestion2", "ферменты" },
                    {"badQuestion3", "антибиотики" },
                },
                new BsonDocument
                {
                    {"id", 16},
                    {"question", "Кто стал первым президентом независимой Украины?"},
                    {"answer", "Леонид Кравчук"},
                    {"badQuestion1", "Виктор Ющенко" },
                    {"badQuestion2", "Леонид Кучма" },
                    {"badQuestion3", "Виктор Янукович" },
                },
                new BsonDocument
                {
                    {"id", 17},
                    {"question", "Какой была длительность Отечественной войны в Украине?"},
                    {"answer", "1941-1945"},
                    {"badQuestion1", "1939-1941" },
                    {"badQuestion2", "1945-1948" },
                    {"badQuestion3", "1941-1943" },
                },
                new BsonDocument
                {
                    {"id", 18},
                    {"question", "Какое государство аннексировало Крым в 2014 году?"},
                    {"answer", "Российская Федерация"},
                    {"badQuestion1", "США" },
                    {"badQuestion2", "Украина" },
                    {"badQuestion3", "Казахстан" },
                },
                new BsonDocument
                {
                    {"id", 19},
                    {"question", "Кто был автором гимна Украины?"},
                    {"answer", "Михайло Вербицький"},
                    {"badQuestion1", "Іван Франко"},
                    {"badQuestion2", "Тарас Шевченко"},
                    {"badQuestion3", "Леся Українка"},
                },
                new BsonDocument
                {
                    {"id", 20},
                    {"question", "Когда произошло Хмельницкое восстание?"},
                    {"answer", "1648 год"},
                    {"badQuestion1", "1661 год"},
                    {"badQuestion2", "1709 год"},
                    {"badQuestion3", "1775 год"},
                },
                new BsonDocument
                {
                    {"id", 21},
                    {"question", "Какая династия правила на Руси в IX-XIII веках?"},
                    {"answer", "Рюриковичи"},
                    {"badQuestion1", "Гедиминовичи"},
                    {"badQuestion2", "Ольговичи"},
                    {"badQuestion3", "Юрьевичи"},
                },
                new BsonDocument
                {
                    {"id", 22},
                    {"question", "Кто стал лидером Украинской революции 2014 года?"},
                    {"answer", "Арсений Яценюк"},
                    {"badQuestion1", "Петр Порошенко"},
                    {"badQuestion2", "Виктор Янукович"},
                    {"badQuestion3", "Юлия Тимошенко"},
                },

             };

            _mix_questions.InsertMany(questions);
            var indexKeysDefinition =
               Builders<BsonDocument>.IndexKeys.Ascending("id");
            var indexModel =
                new CreateIndexModel<BsonDocument>(indexKeysDefinition);
            _mix_questions.Indexes.CreateOne(indexModel);

        }
    }
}