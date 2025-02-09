using RegisterConsumer.Models;
using RegisterConsumer.Repositories;

namespace RegisterConsumer.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository _registerRepository;

        public RegisterService(IRegisterRepository registerRepository) => _registerRepository = registerRepository;

        public async Task AddAsync(Medico? medico)
        {
            if (medico != null)
                await _registerRepository.AddAsync(medico);
            else
                throw new Exception("É necessário um Médico para ser registrado!");
        }

        public async Task AddAsync(Paciente? paciente)
        {
            if (paciente != null)
                await _registerRepository.AddAsync(paciente);
            else
                throw new Exception("É necessário um Paciente para ser registrado!");
        }

        public async Task AddAsync(Consulta? consulta)
        {
            if (consulta != null)
                await _registerRepository.AddAsync(consulta);
            else
                throw new Exception("É necessário uma Consulta para ser registrada!");
        }
    }
}
