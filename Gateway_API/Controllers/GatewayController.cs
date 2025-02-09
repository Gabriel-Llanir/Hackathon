using System.Web;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Gateway.Models;
using Gateway.Services;
using System.Globalization;
using MongoDB.Bson;
using System.Security.Cryptography;
using static Gateway.Models.MedicoDTO_Login;
using static Gateway.Models.MedicoDTO_Consulta;
using System.Text.RegularExpressions;

namespace Gateway.Controllers
{
    [Route("api")]
    [ApiController]
    public class GatewayController(IGatewayService gatewayService, IJwtService jwtService) : ControllerBase
    {
        #region | Construtores

        private static readonly Histogram RequestDuration = Metrics.CreateHistogram("http_request_duration_seconds", "Duration of HTTP requests in seconds", new HistogramConfiguration { LabelNames = ["method", "endpoint"] });
        private static readonly Counter RequestCounter = Metrics.CreateCounter("http_requests_total", "Total number of HTTP requests made", new CounterConfiguration { LabelNames = ["method", "endpoint", "status"] });

        private readonly IGatewayService _gatewayService = gatewayService;
        private readonly IJwtService _jwtService = jwtService;

        private static readonly string chavePublica = Environment.GetEnvironmentVariable("RSA_Public");

        private static readonly string padraoCPF = @"^\d{3}\.\d{3}\.\d{3}-\d{2}$";
        private static readonly string padraoEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private static readonly DateTime dataAmanha = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None);

