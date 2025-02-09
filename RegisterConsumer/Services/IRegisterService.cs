using RegisterConsumer.Models;

namespace RegisterConsumer.Services
{
    public interface IRegisterService
    {
        Task AddAsync(Medico? medico);
        Task AddAsync(Paciente? paciente);
        Task AddAsync(Consulta? consulta);
    }
}
