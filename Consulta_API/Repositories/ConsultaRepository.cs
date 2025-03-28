using Consulta.Data;
using Consulta.Models;
using MongoDB.Driver;
using System.Globalization;
using static BCrypt.Net.BCrypt;

namespace Consulta.Repositories
{
    public class ConsultaRepository : IConsultaRepository
    {
        private readonly DataContext _context;

        public ConsultaRepository(DataContext context) => _context = context;

        public async Task<IEnumerable<Medico>>? Get_MedicosDisponiveis(string? id, DateTime? data, string? especialidade)
        {
            int.TryParse(especialidade, out int especialidadeValue);
            DateTime.TryParse(data.ToString(), out DateTime dataConvertida);

            var builder = Builders<Medico>.Filter;
            var filtro = builder.Eq(m => m.Ativo, "S");

            var medicos = new List<Medico>();

            if (id != null)
            {
                filtro = builder.And(
                    filtro,
                    builder.Eq(m => m.IdMedico, id)
                );

                //medicos = await _context.Medicos.Find(filtro).ToListAsync();
                var cursor = await _context.Medicos.FindAsync(filtro);
                await cursor.MoveNextAsync();

                medicos = [.. cursor.Current];
            }
            else if (data != null && especialidade != null)
            {
                var diaDaSemana = (int)dataConvertida.DayOfWeek;
                var horario = dataConvertida.TimeOfDay;

                filtro = builder.And(
                    filtro,
                    builder.Eq("idEspecialidade", especialidadeValue),
                    builder.ElemMatch(m => m.Disponibilidades, d =>
                        d.Dia == diaDaSemana &&
                        d.Horarios.Any(h => h.Horario_Inicial <= horario && h.Horario_Final > (horario + new TimeSpan(1, 0, 0)))
                    )
                );

                medicos = await _context.Medicos.Find(filtro).ToListAsync();
            }
            else if (especialidade != null)
            {
                filtro = builder.And(
                    filtro,
                    builder.Eq(m => m.Especialidade, especialidadeValue)
                );

                medicos = await _context.Medicos.Find(filtro).ToListAsync();
            }
            else if (data != null)
            {
                var diaDaSemana = (int)dataConvertida.DayOfWeek;
                var horario = dataConvertida.TimeOfDay;

                filtro = builder.And(
                    filtro,
                    builder.ElemMatch(m => m.Disponibilidades, d =>
                        d.Dia == diaDaSemana &&
                        d.Horarios.Any(h => h.Horario_Inicial <= horario && h.Horario_Final > (horario + new TimeSpan(1, 0, 0)))
                    )
                );

                medicos = await _context.Medicos.Find(filtro).ToListAsync();
            }
            else
                medicos = await _context.Medicos.Find(_ => true).ToListAsync();

            medicos.ForEach(m => m.Disponibilidades.ForEach(d => d.Horarios = d.Horarios.SelectMany(h =>
            {
                var horarioInicial = h.Horario_Inicial;
                var horarioFinal = h.Horario_Final;
                var faixasDeHorario = new List<Medico.Horarios>();

                while (horarioInicial < horarioFinal)
                {
                    var proximoHorario = horarioInicial.Add(new TimeSpan(1, 0, 0));

                    if (proximoHorario > horarioFinal)
                        proximoHorario = horarioFinal;

                    faixasDeHorario.Add(new Medico.Horarios
                    {
                        Horario_Inicial = horarioInicial,
                        Horario_Final = proximoHorario
                    });

                    horarioInicial = proximoHorario;
                }

                return faixasDeHorario;
            }).ToList()));

            try
            {
                foreach (var medico in medicos)
                {
                    medico.Disponibilidades.ForEach(d =>
                    {
                        var consultasAgendadas = _context.Consultas.Find(c => c.IdMedico == medico.IdMedico && c.Status == "Agendada").ToList().Where(c => DateTime.ParseExact(c.DtAgendada, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).DayOfWeek == (DayOfWeek)d.Dia).ToList();

                        d.Horarios = d.Horarios.Where(h =>
                        {
                            return !consultasAgendadas.Any(consulta =>
                            {
                                var horarioAgendado = TimeSpan.Parse(consulta.DtAgendada.Split(' ')[1]);
                                return horarioAgendado >= h.Horario_Inicial && horarioAgendado < h.Horario_Final;
                            });
                        }).ToList();
                    });
                }
            }
            catch (Exception ex) { }

            return medicos;
        }

