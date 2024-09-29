namespace Consulta.Models
{
    public class Regiao
    {
        public string Id { get; set; }

        public int IdRegiao { get; set; }

        public required string DDDRegiao { get; set; }

        public required string SiglaRegiao { get; set; }

        public required string NomeRegiao { get; set; }
    }
}
