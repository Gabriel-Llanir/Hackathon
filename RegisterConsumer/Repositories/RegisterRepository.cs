using RegisterConsumer.Data;
using RegisterConsumer.Models;

namespace RegisterConsumer.Repositories
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly DataContext _context;

        public RegisterRepository(DataContext context) => _context = context;

        public async Task AddAsync(Medico medico) => await _context.Medicos.InsertOneAsync(medico);

        public async Task AddAsync(Paciente paciente) => await _context.Pacientes.InsertOneAsync(paciente);

        public async Task AddAsync(Consulta consulta) => await _context.Consultas.InsertOneAsync(consulta);
    }
}
