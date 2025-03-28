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
            string senderEmail = Environment.GetEnvironmentVariable("EMAIL_ADDRESS");
            string senderPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

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
            string status = consulta.Status.StartsWith("Can") ? $"{consulta.Status}</p><p style='color: #666; font-size: 16px; font-weight: bold;'>Motivo do Cancelamento: {consulta.MotivoCancelamento}" : consulta.Status;

            try
            {
                using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(paciente.Email);
                    mail.IsBodyHtml = true;
                    mail.Subject = "Notificação de Agendamento de Consulta";
                    mail.Body = @$"
                        <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                            <table align='center' width='600' style='background: #ffffff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                                <tr>
                                    <td style='text-align: center;'>
                                        <h2 style='color: #333;'>Lembrete de Consulta</h2>
                                        <p style='color: #666; font-size: 16px;'>Olá {paciente.Nome}, este é um lembrete sobre sua Consulta marcada com o Médico {medico.Nome}.</p>
                                        <p style='color: #666; font-size: 16px; font-weight: bold;'>Data e Hora: {consulta.DtAgendada}</p>
                                        <p style='color: #666; font-size: 16px; font-weight: bold;'>Status da Consulta: {status}</p>
                                        <br />
                                        <p style='color: #666; font-size: 14px;'><b>Obs:</b> Caso hajam alterações no agendamento desta consulta, você será notificado via email.</p>
                                    </td>
                                </tr>
                            </table>
                        </div>";

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
