using Gateway.Models;

namespace Gateway.Services
{
    public class UsuarioService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : IUsuarioService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly string _urlConsultaAPI = "https://localhost:5010";

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync($"{_urlConsultaAPI}/Consulta");
            response.EnsureSuccessStatusCode();

            var usuario = await response.Content.ReadFromJsonAsync<IEnumerable<Usuario>>();

            return usuario;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync(string DDD)
        {
            var response = await _httpClient.GetAsync($"{_urlConsultaAPI}/{DDD}");
            response.EnsureSuccessStatusCode();

            var usuario = await response.Content.ReadFromJsonAsync<IEnumerable<Usuario>>();

            return usuario;
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_urlConsultaAPI}/{id}");
            response.EnsureSuccessStatusCode();

            var usuario = await response.Content.ReadFromJsonAsync<Usuario>();
            return usuario;
        }
    }
}
