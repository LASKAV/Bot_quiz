using System;
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

        public DatabaseMongoDB()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("mydatabase");
            _usersCollection = database.GetCollection<BsonDocument>("users");
            _usersHistory = database.GetCollection<BsonDocument>("users_history");
            _history_questions = database.GetCollection<BsonDocument>("history_questions");
            //HistoryQuestions();
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

    }
}