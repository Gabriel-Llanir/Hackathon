using UpdateConsumer.Data;
using UpdateConsumer.Models;
using MongoDB.Driver;
using UpdateConsumer.Email;

namespace UpdateConsumer.Repositories
{
    public class UpdateRepository(IEmail email, DataContext context) : IUpdateRepository
    {
        private readonly IEmail _email = email;
        private readonly DataContext _context = context;

        public async Task UpdateAsync(Medico medico) => await _context.Medicos.ReplaceOneAsync(m => m.IdMedico.Equals(medico.IdMedico), medico);

        public async Task UpdateAsync(Paciente paciente) => await _context.Pacientes.ReplaceOneAsync(p => p.IdPaciente == paciente.IdPaciente, paciente);

        public async Task UpdateAsync(Consulta_Update consulta)
        {
            var consulta_Original = await _context.Consultas.Find(c => c.IdConsulta == consulta.IdConsulta).FirstOrDefaultAsync();

            consulta.Status = consulta.Status.Trim().ToLower();
            consulta_Original.Status = $"{char.ToUpper(consulta.Status[0])}{consulta.Status[1..]}";
            consulta_Original.DtAgendada = string.IsNullOrEmpty(consulta.DataAgendada) ? consulta_Original.DtAgendada : consulta.DataAgendada;
            consulta_Original.MotivoCancelamento = consulta.MotivoCancelamento;

            await _context.Consultas.ReplaceOneAsync(c => c.IdConsulta == consulta_Original.IdConsulta, consulta_Original);
            await _email.EnviarEmail_Paciente(consulta_Original);
        }
    }
}
