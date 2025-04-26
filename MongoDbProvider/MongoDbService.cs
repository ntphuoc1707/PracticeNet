using MongoDB.Driver;
using MongoDbProvider.Entities;

namespace MongoDbProvider
{
    public class MongoDbService
    {
        public static string connectionString = "mongodb+srv://phuocne:Phuoc123@phuocnt.bdbwpzt.mongodb.net/?retryWrites=true&w=majority&appName=phuocnt";
        private readonly IMongoDatabase? _database;

        public IMongoDatabase? Database => _database;
        public MongoDbService()
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);

           // var mongoUrl = MongoUrl.Create(connectionString);
           // var mongoClient = new MongoClient(mongoUrl);
            _database = client.GetDatabase("ChatMessage");
        }
    }
}
