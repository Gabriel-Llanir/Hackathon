using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RegisterConsumer.Models
{
    public class Regiao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("idRegiao")]
        public int IdRegiao { get; set; }

        [BsonElement("nDDDRegiao")]
        public string DDDRegiao { get; set; }

        [BsonElement("sSiglaRegiao")]
        public string SiglaRegiao { get; set; }

        [BsonElement("sNomeRegiao")]
        public string NomeRegiao { get; set; }
    }
}
