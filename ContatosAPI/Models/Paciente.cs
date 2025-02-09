namespace Gateway.Models
{
    public class Paciente
    {
        public string? IdPaciente { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string? Ativo { get; set; }
    }

    #region | DTO

    public class PacienteDTO
    {
        public string AuthToken { get; set; }

        public string IdPaciente { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public string Email { get; set; }
    }

    #endregion
}