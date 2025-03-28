# Arquitetura do Projeto

## Visão Geral
Este repositório contém o sistema baseado em microsserviços desenvolvido em C#. O sistema é responsável pelo cadastro e gerenciamento de médicos e pacientes, utilizando Kubernetes para orquestração dos containers e RabbitMQ para comunicação assíncrona entre os serviços.

## Componentes Principais
### Microsserviços
O sistema é dividido nos seguintes microsserviços:

- **API Gateway**: Encaminha as requisições para os microsserviços corretos.
- **API de Consulta**: Responsável por executar as Consultas de informações ao banco de dados e devolver diretamente à Gateway.
- **Serviço de Registro**: Gerencia o Registro de novas informações no banco de dados.
- **Serviço de Atualização**: Gerencia a Atualização das informações no banco de dados.

### Banco de Dados
Com exceção da API Gateway, todos os Serviços possuem acesso ao banco de dados, para executarem individualmente suas funções específicas.
O banco de dados utilizado é o MongoDB.

### Comunicação entre Microsserviços
- **Síncrona (HTTP REST)**: Utilizada entre a API Gateway e a API de Consulta.
- **Assíncrona (RabbitMQ)**: Utilizada para as operações entre a API Gateway e os Serviços de Registro e Atualização.

## Infraestrutura com Kubernetes
Os microsserviços são implantados em um cluster AKS (Azure Kubernetes Service), seguindo esta estrutura:

- **Pods**: Cada microsserviço roda em um Pod separado.
- **Services**: Criados para cada microsserviço, garantindo comunicação interna.
- **ConfigMaps & Secrets**: Utilizados para armazenar configurações externas e credenciais.
- **Deployments & ReplicaSets**: Gerenciam a escalabilidade e atualizações dos microsserviços.

## Monitoramento e Logs
- **Prometheus & Grafana**: Utilizados para monitorar métricas como uso de CPU, memória e requisições.

## Pipeline CI/CD
A pipeline do GitHub Actions gerencia o build, testes e deploy:

1. **Build**: Compila o código e executa testes unitários.
2. **Containerização**: Gera imagens Docker para cada microsserviço.
3. **Publicação**: Envia as imagens para um registry no Docker Hub.
4. **Deploy no Kubernetes**: Aplica os manifests para atualizar os serviços.

## Como Executar o Projeto
### Pré-requisitos
- Cluster AKS configurado e rodando
- GitHub Actions configurado (para CI/CD, com as Variables e Secrets no repositório)
- Será necessário adicionar um Secret no repositório do GitHub, incluindo o arquivo de configuração do cluster AKS (.kube\config)

### Passos
1. Clone este repositório:
   ```sh
   git clone https://github.com/Gabriel-Llanir/Hackathon.git
   ```
2. Configure os arquivos de ambiente e secrets necessários.
3. Crie o cluster AKS e inclua sua configuração dentro do repositóro do GitHub
4. Se necessário altere os manifests e services dos Serviços para ajustar configurações mais específicas.

## Licença
Este projeto está licenciado sob a MIT License.

