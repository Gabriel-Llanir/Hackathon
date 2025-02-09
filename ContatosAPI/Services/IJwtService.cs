
namespace Gateway.Services
{
    public interface IJwtService
    {
        string GerarToken_Login(string idUsuario, string idLogin, int idTipoUsuario);
        bool ValidarToken_Login(string token, string? filtro_idTipo);
        string Get_idUsuario(string token);
        int Get_idTipoUsuario(string token);
    }
}
