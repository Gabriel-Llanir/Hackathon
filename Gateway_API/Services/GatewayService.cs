using Gateway.Models;

namespace Gateway.Services
{
    public class GatewayService(HttpClient httpClient) : IGatewayService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _urlConsultaAPI = "https://consulta-service";

        public async Task<IEnumerable<Medico>> Get_MedicosDisponiveis(string? id, string? data, string? especialidade)
        {
            string query = "";

            if (!string.IsNullOrEmpty(id))
                query += $"?id={id}";
            else
            {
                if (!string.IsNullOrEmpty(data))
                    query += $"?data={data}";

                if (!string.IsNullOrEmpty(especialidade))
                    query += !string.IsNullOrEmpty(data) ? $"&especialidade={especialidade}" : $"?especialidade={especialidade}";
            }

            var response = await _httpClient.GetAsync($"{_urlConsultaAPI}/Consulta/Medicos{query}");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            response.EnsureSuccessStatusCode();

            var medicosConsulta = await response.Content.ReadFromJsonAsync<IEnumerable<Medico_Consulta>>();

            var medicos = medicosConsulta.Select(m => new Medico
            {
                IdMedico = m.IdMedico,
                Nome = m.Nome,
                CPF = m.CPF,
                CRM = m.CRM,
                Email = m.Email,
                Senha = m.Senha,
                ValorConsulta = m.ValorConsulta,
                Especialidade = m.Especialidade,
                Ativo = m.Ativo,
                Disponibilidades = m.Disponibilidades.Select(d => new Medico.Disponibilidade
                {
                    Dia = d.Dia,
                    Horarios = d.Horarios.Select(h => new Medico.Horarios
                    {
                        Horario_Inicial = h.Horario_Inicial,
                        Horario_Final = h.Horario_Final
                    }).ToList()
                }).ToList()
            });

            return medicos;
        }

        public async Task<IEnumerable<Consulta_DTO>> Get_Consultas(string id, int idTipo)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var response = await _httpClient.GetAsync($"{_urlConsultaAPI}/Consulta/Consultas?id={id}&idTipo={idTipo}");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IEnumerable<Consulta_DTO>>();
        }

        public async Task<string> Get_ValorConsulta_Medico(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var response = await _httpClient.GetAsync($"{_urlConsultaAPI}/Consulta/Medicos/Valor?id={id}");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<Medico> Get_LoginMedico(string crm, string senha)
        {
            if (string.IsNullOrEmpty(crm) || string.IsNullOrEmpty(senha))
                return null;

            var loginMedico = new { login = crm, senha };
            var response = await _httpClient.PostAsJsonAsync($"{_urlConsultaAPI}/Consulta/Medicos/Login", loginMedico);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Medico>();
        }

        public async Task<Paciente> Get_LoginPaciente(string login, string senha)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha))
                return null;

            var loginPaciente = new { login, senha };
            var response = await _httpClient.PostAsJsonAsync($"{_urlConsultaAPI}/Consulta/Pacientes/Login", loginPaciente);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Paciente>();
        }
    }
}
