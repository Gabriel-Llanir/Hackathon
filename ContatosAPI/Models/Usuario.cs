namespace Gateway.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public required string Nome { get; set; }

        public required string Telefone { get; set; }

        public required string Email { get; set; }

        public DateTime Atualizacao { get; set; }

        public required string CodigoRegiao { get; set; }
    }
}
