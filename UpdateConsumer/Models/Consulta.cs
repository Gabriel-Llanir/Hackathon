using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UpdateConsumer.Models
{
    public class Consulta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdConsulta { get; set; }

        [BsonElement("idMedico")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdMedico { get; set; }

        [BsonElement("idPaciente")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdPaciente { get; set; }

        [BsonElement("dtAgendada")]
        public string DtAgendada { get; set; }

        [BsonElement("nTotalConsulta")]
        public decimal TotalConsulta { get; set; }

        [BsonElement("sStatus")]
        public string Status { get; set; }

        [BsonElement("sMotivoCancelamento")]
        public string? MotivoCancelamento { get; set; }
    }

    public class Consulta_Update
    {
        public string IdConsulta { get; set; }
        public string Status { get; set; }
        public string? DataAgendada { get; set; }
        public string? MotivoCancelamento { get; set; }
    }
}
