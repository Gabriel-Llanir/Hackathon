// DataBase do Hackathon
db = db.getSiblingDB("Hackathon");


// Criação da Collection de Médicos
db.createCollection('tbl_Medicos', {
  validator: {
    $jsonSchema: {
      bsonType: 'object',
      required: ['sNome', 'sCPF', 'sCRM', 'sEmail', 'sSenha', 'nValorConsulta', 'idEspecialidade', 'sAtivo'],
      properties: {
        sNome: {
          bsonType: 'string',
          description: 'O nome do médico é obrigatório e deve ser uma string.'
        },
        sCPF: {
          bsonType: 'string',
          pattern: '^\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}$',
          description: 'O CPF do médico é obrigatório e deve ser uma string.'
        },
        sCRM: {
          bsonType: 'string',
          description: 'O CRM do médico é obrigatório e deve ser uma string.'
        },
        sEmail: {
          bsonType: 'string',
          pattern: '^([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,})$',
          description: 'O email do médico é obrigatório e deve ser uma string.'
        },
        sSenha: {
          bsonType: 'string',
          description: 'A senha do médico é obrigatória e deve ser uma string.'
        },
		idEspecialidade: {
			"bsonType": "int",
			"description": "A especialidade é obrigatória e deve ser um número inteiro entre 1 e 11",
			"minimum": 1,
			"maximum": 11
		},
		"aDisponibilidades": {
		  "bsonType": "array",
		  "items": {
		    "bsonType": "object",
		    "description": "A disponibilidade do médico deve ser uma lista de dias da semana e intervalos de horário.",
		    "required": ["nDia", "aHorarios"],
		    "properties": {
			  "nDia": {
			    "bsonType": "int",
			    "description": "O dia da semana (0 = Domingo, 1 = Segunda-feira...) é obrigatório e deve ser um número inteiro entre 0 e 6.",
			    "minimum": 0,
			    "maximum": 6
			  },
			  "aHorarios": {
			    "bsonType": "array",
			    "description": "A lista de horários disponíveis no formato de intervalos de tempo (inicio e fim).",
			    "items": {
				  "bsonType": "object",
				  "required": ["inicio", "fim"],
				  "properties": {
				    "inicio": {
					  "bsonType": "string",
					  "pattern": "^\\d{2}:\\d{2}:\\d{2}$",
					  "description": "O horário de início no formato HH:MM:SS."
				    },
				    "fim": {
					  "bsonType": "string",
					  "pattern": "^\\d{2}:\\d{2}:\\d{2}$",
					  "description": "O horário de término no formato HH:MM:SS."
				    }
				  }
			    }
			  }
		    }
		  }
		},
		nValorConsulta: {
		  bsonType: 'decimal',
		  description: 'O valor da consulta é obrigatório e deve ser um valor decimal válido.'
        },
        sAtivo: {
		  bsonType: 'string',
		  enum: ['S', 'N'],
		  description: 'O valor de Ativo deve ser "S" para ativo ou "N" para inativo.'
        }
      }
    }
  },
  validationLevel: 'strict',
  validationAction: 'error'
});


// Criação da Collection de Pacientes
db.createCollection('tbl_Pacientes', {
  validator: {
    $jsonSchema: {
      bsonType: 'object',
      required: ['sNome', 'sCPF', 'sEmail', 'sSenha', 'sAtivo'],
      properties: {
        sNome: {
          bsonType: 'string',
          description: 'O nome do paciente é obrigatório e deve ser uma string.'
        },
        sCPF: {
          bsonType: 'string',
          pattern: '^\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}$',
          description: 'O CPF do paciente é obrigatório e deve ser uma string.'
        },
        sEmail: {
          bsonType: 'string',
          pattern: '^([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,})$',
          description: 'O email do paciente é obrigatório e deve ser uma string.'
        },
        sSenha: {
          bsonType: 'string',
          description: 'A senha do paciente é obrigatória e deve ser uma string.'
        },
        sAtivo: {
		  bsonType: 'string',
		  enum: ['S', 'N'],
		  description: 'O valor de Ativo deve ser "S" para ativo ou "N" para inativo.'
        }
      }
    }
  },
  validationLevel: 'strict',
  validationAction: 'error'
});


// Criação da Collection de Consultas
db.createCollection('tbl_Consultas', {
  validator: {
    $jsonSchema: {
      bsonType: 'object',
      required: ['idMedico', 'idPaciente', 'dtAgendada', 'nTotalConsulta', 'sStatus'],
      properties: {
        idMedico: {
          bsonType: 'objectId',
          description: 'O ID do médico é obrigatório e deve ser uma referência ao médico na collection tbl_Medicos.'
        },
        idPaciente: {
          bsonType: 'objectId',
          description: 'O ID do paciente é obrigatório e deve ser uma referência ao paciente na collection tbl_Pacientes.'
        },
        dtAgendada: {
          bsonType: 'string',
		  pattern: '^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/\\d{4} (0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$',
          description: 'A data e hora da consulta são obrigatórias.'
        },
        nTotalConsulta: {
          bsonType: 'decimal',
          description: 'O valor total da consulta é obrigatório e deve ser um valor decimal válido.'
        },
        sStatus: {
          bsonType: 'string',
          enum: ['Agendada', 'Em processamento', 'Recusada', 'Cancelada'],
          description: 'O valor de Ativo deve ser "S" para ativo ou "N" para inativo.'
        },
        sMotivoCancelamento: {
		  bsonType: ['string', 'null'],
          description: 'O motivo do cancelamento deve ser uma string, e apenas é obrigatório caso o Status seja "Cancelada".'
        }
      }
    }
  },
  validationLevel: 'strict',
  validationAction: 'error'
});


// Criação de índices
db.tbl_Medicos.createIndex({ sCPF: 1 }, { unique: true });
db.tbl_Medicos.createIndex({ sCRM: 1 }, { unique: true });
db.tbl_Medicos.createIndex({ sEmail: 1 }, { unique: true });

db.tbl_Pacientes.createIndex({ sCPF: 1 }, { unique: true });
db.tbl_Pacientes.createIndex({ sEmail: 1 }, { unique: true });

db.tbl_Consultas.createIndex({ idMedico: 1 });
db.tbl_Consultas.createIndex({ idPaciente: 1 });