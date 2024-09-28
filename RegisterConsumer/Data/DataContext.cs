using MongoDB.Driver;
using RegisterConsumer.Models;

namespace RegisterConsumer.Data
{
    public class DataContext
    {
        private readonly IMongoDatabase _database;

        public DataContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client?.GetDatabase("TECH_CHALLENGE")!;
        }

        public IMongoDatabase Database => _database;

        public virtual IMongoCollection<Regiao> Regioes => _database.GetCollection<Regiao>("TBL_REGIOES");

        public virtual IMongoCollection<Usuario> Usuarios => _database.GetCollection<Usuario>("TBL_TC_USUARIO");
    }
}
