﻿using MongoDB.Driver;
using UpdateConsumer.Models;

namespace UpdateConsumer.Data
{
    public class DataContext
    {
        private readonly IMongoDatabase _database;

        public DataContext()
        {
            var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION") ?? "mongodb://mongo:27017";
            var client = new MongoClient(connectionString);
            _database = client?.GetDatabase("Hackathon")!;
        }

        public IMongoDatabase Database => _database;

        public virtual IMongoCollection<Medico> Medicos => _database.GetCollection<Medico>("tbl_Medicos");
        public virtual IMongoCollection<Paciente> Pacientes => _database.GetCollection<Paciente>("tbl_Pacientes");
        public virtual IMongoCollection<Consulta> Consultas => _database.GetCollection<Consulta>("tbl_Consultas");
    }
}
