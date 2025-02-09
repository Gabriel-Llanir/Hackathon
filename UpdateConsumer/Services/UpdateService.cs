using UpdateConsumer.Models;
using UpdateConsumer.Repositories;

namespace UpdateConsumer.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IUpdateRepository _updateRepository;

        public UpdateService(IUpdateRepository updateRepository) => _updateRepository = updateRepository;

        public async Task UpdateAsync(Medico medico)
        {
            await _updateRepository.UpdateAsync(medico);
        }

        public async Task UpdateAsync(Paciente? paciente)
        {
            await _updateRepository.UpdateAsync(paciente);
        }

        public async Task UpdateAsync(Consulta_Update? consulta)
        {
            await _updateRepository.UpdateAsync(consulta);
        }
    }
}
