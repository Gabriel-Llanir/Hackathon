using UpdateConsumer.Models;

namespace UpdateConsumer.Services
{
    public interface IUpdateService
    {
        Task UpdateAsync(Medico medico);
        Task UpdateAsync(Paciente pacientes);
        Task UpdateAsync(Consulta_Update consulta);
    }
}
