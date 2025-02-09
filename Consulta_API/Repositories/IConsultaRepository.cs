using Consulta.Models;

namespace Consulta.Repositories
{
    public interface IConsultaRepository
    {
        Task<IEnumerable<Medico>>? Get_MedicosDisponiveis(string? id, DateTime? data, string? especialidade);
        Task<IEnumerable<Consulta_DTO>>? Get_Consultas(string id, int idTipo);
        Task<string>? Get_ValorConsulta_Medico(string id);
        Task<Medico>? Get_LoginMedico(string email, string senha);
        Task<Paciente>? Get_LoginPaciente(string email, string senha);
    }
}
