using Moq;
using MongoDB.Driver;
using Consulta_Data = Consulta.Data.DataContext;
using Consulta_Models_Consulta = Consulta.Models.Consulta;
using Consulta_Models_Medico = Consulta.Models.Medico;
using Consulta_Models_Paciente = Consulta.Models.Paciente;
using Consulta_Repository = Consulta.Repositories.ConsultaRepository;
using Register_IEmail = RegisterConsumer.Email.IEmail;
using Register_Data = RegisterConsumer.Data.DataContext;
using Register_Models_Consulta = RegisterConsumer.Models.Consulta;
using Register_Models_Medico = RegisterConsumer.Models.Medico;
using Register_Models_Paciente = RegisterConsumer.Models.Paciente;
using Register_Repository = RegisterConsumer.Repositories.RegisterRepository;
using Update_IEmail = UpdateConsumer.Email.IEmail;
using Update_Data = UpdateConsumer.Data.DataContext;
using Update_Models_Consulta = UpdateConsumer.Models.Consulta;
using Update_Models_ConsultaUpdate = UpdateConsumer.Models.Consulta_Update;
using Update_Models_Medico = UpdateConsumer.Models.Medico;
using Update_Models_Paciente = UpdateConsumer.Models.Paciente;
using Update_Repository = UpdateConsumer.Repositories.UpdateRepository;

namespace UnitTests.Tests
{
    public class ConsultaRepositoryTests
    {
        private readonly Mock<Consulta_Data> _mockContext;
        private readonly Consulta_Repository _repository;

        public ConsultaRepositoryTests()
        {
            _mockContext = new Mock<Consulta_Data>();
            _repository = new Consulta_Repository(_mockContext.Object);
        }

        [Fact]
        public async Task Get_MedicosDisponiveis_ReturnsMedicos_WhenIdIsProvided()
        {
            var medicoId = "123";
            var medicos = new List<Consulta_Models_Medico> { new Consulta_Models_Medico { IdMedico = medicoId, Ativo = "S" } };
            var mockCollection = new Mock<IMongoCollection<Consulta_Models_Medico>>();
            var mockCursor = new Mock<IAsyncCursor<Consulta_Models_Medico>>();
            mockCursor.Setup(_ => _.Current).Returns(medicos);
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Consulta_Models_Medico>>(), null, default))
                .ReturnsAsync(mockCursor.Object);
            _mockContext.Setup(c => c.Medicos).Returns(mockCollection.Object);