        private readonly ConnectionFactory factory = new()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };

        public class Login
        {
            public string login { get; set; }
            public string senha { get; set; }
        }

        public class Consulta_Register
        {
            public string IdMedico { get; set; }
            public string DtAgendada { get; set; }
        }

        public class Consulta_Update
        {
            public string IdConsulta { get; set; }
            public string Status { get; set; }
            public string? MotivoCancelamento { get; set; }
        }

        #endregion

        #region | Endpoints

        [HttpGet("Medicos")]
        public async Task<ActionResult<IEnumerable<Medico>>> Get_MedicosDisponiveis()
        {
            using (RequestDuration.WithLabels("GET", "api/Medicos").NewTimer())
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Trim();

                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token["Bearer ".Length..].Trim();

                if (!_jwtService.ValidarToken_Login(token, "2"))
                {
                    RequestCounter.WithLabels("GET", "api/Medicos", Unauthorized().StatusCode.ToString()).Inc();

                    return Unauthorized("É necessário estar logado como Paciente para Consultar os Médicos Disponíveis!");
                }

                var queryParams = HttpContext.Request.Query;

                string? id = queryParams.ContainsKey("id") ? queryParams["id"].ToString() : null;
                string? data = queryParams.ContainsKey("data") ? queryParams["data"].ToString() : null;
                string? especialidade = queryParams.ContainsKey("especialidade") ? queryParams["especialidade"].ToString() : null;

                if (!string.IsNullOrEmpty(data) && (!DateTime.TryParseExact(HttpUtility.UrlDecode(data), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataConvertida) || dataConvertida < dataAmanha))
                {
                    RequestCounter.WithLabels("GET", "api/Medicos", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest(dataConvertida < dataAmanha ? $"A data para consulta deve ser ao menos um dia após a data de hoje (mínimo: {dataAmanha})." : "Formato de data inválido. dd/MM/yyyy HH:mm:ss.");
                }
                else if (!string.IsNullOrEmpty(especialidade) && !(int.TryParse(especialidade, out int especialidadeValue) && Enum.IsDefined(typeof(Especialidade), especialidadeValue)))
                {
                    RequestCounter.WithLabels("GET", "api/Medicos", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O filtro por Especialidade deve ser uma das opções válidas.");
                }

                var medicos = await _gatewayService.Get_MedicosDisponiveis(id, data, especialidade);

                if (medicos != null && medicos.Any())
                {
                    var medicosDTO = medicos.Select(m => new MedicoDTO_Consulta
                    {
                        IdMedico = m.IdMedico,
                        Nome = m.Nome,
                        Email = m.Email,
                        ValorConsulta = Math.Round(m.ValorConsulta, 2),
                        Especialidade = Enum.GetName(typeof(Especialidade), m.Especialidade) ?? "",
                        Disponibilidades = m.Disponibilidades.Select(d => new DisponibilidadeDTO_Consulta
                        {
                            Dia = Enum.GetName(typeof(DiaDaSemana), d.Dia),
                            Horarios = d.Horarios.Select(h => new HorariosDTO_Consulta
                            {
                                Horario_Inicial = h.Horario_Inicial.ToString(@"hh\:mm\:ss"),
                                Horario_Final = h.Horario_Final.ToString(@"hh\:mm\:ss")
                            }).ToList()
                        }).ToList()
                    }).ToList();

                    RequestCounter.WithLabels("GET", "api/Medicos", Ok().StatusCode.ToString()).Inc();

                    return Ok(medicosDTO);
                }
                else
                {
                    RequestCounter.WithLabels("GET", "api/Medicos", NotFound().StatusCode.ToString()).Inc();

                    return NotFound();
                }
            }
        }

        [HttpGet("Consultas")]
        public async Task<ActionResult<IEnumerable<Medico>>> Get_Consultas_x_Usuario()
        {
            using (RequestDuration.WithLabels("GET", "api/Consultas").NewTimer())
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Trim();

                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token["Bearer ".Length..].Trim();

                if (!_jwtService.ValidarToken_Login(token, null))
                {
                    RequestCounter.WithLabels("GET", "api/Consultas", Unauthorized().StatusCode.ToString()).Inc();

                    return Unauthorized("É necessário estar logado para executar esta Consulta!");
                }

                var consultas = await _gatewayService.Get_Consultas(_jwtService.Get_idUsuario(token), _jwtService.Get_idTipoUsuario(token));

                if (consultas != null && consultas.Any())
                {
                    RequestCounter.WithLabels("GET", "api/Consultas", Ok().StatusCode.ToString()).Inc();

                    return Ok(consultas);
                }
                else
                {
                    RequestCounter.WithLabels("GET", "api/Consultas", NotFound().StatusCode.ToString()).Inc();

                    return NotFound();
                }
            }
        }

        [HttpPost("Medicos/Login")]
        public async Task<IActionResult> GetLogin_Medico([FromBody] Login login)
        {
            using (RequestDuration.WithLabels("GET", "api/Medicos/Login").NewTimer())
            {
                var rsa = new RSACryptoServiceProvider();
                byte[] publicKeyBytes = Convert.FromBase64String(chavePublica);
                rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

                string senha = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(login.senha), false));

                var medicoLogado = await _gatewayService.Get_LoginMedico(login.login, senha);

                if (medicoLogado == null)
                {
                    RequestCounter.WithLabels("GET", "api/Medicos/Login", NotFound().StatusCode.ToString()).Inc();

                    return NotFound();
                }

                var jwtToken = _jwtService.GerarToken_Login(medicoLogado.IdMedico, Guid.NewGuid().ToString(), 1);

                var medicoDTO = new MedicoDTO_Login
                {
                    AuthToken = $"Bearer {jwtToken}",
                    IdMedico = medicoLogado.IdMedico,
                    Nome = medicoLogado.Nome,
                    CPF = medicoLogado.CPF,
                    CRM = medicoLogado.CRM,
                    Email = medicoLogado.Email,
                    ValorConsulta = Math.Round(medicoLogado.ValorConsulta, 2),
                    Especialidade = Enum.GetName(typeof(Especialidade), medicoLogado.Especialidade) ?? "",
                    Disponibilidades = medicoLogado.Disponibilidades.Select(d => new DisponibilidadeDTO_Login
                    {
                        Dia = Enum.GetName(typeof(DiaDaSemana), d.Dia),
                        Horarios = d.Horarios.Select(h => new HorariosDTO_Login
                        {
                            Horario_Inicial = h.Horario_Inicial.ToString(@"hh\:mm\:ss"),
                            Horario_Final = h.Horario_Final.ToString(@"hh\:mm\:ss")
                        }).ToList()
                    }).ToList()
                };

                var response = Ok(medicoDTO);

                RequestCounter.WithLabels("GET", "api/Medicos/Login", response.StatusCode.ToString()).Inc();

                return response;
            }
        }

        [HttpPost("Pacientes/Login")]
        public async Task<IActionResult> GetLogin_Paciente([FromBody] Login login)
        {
            using (RequestDuration.WithLabels("GET", "api/Pacientes/Login").NewTimer())
            {
                var rsa = new RSACryptoServiceProvider();
                byte[] publicKeyBytes = Convert.FromBase64String(chavePublica);
                rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

                string senha = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(login.senha), false));

                var pacienteLogado = await _gatewayService.Get_LoginPaciente(login.login, senha);

                if (pacienteLogado == null)
                {
                    RequestCounter.WithLabels("GET", "api/Pacientes/Login", NotFound().StatusCode.ToString()).Inc();

                    return NotFound();
                }

                string jwtToken = _jwtService.GerarToken_Login(pacienteLogado.IdPaciente, Guid.NewGuid().ToString(), 2);

                var pacienteDTO = new PacienteDTO
                {
                    AuthToken = $"Bearer {jwtToken}",
                    IdPaciente = pacienteLogado.IdPaciente,
                    Nome = pacienteLogado.Nome,
                    CPF = pacienteLogado.CPF,
                    Email = pacienteLogado.Email
                };

                var response = Ok(pacienteDTO);

                RequestCounter.WithLabels("GET", "api/Pacientes/Login", response.StatusCode.ToString()).Inc();

                return response;
            }
        }

        [HttpPost("Medicos/Register")]
        public IActionResult Register_Medico([FromBody] Medico medico)
        {
            using (RequestDuration.WithLabels("POST", "api/Medicos/Register").NewTimer())
            {
                if (string.IsNullOrEmpty(medico.Nome))
                {
                    RequestCounter.WithLabels("POST", "api/Medicos/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Nome é obrigatório!");
                }

                if (!Regex.IsMatch(medico.Email, padraoEmail))
                {
                    RequestCounter.WithLabels("POST", "api/Medicos/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Email é obrigatório e deve seguir o seguinte formato: exemplo@exemplo.com!");
                }

                if (string.IsNullOrEmpty(medico.Senha))
                {
                    RequestCounter.WithLabels("POST", "api/Medicos/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A Senha é obrigatória!");
                }

                if (!Regex.IsMatch(medico.CPF, padraoCPF))
                {
                    RequestCounter.WithLabels("POST", "api/Medicos/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O CPF é obrigatório e deve seguir o seguinte formato: XXX.XXX.XXX-XX!");
                }

                if (string.IsNullOrEmpty(medico.CRM))
                {
                    RequestCounter.WithLabels("POST", "api/Medicos/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O CRM é obrigatório!");
                }

                if (!Enum.IsDefined(typeof(Especialidade), medico.Especialidade))
                {
                    RequestCounter.WithLabels("POST", "api/Medicos/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A Especialidade é obrigatória e deve ser uma das opções válidas!");
                }

                if (string.IsNullOrEmpty(medico.ValorConsulta.ToString()) || medico.ValorConsulta <= decimal.Zero)
                {
                    RequestCounter.WithLabels("POST", "api/Medicos/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Valor da Consulta é obrigatório e deve ser um valor decimal acima de 0 (zero)!");
                }

                medico.IdMedico = ObjectId.GenerateNewId().ToString();
                medico.Senha = BCrypt.Net.BCrypt.HashPassword(medico.Senha, 10);
                medico.Disponibilidades = [];
                medico.Ativo = "S";

                using (var channel = factory.CreateConnection().CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "Medicos_RegisterQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "Medicos_RegisterQueue",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(medico)));
                }

                RequestCounter.WithLabels("POST", "api/Medicos/Register", Ok().StatusCode.ToString()).Inc();

                return Ok(medico.IdMedico);
            }
        }

        [HttpPost("Pacientes/Register")]
        public IActionResult Register_Paciente([FromBody] Paciente paciente)
        {
            using (RequestDuration.WithLabels("POST", "api/Pacientes/Register").NewTimer())
            {
                if (string.IsNullOrEmpty(paciente.Nome))
                {
                    RequestCounter.WithLabels("POST", "api/Pacientes/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Nome é obrigatório!");
                }

                if (!Regex.IsMatch(paciente.Email, padraoEmail))
                {
                    RequestCounter.WithLabels("POST", "api/Pacientes/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Email é obrigatório e deve seguir o seguinte formato: exemplo@exemplo.com!");
                }

                if (string.IsNullOrEmpty(paciente.Senha))
                {
                    RequestCounter.WithLabels("POST", "api/Pacientes/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A Senha é obrigatória!");
                }

                if (!Regex.IsMatch(paciente.CPF, padraoCPF))
                {
                    RequestCounter.WithLabels("POST", "api/Pacientes/Register", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O CPF é obrigatório e deve seguir o seguinte formato: XXX.XXX.XXX-XX!");
                }

                paciente.IdPaciente = ObjectId.GenerateNewId().ToString();
                paciente.Senha = BCrypt.Net.BCrypt.HashPassword(paciente.Senha, 10);
                paciente.Ativo = "S";

                using (var channel = factory.CreateConnection().CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "Pacientes_RegisterQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "Pacientes_RegisterQueue",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(paciente)));
                }

                RequestCounter.WithLabels("POST", "api/Pacientes/Register", Ok().StatusCode.ToString()).Inc();

                return Ok(paciente.IdPaciente);
            }
        }

        [HttpPost("Consultas/Agendar")]
        public async Task<IActionResult> Register_Consulta([FromBody] Consulta_Register agendar_Consulta)
        {
            using (RequestDuration.WithLabels("POST", "api/Consultas/Agendar").NewTimer())
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Trim();

                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token["Bearer ".Length..].Trim();

                if (!_jwtService.ValidarToken_Login(token, "2"))
                {
                    RequestCounter.WithLabels("POST", "api/Consultas/Agendar", Unauthorized().StatusCode.ToString()).Inc();

                    return Unauthorized("É necessário estar logado como Paciente para Agendar uma Consulta!");
                }

                if (string.IsNullOrEmpty(agendar_Consulta.IdMedico))
                {
                    RequestCounter.WithLabels("POST", "api/Consultas/Agendar", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O ID do médico é obrigatório!");
                }

                if (!DateTime.TryParseExact(agendar_Consulta.DtAgendada, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data) || data <= dataAmanha)
                {
                    RequestCounter.WithLabels("POST", "api/Consultas/Agendar", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest($"A data para consulta deve estar no formato (dd/MM/yyyy HH:mm:ss) e ser ao menos um dia após a data de hoje (mínimo: {DateTime.Now.AddDays(1):dd/MM/yyyy HH:mm:ss}).");
                }

                if (!decimal.TryParse(await _gatewayService.Get_ValorConsulta_Medico(agendar_Consulta.IdMedico), out decimal total))
                {
                    RequestCounter.WithLabels("POST", "api/Consultas/Agendar", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("Não foi possível encontrar o Valor da Consulta a partir do ID Médico definido!");
                }

                string idConsulta = ObjectId.GenerateNewId().ToString();

                var consulta = new Consulta
                {
                    IdConsulta = idConsulta,
                    IdPaciente = _jwtService.Get_idUsuario(token),
                    IdMedico = agendar_Consulta.IdMedico,
                    DtAgendada = agendar_Consulta.DtAgendada,
                    TotalConsulta = total,
                    Status = "Em processamento"
                };

                using (var channel = factory.CreateConnection().CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "Consultas_RegisterQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "Consultas_RegisterQueue",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(consulta)));
                }

                RequestCounter.WithLabels("POST", "api/Consultas/Agendar", Ok().StatusCode.ToString()).Inc();

                return Ok(idConsulta);
            }
        }

        [HttpPut("Medicos/Update")]
        public IActionResult Update_Medicos([FromBody] Medico medico)
        {
            using (RequestDuration.WithLabels("PUT", "api/Medicos/Update").NewTimer())
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Trim();

                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token["Bearer ".Length..].Trim();

                if (!_jwtService.ValidarToken_Login(token, "1"))
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", Unauthorized().StatusCode.ToString()).Inc();

                    return Unauthorized("É necessário estar logado como Médico para realizar esta alteração!");
                }

                medico.IdMedico = _jwtService.Get_idUsuario(token);

                if (string.IsNullOrEmpty(medico.Nome))
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Nome é obrigatório!");
                }

                if (!Regex.IsMatch(medico.Email, padraoEmail))
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Email é obrigatório e deve seguir o seguinte formato: exemplo@exemplo.com!");
                }

                if (string.IsNullOrEmpty(medico.Senha))
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A Senha é obrigatória!");
                }

                if (!Regex.IsMatch(medico.CPF, padraoCPF))
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O CPF é obrigatório e deve seguir o seguinte formato: XXX.XXX.XXX-XX!");
                }

                if (string.IsNullOrEmpty(medico.CRM))
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O CRM é obrigatório!");
                }

                if (!Enum.IsDefined(typeof(Especialidade), medico.Especialidade))
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A Especialidade é obrigatória e deve ser uma das opções válidas!");
                }

                if (string.IsNullOrEmpty(medico.ValorConsulta.ToString()) || medico.ValorConsulta <= decimal.Zero)
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Valor da Consulta é obrigatório e deve ser um valor decimal acima de 0 (zero)!");
                }

                if (medico.Disponibilidades == null || medico.Disponibilidades.Count <= 0)
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A lista de Disponibilidade é obrigatória!");
                }

                if (string.IsNullOrEmpty(medico.Ativo))
                {
                    RequestCounter.WithLabels("PUT", "api/Medicos/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A situação (Ativo) é obrigatório!");
                }

                medico.Senha = BCrypt.Net.BCrypt.HashPassword(medico.Senha, 10);

                using (var channel = factory.CreateConnection().CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "Medicos_UpdateQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "Medicos_UpdateQueue",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(medico)));
                }

                RequestCounter.WithLabels("PUT", "api/Medicos/Update", Ok().StatusCode.ToString()).Inc();

                return Ok(medico.IdMedico);
            }
        }

        [HttpPut("Pacientes/Update")]
        public IActionResult Update_Pacientes([FromBody] Paciente paciente)
        {
            using (RequestDuration.WithLabels("PUT", "api/Pacientes/Update").NewTimer())
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Trim();

                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token["Bearer ".Length..].Trim();

                if (!_jwtService.ValidarToken_Login(token, "2"))
                {
                    RequestCounter.WithLabels("PUT", "api/Pacientes/Update", Unauthorized().StatusCode.ToString()).Inc();

                    return Unauthorized("É necessário estar logado como Paciente para realizar esta alteração!");
                }

                paciente.IdPaciente = _jwtService.Get_idUsuario(token);

                if (string.IsNullOrEmpty(paciente.Nome))
                {
                    RequestCounter.WithLabels("PUT", "api/Pacientes/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Nome é obrigatório!");
                }

                if (!Regex.IsMatch(paciente.Email, padraoEmail))
                {
                    RequestCounter.WithLabels("PUT", "api/Pacientes/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Email é obrigatório e deve seguir o seguinte formato: exemplo@exemplo.com!");
                }

                if (string.IsNullOrEmpty(paciente.Senha))
                {
                    RequestCounter.WithLabels("PUT", "api/Pacientes/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A Senha é obrigatória!");
                }

                if (!Regex.IsMatch(paciente.CPF, padraoCPF))
                {
                    RequestCounter.WithLabels("PUT", "api/Pacientes/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O CPF é obrigatório e deve seguir o seguinte formato: XXX.XXX.XXX-XX!");
                }

                if (string.IsNullOrEmpty(paciente.Ativo))
                {
                    RequestCounter.WithLabels("PUT", "api/Pacientes/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("A situação (Ativo) é obrigatório!");
                }

                paciente.Senha = BCrypt.Net.BCrypt.HashPassword(paciente.Senha, 10);

                using (var channel = factory.CreateConnection().CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "Pacientes_UpdateQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "Pacientes_UpdateQueue",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(paciente)));
                }

                RequestCounter.WithLabels("PUT", "api/Pacientes/Update", Ok().StatusCode.ToString()).Inc();

                return Ok(paciente.IdPaciente);
            }
        }

        [HttpPut("Consultas/Update")]
        public IActionResult Update_Consulta([FromBody] Consulta_Update consulta_Update)
        {
            using (RequestDuration.WithLabels("PUT", "api/Consultas/Update").NewTimer())
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Trim();

                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    token = token["Bearer ".Length..].Trim();

                if (string.IsNullOrEmpty(consulta_Update.IdConsulta))
                {
                    RequestCounter.WithLabels("PUT", "api/Consultas/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O ID da Consulta é obrigatório!");
                }

                if (string.IsNullOrEmpty(consulta_Update.Status))
                {
                    RequestCounter.WithLabels("PUT", "api/Consultas/Update", BadRequest().StatusCode.ToString()).Inc();

                    return BadRequest("O Status da Consulta é obrigatório!");
                }

                if (consulta_Update.Status.ToLower().Trim().Equals("agendada"))
                {
                    if (!_jwtService.ValidarToken_Login(token, "1"))
                    {
                        RequestCounter.WithLabels("PUT", "api/Consultas/Update", Unauthorized().StatusCode.ToString()).Inc();

                        return Unauthorized("É necessário estar logado como Médico para realizar esta alteração!");
                    }
                }
                else if (consulta_Update.Status.ToLower().Trim().Equals("recusada"))
                {
                    if (!_jwtService.ValidarToken_Login(token, "1"))
                    {
                        RequestCounter.WithLabels("PUT", "api/Consultas/Update", Unauthorized().StatusCode.ToString()).Inc();

                        return Unauthorized("É necessário estar logado como Médico para realizar esta alteração!");
                    }
                }
                else if (consulta_Update.Status.ToLower().Trim().Equals("cancelada"))
                {
                    if (string.IsNullOrEmpty(consulta_Update.MotivoCancelamento))
                    {
                        RequestCounter.WithLabels("PUT", "api/Consultas/Update", BadRequest().StatusCode.ToString()).Inc();

                        return BadRequest("O Motivo do Cancelamento é obrigatório!");
                    }

                    if (!_jwtService.ValidarToken_Login(token, "2"))
                    {
                        RequestCounter.WithLabels("PUT", "api/Consultas/Update", Unauthorized().StatusCode.ToString()).Inc();

                        return Unauthorized("É necessário estar logado como Paciente para realizar esta alteração!");
                    }
                }
                else
                    return Unauthorized("O Status da Consulta deve ser uma das opções válidas: 'Agendada', Recusada', 'Cancelada'!");

                using (var channel = factory.CreateConnection().CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "Consultas_UpdateQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "Consultas_UpdateQueue",
                        basicProperties: null,
                        body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(consulta_Update)));
                }

                RequestCounter.WithLabels("PUT", "api/Consultas/Update", Ok().StatusCode.ToString()).Inc();

                return Ok(consulta_Update.IdConsulta);
            }
        }

        #endregion
    }
}