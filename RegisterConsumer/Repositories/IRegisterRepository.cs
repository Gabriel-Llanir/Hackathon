using RegisterConsumer.Models;

namespace RegisterConsumer.Repositories
{
    public interface IRegisterRepository
    {
        Task AddAsync(Medico medico);
        Task AddAsync(Paciente paciente);
        Task AddAsync(Consulta consulta);
    }
}
