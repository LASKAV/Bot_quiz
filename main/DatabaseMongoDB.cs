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

            //_usersCollection = database.GetCollection<BsonDocument>("geography_questions");
            //_usersCollection = database.GetCollection<BsonDocument>("biology_questions");
            //_usersCollection = database.GetCollection<BsonDocument>("mixed_questions");
        }

        public void Iuser(string UserTgid)
        {
            var user = new BsonDocument {
            { "UserID", UserTgid },
            { "Login",  "" },
            { "Password ", "" },
            { "Age", 0},
            { "Points", 0}
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
            { "Age", bsonDate},
            { "Points", 0}
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
                    {"question", "Кто был первым князем Киевской Руси?"},
                    {"answer", "Рюрик"}
                },
                new BsonDocument
                {
                    {"id", 2},
                    {"question", "Когда произошло крещение Киевской Руси?"},
                    {"answer", "988 год"}
                },
                new BsonDocument
                {
                    {"id", 3},
                    {"question", "Кто был последним королем Запорожской Сечи?"},
                    {"answer", "Петр Конашевич-Сагайдачный"}
                },
                new BsonDocument
                {
                    {"id", 4},
                    {"question", "Кто возглавлял крестьянское восстание 1648 года?"},
                    {"answer", "Богдан Хмельницкий"}
                },
                new BsonDocument
                {
                    {"id", 5},
                    {"question", "Кто был первым президентом независимой Украины?"},
                    {"answer", "Леонид Кравчук"}
                },
                new BsonDocument
                {
                    {"id", 6},
                    {"question", "Какая территория входила в состав Галицко-Волынского княжества?"},
                    {"answer", "Западная Украина"}
                },
                new BsonDocument
                {
                    {"id", 7},
                    {"question", "Кто создал первый украинский государственный флаг?"},
                    {"answer", "Михаил Драгоманов"}
                },
                new BsonDocument
                {
                    {"id", 8},
                    {"question", "Как называлось княжество, созданное на территории современной Украины в XII-XIII веках?"},
                    {"answer", "Галицко-Волынское княжество"}
                },
                new BsonDocument
                {
                    {"id", 9},
                    {"question", "Как называлась первая украинская конституция?"},
                    {"answer", "Пилипа Орлика"}
                },
                new BsonDocument
                {
                    {"id", 10},
                    {"question", "Кто возглавлял украинскую повстанческую армию в 1940-х годах?"},
                    {"answer", "Степан Бандера"}
                },
                new BsonDocument
                {
                    {"id", 11},
                    {"question", "Какое событие стало поводом для начала Оранжевой революции?"},
                    {"answer", "фальсификация результатов президентских выборов 2004 года"}
                },
                new BsonDocument
                {
                    {"id", 12},
                    {"question", "Кто основал Киевскую Русь?"},
                    {"answer", "Варяги"}
                },
                new BsonDocument
                {
                    {"id", 13},
                    {"question", "В каком году произошло разделение Речи Посполитой?"},
                    {"answer", "1772"}
                },
                new BsonDocument
                {
                    {"id", 14},
                    {"question", "Кто был первым козачьим гетманом Левобережной Украины?"},
                    {"answer", "Богдан Хмельницкий"}
                },
                new BsonDocument
                {
                    {"id", 15},
                    {"question", "Кто автор слов гимна Украины?"},
                    {"answer", "Павел Чубинский"}
                },
                new BsonDocument
                {
                    {"id", 16},
                    {"question", "В каком году началась Вторая мировая война на территории Украины?"},
                    {"answer", "1941"}
                },
                new BsonDocument
                {
                    {"id", 17},
                    {"question", "В каком году была провозглашена независимость Украины?"},
                    {"answer", "1991"}
                },
                new BsonDocument
                {
                    {"id", 18},
                    {"question", "Кто был президентом Украины в период 2005-2010 годов?"},
                    {"answer", "Виктор Ющенко"}
                },
                new BsonDocument
                {
                    {"id", 19},
                    {"question", "Кто создал Украинскую повстанческую армию?"},
                    {"answer", "Роман Шухевич"}
                },
                new BsonDocument
                {
                    {"id", 20},
                    {"question", "Какой президент Украины был свергнут в результате Революции достоинства 2014 года?"},
                    {"answer", "Виктор Янукович"}
                }
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


    }
}