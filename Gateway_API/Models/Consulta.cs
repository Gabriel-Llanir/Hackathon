namespace Gateway.Models
{
    public class Consulta
    {
        public string IdConsulta { get; set; }

        public string IdMedico { get; set; }

        public string IdPaciente { get; set; }

        public string DtAgendada { get; set; }

        public decimal TotalConsulta { get; set; }

        public string Status { get; set; }

        public string? MotivoCancelamento { get; set; }
    }

    #region | DTO

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
