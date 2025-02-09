using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace UpdateConsumer.Models
{
    public class Medico
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdMedico { get; set; }

        [BsonElement("sNome")]
        public string Nome { get; set; }

        [BsonElement("sCPF")]
        public string CPF { get; set; }

        [BsonElement("sCRM")]
        public string CRM { get; set; }

        [BsonElement("sEmail")]
        public string Email { get; set; }

        [BsonElement("sSenha")]
        public string Senha { get; set; }

        [BsonElement("idEspecialidade")]
        public int Especialidade { get; set; }

        [BsonElement("nValorConsulta")]
        public decimal ValorConsulta { get; set; }

        [BsonElement("sAtivo")]
        public string Ativo { get; set; }

        [BsonElement("aDisponibilidades")]
        public List<Disponibilidade> Disponibilidades { get; set; } = [];

        public class Disponibilidade
        {
            [BsonElement("nDia")]
            [Range(0, 6, ErrorMessage = "O dia da semana deve estar entre 0 (Domingo) e 6 (Sábado).")]
            public int Dia { get; set; }

            [BsonElement("aHorarios")]
            public List<Horarios> Horarios { get; set; } = [];
        }

        public class Horarios
        {
            [BsonElement("inicio")]
            [BsonRepresentation(BsonType.String)]
            public TimeSpan Horario_Inicial { get; set; }

            [BsonElement("fim")]
            [BsonRepresentation(BsonType.String)]
            public TimeSpan Horario_Final { get; set; }
        }
    }
}
