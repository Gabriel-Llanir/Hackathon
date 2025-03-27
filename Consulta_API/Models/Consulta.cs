using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Consulta.Models
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

    #region | Consulta_DTO

    public class Consulta_DTO
    {
        public string IdConsulta { get; set; }

        public string Nome_Medico { get; set; }

        public string Nome_Paciente { get; set; }

        public string IdMedico { get; set; }

        public string IdPaciente { get; set; }

        public string DtAgendada { get; set; }

        public decimal TotalConsulta { get; set; }

        public string Status { get; set; }

        public string? MotivoCancelamento { get; set; }
    }

    #endregion
}
