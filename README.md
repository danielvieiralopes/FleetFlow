FleetFlow - API de Gestão de Veículos
API RESTful construída com .NET 8 e C# para o desafio técnico de backend. A solução permite o gerenciamento completo do ciclo de vida de veículos e seus documentos, utilizando uma arquitetura robusta, escalável e orientada a microsserviços.

🚀 Como Executar o Projeto
Pré-requisito: Ter o Docker Desktop instalado e em execução.

Clone este repositório para a sua máquina local.

Abra um terminal na pasta raiz do projeto (onde se encontra o ficheiro docker-compose.yml).

Execute o seguinte comando:

docker-compose up --build

Aguarde até que todos os serviços (API, Banco de Dados, RabbitMQ, MinIO) sejam construídos e iniciados.

Acessando os Serviços
API (Swagger UI): http://localhost:8080/swagger

RabbitMQ Management: http://localhost:15672 (login: guest / guest)

MinIO Console: http://localhost:9001 (login: minioadmin / minioadmin)

🧪 Como Executar os Testes
Para executar os testes unitários e de integração, utilize o seguinte comando na pasta raiz do projeto:

dotnet test

Nota: Os testes de integração utilizam Testcontainers e requerem que o Docker Desktop esteja em execução.

🏛️ Arquitetura e Decisões de Design
A solução foi desenvolvida utilizando Clean Architecture para garantir um sistema com baixo acoplamento, alta testabilidade e separação de responsabilidades.

Domain: Contém as entidades de negócio e regras puras.

Application: Orquestra os casos de uso, utilizando o padrão CQRS com a biblioteca MediatR.

Infrastructure: Contém os detalhes de implementação (acesso a dados com EF Core, serviços de mensageria, etc.).

API: O ponto de entrada da aplicação, expondo os endpoints RESTful.

O upload de ficheiros foi implementado de forma assíncrona utilizando o padrão Pre-signed URL, o que garante alta performance e escalabilidade, desacoplando a API do processo de transferência de ficheiros.

🛠️ Tecnologias Utilizadas
Categoria

Tecnologia/Ferramenta

Linguagem & Framework

C# e .NET 8

Banco de Dados

PostgreSQL (orquestrado com Docker)

ORM

Entity Framework Core

Mensageria

RabbitMQ (orquestrado com Docker)

Storage de Ficheiros

MinIO (orquestrado com Docker)

Testes

xUnit, Moq, FluentAssertions, Testcontainers

Containerização

Docker e Docker Compose

✅ Desafio Concluído
Este projeto cumpre todos os requisitos funcionais e não funcionais do desafio, incluindo a implementação de testes unitários e de integração, e a utilização de uma arquitetura limpa e escalável.