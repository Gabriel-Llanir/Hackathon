using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UpdateConsumer.Models
{
    public class Regiao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("idRegiao")]
        public int IdRegiao { get; set; }

        [BsonElement("nDDDRegiao")]
        public required string DDDRegiao { get; set; }

        [BsonElement("sSiglaRegiao")]
        public required string SiglaRegiao { get; set; }

        [BsonElement("sNomeRegiao")]
        public required string NomeRegiao { get; set; }
    }
}