            var result = await _repository.Get_MedicosDisponiveis(medicoId, null, null);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(medicoId, result.First().IdMedico);
        }

        [Fact]
        public async Task Get_Consultas_ReturnsConsultas_WhenIdAndIdTipoAreProvided()
        {
            var consultas = new List<Consulta_Models_Consulta>
            {
                new Consulta_Models_Consulta { IdConsulta = "1", IdMedico = "123", IdPaciente = "456", DtAgendada = "01/01/2023 10:00:00", TotalConsulta = 100, Status = "Agendada" }
            };
            var medicos = new List<Consulta_Models_Medico> { new Consulta_Models_Medico { IdMedico = "123", Nome = "Dr. Teste" } };
            var pacientes = new List<Consulta_Models_Paciente> { new Consulta_Models_Paciente { IdPaciente = "456", Nome = "Paciente Teste" } };

            var mockConsultasCollection = new Mock<IMongoCollection<Consulta_Models_Consulta>>();
            var mockMedicosCollection = new Mock<IMongoCollection<Consulta_Models_Medico>>();
            var mockPacientesCollection = new Mock<IMongoCollection<Consulta_Models_Paciente>>();

            var mockConsultasCursor = new Mock<IAsyncCursor<Consulta_Models_Consulta>>();
            var mockMedicosCursor = new Mock<IAsyncCursor<Consulta_Models_Medico>>();
            var mockPacientesCursor = new Mock<IAsyncCursor<Consulta_Models_Paciente>>();

            mockConsultasCursor.Setup(_ => _.Current).Returns(consultas);
            mockConsultasCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            mockMedicosCursor.Setup(_ => _.Current).Returns(medicos);
            mockMedicosCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            mockPacientesCursor.Setup(_ => _.Current).Returns(pacientes);
            mockPacientesCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            mockConsultasCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Consulta_Models_Consulta>>(), null, default))
                .ReturnsAsync(mockConsultasCursor.Object);
            mockMedicosCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Consulta_Models_Medico>>(), null, default))
                .ReturnsAsync(mockMedicosCursor.Object);
            mockPacientesCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Consulta_Models_Paciente>>(), null, default))
                .ReturnsAsync(mockPacientesCursor.Object);

            _mockContext.Setup(c => c.Consultas).Returns(mockConsultasCollection.Object);
            _mockContext.Setup(c => c.Medicos).Returns(mockMedicosCollection.Object);
            _mockContext.Setup(c => c.Pacientes).Returns(mockPacientesCollection.Object);

            var result = await _repository.Get_Consultas("123", 1);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("1", result.First().IdConsulta);
        }

        [Fact]
        public async Task Get_ValorConsulta_Medico_ReturnsValorConsulta_WhenIdIsProvided()
        {
            var medicoId = "123";
            var medico = new Consulta_Models_Medico { IdMedico = medicoId, ValorConsulta = 100 };
            var mockCollection = new Mock<IMongoCollection<Consulta_Models_Medico>>();
            var mockCursor = new Mock<IAsyncCursor<Consulta_Models_Medico>>();
            mockCursor.Setup(_ => _.Current).Returns(new List<Consulta_Models_Medico> { medico });
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<System.Threading.CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Consulta_Models_Medico>>(), null, default))
                .ReturnsAsync(mockCursor.Object);
            _mockContext.Setup(c => c.Medicos).Returns(mockCollection.Object);

            var result = await _repository.Get_ValorConsulta_Medico(medicoId);

            Assert.NotNull(result);
            Assert.Equal("100", result);
        }

        [Fact]
        public async Task Get_LoginMedico_ReturnsMedico_WhenCredentialsAreValid()
        {
            var crm = "123";
            var senha = "senha";
            var hashedSenha = BCrypt.Net.BCrypt.HashPassword(senha, 10);
            var medico = new Consulta_Models_Medico { CRM = crm, Senha = hashedSenha, Ativo = "S" };
            var mockCollection = new Mock<IMongoCollection<Consulta_Models_Medico>>();
            var mockCursor = new Mock<IAsyncCursor<Consulta_Models_Medico>>();
            mockCursor.Setup(_ => _.Current).Returns(new List<Consulta_Models_Medico> { medico });
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Consulta_Models_Medico>>(), null, default))
                .ReturnsAsync(mockCursor.Object);
            _mockContext.Setup(c => c.Medicos).Returns(mockCollection.Object);

            var result = await _repository.Get_LoginMedico(crm, senha);

            Assert.NotNull(result);
            Assert.Equal(crm, result.CRM);
        }

        [Fact]
        public async Task Get_LoginPaciente_ReturnsPaciente_WhenCredentialsAreValid()
        {
            var login = "email@teste.com";
            var senha = "senha";
            var hashedSenha = BCrypt.Net.BCrypt.HashPassword(senha, 10);
            var paciente = new Consulta_Models_Paciente { Email = login, Senha = hashedSenha, Ativo = "S" };
            var mockCollection = new Mock<IMongoCollection<Consulta_Models_Paciente>>();
            var mockCursor = new Mock<IAsyncCursor<Consulta_Models_Paciente>>();
            mockCursor.Setup(_ => _.Current).Returns(new List<Consulta_Models_Paciente> { paciente });
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Consulta_Models_Paciente>>(), null, default))
                .ReturnsAsync(mockCursor.Object);
            _mockContext.Setup(c => c.Pacientes).Returns(mockCollection.Object);

            var result = await _repository.Get_LoginPaciente(login, senha);

            Assert.NotNull(result);
            Assert.Equal(login, result.Email);
        }
    }

    public class RegisterRepositoryTests
    {
        private readonly Mock<Register_IEmail> _mockEmail;
        private readonly Mock<Register_Data> _mockContext;
        private readonly Register_Repository _repository;

        public RegisterRepositoryTests()
        {
            _mockEmail = new Mock<Register_IEmail>();
            _mockContext = new Mock<Register_Data>();
            _repository = new Register_Repository(_mockEmail.Object, _mockContext.Object);
        }

        [Fact]
        public async Task AddAsync_Medico_AddsMedicoToDatabase()
        {
            var medico = new Register_Models_Medico { IdMedico = "123", Nome = "Dr. Teste" };
            var mockCollection = new Mock<IMongoCollection<Register_Models_Medico>>();
            _mockContext.Setup(c => c.Medicos).Returns(mockCollection.Object);

            await _repository.AddAsync(medico);

            mockCollection.Verify(c => c.InsertOneAsync(medico, null, default), Times.Once);
        }

        [Fact]
        public async Task AddAsync_Paciente_AddsPacienteToDatabase()
        {
            var paciente = new Register_Models_Paciente { IdPaciente = "456", Nome = "Paciente Teste" };
            var mockCollection = new Mock<IMongoCollection<Register_Models_Paciente>>();
            _mockContext.Setup(c => c.Pacientes).Returns(mockCollection.Object);

            await _repository.AddAsync(paciente);

            mockCollection.Verify(c => c.InsertOneAsync(paciente, null, default), Times.Once);
        }

        [Fact]
        public async Task AddAsync_Consulta_AddsConsultaToDatabaseAndSendsEmail()
        {
            var consulta = new Register_Models_Consulta { IdConsulta = "789", IdMedico = "123", IdPaciente = "456", DtAgendada = "01/01/2023 10:00:00", TotalConsulta = 100, Status = "Agendada" };
            var mockCollection = new Mock<IMongoCollection<Register_Models_Consulta>>();
            _mockContext.Setup(c => c.Consultas).Returns(mockCollection.Object);

            await _repository.AddAsync(consulta);

            mockCollection.Verify(c => c.InsertOneAsync(consulta, null, default), Times.Once);
            _mockEmail.Verify(e => e.EnviarEmail_Paciente(consulta), Times.Once);
        }
    }

    public class UpdateRepositoryTests
    {
        private readonly Mock<Update_IEmail> _mockEmail;
        private readonly Mock<Update_Data> _mockContext;
        private readonly Update_Repository _repository;

        public UpdateRepositoryTests()
        {
            _mockEmail = new Mock<Update_IEmail>();
            _mockContext = new Mock<Update_Data>();
            _repository = new Update_Repository(_mockEmail.Object, _mockContext.Object);
        }

        [Fact]
        public async Task UpdateAsync_Medico_UpdatesMedicoInDatabase()
        {
            var medico = new Update_Models_Medico { IdMedico = "123", Nome = "Dr. Teste" };
            var mockCollection = new Mock<IMongoCollection<Update_Models_Medico>>();
            _mockContext.Setup(c => c.Medicos).Returns(mockCollection.Object);

            await _repository.UpdateAsync(medico);

            mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Update_Models_Medico>>(), medico, new ReplaceOptions(), default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Paciente_UpdatesPacienteInDatabase()
        {
            var paciente = new Update_Models_Paciente { IdPaciente = "456", Nome = "Paciente Teste" };
            var mockCollection = new Mock<IMongoCollection<Update_Models_Paciente>>();
            _mockContext.Setup(c => c.Pacientes).Returns(mockCollection.Object);

            await _repository.UpdateAsync(paciente);

            mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Update_Models_Paciente>>(), paciente, new ReplaceOptions(), default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Consulta_UpdatesConsultaInDatabaseAndSendsEmail()
        {
            // Arrange
            var consulta = new Update_Models_ConsultaUpdate { IdConsulta = "789", Status = "agendada", DataAgendada = "01/01/2023 10:00:00", MotivoCancelamento = "Motivo" };
            var consultaOriginal = new Update_Models_Consulta { IdConsulta = "789", IdMedico = "123", IdPaciente = "456", DtAgendada = "01/01/2023 10:00:00", TotalConsulta = 100, Status = "Agendada" };
            var mockCollection = new Mock<IMongoCollection<Update_Models_Consulta>>();
            var mockCursor = new Mock<IAsyncCursor<Update_Models_Consulta>>();
            mockCursor.Setup(_ => _.Current).Returns(new List<Update_Models_Consulta> { consultaOriginal });
            mockCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            mockCollection.Setup(c => c.Find(It.IsAny<FilterDefinition<Update_Models_Consulta>>(), null).FirstOrDefaultAsync(default))
                .ReturnsAsync(consultaOriginal);
            _mockContext.Setup(c => c.Consultas).Returns(mockCollection.Object);

            await _repository.UpdateAsync(consulta);

            mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Update_Models_Consulta>>(), It.IsAny<Update_Models_Consulta>(), new ReplaceOptions(), default), Times.Once);
            _mockEmail.Verify(e => e.EnviarEmail_Paciente(It.IsAny<Update_Models_Consulta>()), Times.Once);
        }
    }
}
