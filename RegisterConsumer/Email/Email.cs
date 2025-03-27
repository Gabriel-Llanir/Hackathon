using System.Net;
using System.Net.Mail;
using RegisterConsumer.Data;
using RegisterConsumer.Models;
using MongoDB.Driver;

namespace RegisterConsumer.Email
{
    public class Email : IEmail
    {
        private readonly DataContext _context;

        public Email(DataContext context) => _context = context;

        public async Task EnviarEmail_Paciente(Consulta consulta)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string senderEmail = "hackathon.subs@gmail.com";
            string senderPassword = "ProjetoHackathon10";

            var builder_Medico = Builders<Medico>.Filter;
            var filtro_Medico = builder_Medico.And(
                builder_Medico.Eq(c => c.Ativo, "S"),
                builder_Medico.Eq(c => c.IdMedico, consulta.IdMedico)
            );
            var builder_Paciente = Builders<Paciente>.Filter;
            var filtro_Paciente = builder_Paciente.And(
                builder_Paciente.Eq(c => c.Ativo, "S"),
                builder_Paciente.Eq(c => c.IdPaciente, consulta.IdPaciente)
            );

            var medico = await _context.Medicos.Find(filtro_Medico).FirstOrDefaultAsync();
            var paciente = await _context.Pacientes.Find(filtro_Paciente).FirstOrDefaultAsync();

            try
            {
                using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(paciente.Email);
                    mail.Subject = "Notificação de Agendamento de Consulta";
                    mail.Body = $"<b>Olá {paciente.Nome}!</b><br /> Este é um email automático, enviado para te lembrar que há uma Consulta Agendada no dia e hora <b>{consulta.DtAgendada}</b>, com o Médico <b>{medico.Nome}</b>!";
                    mail.IsBodyHtml = true;

                    smtp.Send(mail);
                    Console.WriteLine("Email enviado com sucesso!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
            }
        }
    }
}
