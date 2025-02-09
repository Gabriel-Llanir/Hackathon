using Consulta.Models;
using Consulta.Repositories;
using System.Xml.Linq;

namespace Consulta.Services
{
    public class ConsultaService : IConsultaService
    {
        private readonly IConsultaRepository _usuarioRepository;

        public ConsultaService(IConsultaRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<Medico>>? Get_MedicosDisponiveis(string? id, DateTime? data, string? especialidade)
        {
            return await _usuarioRepository.Get_MedicosDisponiveis(id, data, especialidade);
        }

        public async Task<IEnumerable<Consulta_DTO>>? Get_Consultas(string? id, int idTipo)
        {
            return await _usuarioRepository.Get_Consultas(id, idTipo);
        }

        public async Task<string>? Get_ValorConsulta_Medico(string id)
        {
            return await _usuarioRepository.Get_ValorConsulta_Medico(id);
        }

        public async Task<Medico>? Get_LoginMedico(string crm, string senha)
        {
            return await _usuarioRepository.Get_LoginMedico(crm, senha);
        }

        public async Task<Paciente>? Get_LoginPaciente(string login, string senha)
        {
            return await _usuarioRepository.Get_LoginPaciente(login, senha);
        }
    }
}
