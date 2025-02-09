using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RegisterConsumer.Models
{
    public class Paciente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdPaciente { get; set; }

        [BsonElement("sNome")]
        public string Nome { get; set; }

        [BsonElement("sCPF")]
        public string CPF { get; set; }

        [BsonElement("sEmail")]
        public string Email { get; set; }

        [BsonElement("sSenha")]
        public string Senha { get; set; }

        [BsonElement("sAtivo")]
        public string Ativo { get; set; }
    }
}
