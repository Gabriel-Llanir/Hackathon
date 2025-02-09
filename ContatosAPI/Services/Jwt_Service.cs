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
                _secretKey = "2d7752a7f0adfbe9b3e4cdad0bad5a7eff9b08df46ece60a7e69df18a73eec6987f237ce182aeba74a4a765cb7d85e96fc6cb616512370a9f179a58a93666ee58c9aba680594aaec6d48c97539aa935ae37c0d22931e74261c916923fe9c2e4b16acc65fa21d5439e79d17b763688a87d7b7aab545fd0754c10b478dee7c9372ae2f7bd54298417f75a275abeeac05fee7d0d05bbb3b3d41d5c74804d742e2dde0ec8057f2e8eafaa8dd166cf36e9763a2bbf3bc4ba97f000158ff515e00527465cfc6bbf1fd57246ce1a288c54508b921baf8743fa13259fb337753dffcf584b77f64b73fff53ccbd4b5a60eb6b5d045d5bc2dea1d900bfd22e274ddda7d20e";
                //throw new InvalidOperationException("JWT_SECRET não está definido.");
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
