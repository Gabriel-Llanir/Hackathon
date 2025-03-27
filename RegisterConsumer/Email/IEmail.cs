using RegisterConsumer.Models;

namespace RegisterConsumer.Email
{
    public interface IEmail
    {
        Task EnviarEmail_Paciente(Consulta consulta);
    }
}