        public async Task<IEnumerable<Consulta_DTO>>? Get_Consultas(string id, int idTipo)
        {
            var builder = Builders<Models.Consulta>.Filter;
            var builder_Medico = Builders<Medico>.Filter;
            var builder_Paciente = Builders<Paciente>.Filter;

            var filtro = idTipo == 1 ?
                  builder.Eq(p => p.IdMedico, id)
                : builder.Eq(p => p.IdPaciente, id);

            //filtro = builder.And(
            //    filtro,
            //    builder.Or(
            //        builder.Eq(p => p.Status, "Agendada"),
            //        builder.Eq(p => p.Status, "Em processamento")
            //    )
            //);

            var cursor_ConsultasOriginal = await _context.Consultas.FindAsync(filtro);
            await cursor_ConsultasOriginal.MoveNextAsync();

            List<Models.Consulta> consultas_Original = [.. cursor_ConsultasOriginal.Current];
            Console.WriteLine(string.Format("Primeira Consulta: {0}", consultas_Original.FirstOrDefault()));

            var consultas = new List<Consulta_DTO>();

            if (consultas_Original != null && consultas_Original.Count > 0)
            {
                Console.WriteLine("--- chegou no foreach ---");
                consultas_Original.ForEach(async c =>
                {
                    var cursor_medico = await _context.Medicos.FindAsync(builder_Medico.Eq(m => m.IdMedico, c.IdMedico));
                    await cursor_medico.MoveNextAsync();
                    var cursor_paciente = await _context.Pacientes.FindAsync(builder_Paciente.Eq(p => p.IdPaciente, c.IdPaciente));
                    await cursor_paciente.MoveNextAsync();

                    Console.WriteLine("--- passou dos cursores de Médico e Paciente ---");

                    Medico medico = cursor_medico.Current.FirstOrDefault();
                    Paciente paciente = cursor_paciente.Current.FirstOrDefault();

                    Console.WriteLine(string.Format("===== Médico: {0}, Paciente: {1} ===", medico, paciente));

                    consultas.Add(new Consulta_DTO
                    {
                        IdConsulta = c.IdConsulta,
                        IdMedico = c.IdMedico,
                        IdPaciente = c.IdPaciente,
                        Nome_Medico = medico.Nome,
                        Nome_Paciente = paciente.Nome,
                        DtAgendada = c.DtAgendada,
                        TotalConsulta = c.TotalConsulta,
                        Status = c.Status,
                        MotivoCancelamento = c.MotivoCancelamento
                    });
                });
            }
            Console.WriteLine(string.Format("--- Consultas: {0}, Quantas: {1}, Alguma: {2} ---", consultas, consultas != null ? consultas.Count() : false, consultas != null ? consultas.Any() : false));

            return consultas;
        }

        public async Task<string>? Get_ValorConsulta_Medico(string id)
        {
            var builder = Builders<Medico>.Filter;

            var filtro = builder.And(
                builder.Eq(p => p.IdMedico, id)
            );

            var cursor_medico = await _context.Medicos.FindAsync(filtro);
            await cursor_medico.MoveNextAsync();

            var medico = cursor_medico.Current.FirstOrDefault();

            if (medico != null)
                return medico.ValorConsulta.ToString();

            return null;
        }

        public async Task<Medico>? Get_LoginMedico(string crm, string senha)
        {
            var builder = Builders<Medico>.Filter;

            var filtro = builder.And(
                builder.Eq(p => p.Ativo, "S"),
                builder.Eq(p => p.CRM, crm)
            );

            var cursor_medico = await _context.Medicos.FindAsync(filtro);
            await cursor_medico.MoveNextAsync();

            var medico = cursor_medico.Current.FirstOrDefault();

            if (medico != null && Verify(senha, medico.Senha, false, BCrypt.Net.HashType.SHA384))
                return medico;

            return null;
        }

        public async Task<Paciente>? Get_LoginPaciente(string login, string senha)
        {
            var builder = Builders<Paciente>.Filter;

            var filtro = builder.And(
                builder.Eq(p => p.Ativo, "S"),
                builder.Or(
                    builder.Eq(p => p.Email, login),
                    builder.Eq(p => p.CPF, login)
                )
            );

            var cursor_paciente = await _context.Pacientes.FindAsync(filtro);
            await cursor_paciente.MoveNextAsync();

            var paciente = cursor_paciente.Current.FirstOrDefault();

            if (paciente != null && Verify(senha, paciente.Senha, false, BCrypt.Net.HashType.SHA384))
                return paciente;

            return null;
        }

    }
}
