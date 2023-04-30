using System;
using DnsClient;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using ThirdParty.Json.LitJson;

namespace main
{

    public class DatabaseMongoDB
    {
        private readonly IMongoCollection<BsonDocument> _usersCollection;

        public DatabaseMongoDB()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("mydatabase");
            _usersCollection = database.GetCollection<BsonDocument>("users");
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
    }

}

