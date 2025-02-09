using Consulta.Models;

namespace Consulta.Services
{
    public interface IConsultaService
    {
        Task<IEnumerable<Medico>>? Get_MedicosDisponiveis(string? id, DateTime? data, string? especialidade);
        Task<IEnumerable<Consulta_DTO>>? Get_Consultas(string id,int idTipo);
        Task<string>? Get_ValorConsulta_Medico(string id);
        Task<Medico>? Get_LoginMedico(string crm, string senha);
        Task<Paciente>? Get_LoginPaciente(string login, string senha);
    }
}
