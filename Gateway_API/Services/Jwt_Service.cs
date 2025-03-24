using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gateway.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;

        public JwtService()
        {
            _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");

            if (string.IsNullOrEmpty(_secretKey))
                throw new InvalidOperationException("JWT_Secret não está definido.");
        }

        public string GerarToken_Login(string idUsuario, string idLogin, int idTipoUsuario) // idTipo: 1 = Médico, 2 = Paciente
        {
            if (string.IsNullOrEmpty(idUsuario) || string.IsNullOrEmpty(idLogin) || idTipoUsuario <= 0)
                throw new Exception("O ID do Usuário, o ID de Login e o Tipo de Usuário são obrigatórios para a geração do Token JWT!");

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("idUsuario", idUsuario),
                    new Claim("idLogin", idLogin),
                    new Claim("idTipoUsuario", idTipoUsuario.ToString())
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public bool ValidarToken_Login(string token, string? filtro_idTipo) // filtro: 1 = Médico, 2 = Paciente
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var idTipoUsuario = principal.Claims.FirstOrDefault(c => c.Type == "idTipoUsuario");

                if (filtro_idTipo == null || (idTipoUsuario != null && idTipoUsuario.Value == filtro_idTipo))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string Get_idUsuario(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return principal.Claims.First(c => c.Type == "idUsuario").Value;
            }
            catch
            {
                return null;
            }
        }

        public int Get_idTipoUsuario(string token) // idTipo: 1 = Médico, 2 = Paciente
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return int.Parse(principal.Claims.First(c => c.Type == "idTipoUsuario").Value);
            }
            catch
            {
                return 0;
            }
        }

    }
}
