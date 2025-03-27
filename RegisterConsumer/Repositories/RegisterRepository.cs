using RegisterConsumer.Data;
using RegisterConsumer.Email;
using RegisterConsumer.Models;

namespace RegisterConsumer.Repositories
{
    public class RegisterRepository(IEmail email, DataContext context) : IRegisterRepository
    {
        private readonly IEmail _email = email;
        private readonly DataContext _context = context;

        public async Task AddAsync(Medico medico) => await _context.Medicos.InsertOneAsync(medico);

        public async Task AddAsync(Paciente paciente) => await _context.Pacientes.InsertOneAsync(paciente);

        public async Task AddAsync(Consulta consulta) { await _context.Consultas.InsertOneAsync(consulta); await _email.EnviarEmail_Paciente(consulta); }
    }
}
