using MongoDB.Driver;
using DeleteConsumer.Models;

namespace DeleteConsumer.Data
{
    public class MongoDBMigrations
    {
        private readonly IMongoDatabase _database;

        public MongoDBMigrations(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task SeedDataAsync()
        {
            var regioesCollection = _database.GetCollection<Regiao>("TBL_REGIOES");
            if (await regioesCollection.CountDocumentsAsync(_ => true) == 0)
            {
                var regioes = new List<Regiao>
                {
                    new Regiao { IdRegiao = 1, DDDRegiao = "11", SiglaRegiao = "SP", NomeRegiao = "São Paulo" },
                    new Regiao { IdRegiao = 2, DDDRegiao = "21", SiglaRegiao = "RJ", NomeRegiao = "Rio de Janeiro" },
                    new Regiao { IdRegiao = 3, DDDRegiao = "31", SiglaRegiao = "MG", NomeRegiao = "Minas Gerais" },
                    new Regiao { IdRegiao = 4, DDDRegiao = "41", SiglaRegiao = "PR", NomeRegiao = "Paraná" },
                    new Regiao { IdRegiao = 5, DDDRegiao = "51", SiglaRegiao = "RS", NomeRegiao = "Rio Grande do Sul" },
                    new Regiao { IdRegiao = 6, DDDRegiao = "61", SiglaRegiao = "DF", NomeRegiao = "Distrito Federal" },
                    new Regiao { IdRegiao = 7, DDDRegiao = "71", SiglaRegiao = "BA", NomeRegiao = "Bahia" },
                    new Regiao { IdRegiao = 8, DDDRegiao = "81", SiglaRegiao = "PE", NomeRegiao = "Pernambuco" },
                    new Regiao { IdRegiao = 9, DDDRegiao = "91", SiglaRegiao = "PA", NomeRegiao = "Pará" },
                    new Regiao { IdRegiao = 10, DDDRegiao = "92", SiglaRegiao = "AM", NomeRegiao = "Amazonas" },
                    new Regiao { IdRegiao = 11, DDDRegiao = "82", SiglaRegiao = "AL", NomeRegiao = "Alagoas" },
                    new Regiao { IdRegiao = 12, DDDRegiao = "83", SiglaRegiao = "PB", NomeRegiao = "Paraíba" },
                    new Regiao { IdRegiao = 13, DDDRegiao = "84", SiglaRegiao = "RN", NomeRegiao = "Rio Grande do Norte" },
                    new Regiao { IdRegiao = 14, DDDRegiao = "85", SiglaRegiao = "CE", NomeRegiao = "Ceará" },
                    new Regiao { IdRegiao = 15, DDDRegiao = "86", SiglaRegiao = "PI", NomeRegiao = "Piauí" },
                    new Regiao { IdRegiao = 16, DDDRegiao = "87", SiglaRegiao = "PE", NomeRegiao = "Pernambuco (Interior)" },
                    new Regiao { IdRegiao = 17, DDDRegiao = "88", SiglaRegiao = "CE", NomeRegiao = "Ceará (Interior)" },
                    new Regiao { IdRegiao = 18, DDDRegiao = "89", SiglaRegiao = "PI", NomeRegiao = "Piauí (Interior)" },
                    new Regiao { IdRegiao = 19, DDDRegiao = "62", SiglaRegiao = "GO", NomeRegiao = "Goiás" },
                    new Regiao { IdRegiao = 20, DDDRegiao = "63", SiglaRegiao = "TO", NomeRegiao = "Tocantins" },
                    new Regiao { IdRegiao = 21, DDDRegiao = "64", SiglaRegiao = "GO", NomeRegiao = "Goiás (Interior)" },
                    new Regiao { IdRegiao = 22, DDDRegiao = "65", SiglaRegiao = "MT", NomeRegiao = "Mato Grosso" },
                    new Regiao { IdRegiao = 23, DDDRegiao = "66", SiglaRegiao = "MT", NomeRegiao = "Mato Grosso (Interior)" },
                    new Regiao { IdRegiao = 24, DDDRegiao = "67", SiglaRegiao = "MS", NomeRegiao = "Mato Grosso do Sul" },
                    new Regiao { IdRegiao = 25, DDDRegiao = "68", SiglaRegiao = "AC", NomeRegiao = "Acre" },
                    new Regiao { IdRegiao = 26, DDDRegiao = "69", SiglaRegiao = "RO", NomeRegiao = "Rondônia" },
                    new Regiao { IdRegiao = 27, DDDRegiao = "73", SiglaRegiao = "BA", NomeRegiao = "Bahia (Interior Sul)" },
                    new Regiao { IdRegiao = 28, DDDRegiao = "74", SiglaRegiao = "BA", NomeRegiao = "Bahia (Interior Norte)" },
                    new Regiao { IdRegiao = 29, DDDRegiao = "75", SiglaRegiao = "BA", NomeRegiao = "Bahia (Interior Leste)" },
                    new Regiao { IdRegiao = 30, DDDRegiao = "77", SiglaRegiao = "BA", NomeRegiao = "Bahia (Interior Oeste)" },
                    new Regiao { IdRegiao = 31, DDDRegiao = "79", SiglaRegiao = "SE", NomeRegiao = "Sergipe" },
                    new Regiao { IdRegiao = 32, DDDRegiao = "93", SiglaRegiao = "PA", NomeRegiao = "Pará (Interior Oeste)" },
                    new Regiao { IdRegiao = 33, DDDRegiao = "94", SiglaRegiao = "PA", NomeRegiao = "Pará (Interior Sudeste)" },
                    new Regiao { IdRegiao = 34, DDDRegiao = "95", SiglaRegiao = "RR", NomeRegiao = "Roraima" },
                    new Regiao { IdRegiao = 35, DDDRegiao = "96", SiglaRegiao = "AP", NomeRegiao = "Amapá" },
                    new Regiao { IdRegiao = 36, DDDRegiao = "97", SiglaRegiao = "AM", NomeRegiao = "Amazonas (Interior)" },
                    new Regiao { IdRegiao = 37, DDDRegiao = "98", SiglaRegiao = "MA", NomeRegiao = "Maranhão" },
                    new Regiao { IdRegiao = 38, DDDRegiao = "99", SiglaRegiao = "MA", NomeRegiao = "Maranhão (Interior)" }
                };
                await regioesCollection.InsertManyAsync(regioes);
            }
        }
    }
}
