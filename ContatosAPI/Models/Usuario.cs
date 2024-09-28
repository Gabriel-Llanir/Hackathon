namespace Gateway.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string Nome { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        public DateTime Atualizacao { get; set; }

        public string CodigoRegiao { get; set; }
    }
}
