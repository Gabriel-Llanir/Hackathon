using UpdateConsumer.Models;

namespace UpdateConsumer.Email
{
    public interface IEmail
    {
        Task EnviarEmail_Paciente(Consulta consulta);
    }
}
