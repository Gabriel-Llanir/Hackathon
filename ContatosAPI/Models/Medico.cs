using System.ComponentModel.DataAnnotations;

namespace Gateway.Models
{
    public class Medico
    {
        public string? IdMedico { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public string CRM { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public int Especialidade { get; set; }

        public decimal ValorConsulta { get; set; }

        public string? Ativo { get; set; }

        public List<Disponibilidade> Disponibilidades { get; set; } = [];

        public class Disponibilidade
        {
            [Range(0, 6, ErrorMessage = "O dia da semana deve estar entre 0 (Domingo) e 6 (Sábado).")]
            public int Dia { get; set; }

            public List<Horarios> Horarios { get; set; } = [];
        }

        public class Horarios
        {
            public TimeSpan Horario_Inicial { get; set; }

            public TimeSpan Horario_Final { get; set; }
        }
    }

    public enum Especialidade
    {
        Cardiologista = 1,
        Neurologista = 2,
        Dentista = 3,
        Pneumologo = 4,
        Psicologo = 5,
        Psicoterapeuta = 6,
        Nutricionista = 7,
        Fisioterapeuta = 8,
        Oncologista = 9,
        Dermatologista = 10,
        Endocrinologista = 11
    }

    #region | DTO

    public class MedicoDTO_Consulta
    {
        public string IdMedico { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Especialidade { get; set; }

        public decimal ValorConsulta { get; set; }

        public List<DisponibilidadeDTO_Consulta> Disponibilidades { get; set; }

        public class DisponibilidadeDTO_Consulta
        {
            public string Dia { get; set; }

            public List<HorariosDTO_Consulta> Horarios { get; set; }
        }

        public class HorariosDTO_Consulta
        {
            public string Horario_Inicial { get; set; }

            public string Horario_Final { get; set; }
        }
    }

    public class MedicoDTO_Login
    {
        public string AuthToken { get; set; }

        public string IdMedico { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public string CRM { get; set; }

        public string Email { get; set; }

        public string Especialidade { get; set; }

        public decimal ValorConsulta { get; set; }

        public List<DisponibilidadeDTO_Login> Disponibilidades { get; set; }

        public class DisponibilidadeDTO_Login
        {
            public string Dia { get; set; }

            public List<HorariosDTO_Login> Horarios { get; set; }
        }

        public class HorariosDTO_Login
        {
            public string Horario_Inicial { get; set; }

            public string Horario_Final { get; set; }
        }
    }

    public enum DiaDaSemana
    {
        Domingo = 0,
        Segunda = 1,
        Terça = 2,
        Quarta = 3,
        Quinta = 4,
        Sexta = 5,
        Sábado = 6
    }

    #endregion

    #region | Consulta

    public class Medico_Consulta
    {
        public string? IdMedico { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public string CRM { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public int Especialidade { get; set; }

        public decimal ValorConsulta { get; set; }

        public string? Ativo { get; set; }

        public List<Disponibilidade> Disponibilidades { get; set; } = [];

        public class Disponibilidade
        {
            [Range(0, 6, ErrorMessage = "O dia da semana deve estar entre 0 (Domingo) e 6 (Sábado).")]
            public int Dia { get; set; }

            public List<Horarios> Horarios { get; set; } = [];
        }

        public class Horarios
        {
            public TimeSpan Horario_Inicial { get; set; }

            public TimeSpan Horario_Final { get; set; }
        }
    }

    #endregion
}
