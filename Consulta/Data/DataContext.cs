using MongoDB.Driver;
using Consulta.Models;

namespace Consulta.Data
{
    public class DataContext
    {
        private readonly IMongoDatabase _database;

        public DataContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client?.GetDatabase("Hackathon")!;
        }

        public IMongoDatabase Database => _database;

        public virtual IMongoCollection<Medico> Medicos => _database.GetCollection<Medico>("tbl_Medicos");
        public virtual IMongoCollection<Paciente> Pacientes => _database.GetCollection<Paciente>("tbl_Pacientes");
        public virtual IMongoCollection<Models.Consulta> Consultas => _database.GetCollection <Models.Consulta>("tbl_Consultas");
    }
}
