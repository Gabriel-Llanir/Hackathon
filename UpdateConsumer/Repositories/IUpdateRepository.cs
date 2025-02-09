using UpdateConsumer.Models;

namespace UpdateConsumer.Repositories
{
    public interface IUpdateRepository
    {
        Task UpdateAsync(Medico medico);
        Task UpdateAsync(Paciente paciente);
        Task UpdateAsync(Consulta_Update consulta);
    }
}
