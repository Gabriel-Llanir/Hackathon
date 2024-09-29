using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Consulta.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("idUsuario")]
        public int IdUsuario { get; set; }

        [BsonElement("sNome")]
        public required string Nome { get; set; }

        [BsonElement("sTelefone")]
        public required string Telefone { get; set; }

        [BsonElement("sEmail")]
        public required string Email { get; set; }

        [BsonElement("dtAtualizacao")]
        public DateTime Atualizacao { get; set; }

        [BsonElement("sCodigoRegiao")]
        public string CodigoRegiao { get; set; }
    }
}
