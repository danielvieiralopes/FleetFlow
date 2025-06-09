# 🚗 FleetFlow - API de Gestão de Veículos

API RESTful construída com .NET 8 e C# para o desafio técnico de backend.  
A solução permite o gerenciamento completo do ciclo de vida de veículos e seus documentos, utilizando uma arquitetura robusta, escalável e orientada a microsserviços.

---

## 🚀 Como Executar o Projeto

**Pré-requisitos:**  
- [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado e em execução.

**Passos:**

1. Clone este repositório:
   ```bash
   git clone https://github.com/seu-usuario/FleetFlow.git
   cd FleetFlow
   ```

2. Execute o Docker Compose:
   ```bash
   docker-compose up --build
   ```

3. Aguarde até que todos os serviços (API, Banco de Dados, RabbitMQ, MinIO) sejam construídos e iniciados.

---

## 🌐 Acessando os Serviços

| Serviço              | URL                          | Credenciais                    |
|----------------------|-------------------------------|--------------------------------|
| **Swagger UI**       | [http://localhost:8080/swagger](http://localhost:8080/swagger) | — |
| **RabbitMQ**         | [http://localhost:15672](http://localhost:15672)           | `guest / guest`               |
| **MinIO Console**    | [http://localhost:9001](http://localhost:9001)             | `minioadmin / minioadmin`     |

---

## 🧪 Como Executar os Testes

Execute os testes unitários e de integração com:

```bash
dotnet test
```

> ⚠️ Os testes de integração utilizam [Testcontainers](https://dotnet.testcontainers.org/) e requerem que o Docker Desktop esteja em execução.

---

## 🏛️ Arquitetura e Decisões de Design

A solução adota **Clean Architecture**, garantindo:

- Baixo acoplamento
- Alta testabilidade
- Separação clara de responsabilidades

### 📁 Estrutura de Pastas

- **Domain**: Entidades de negócio e regras puras
- **Application**: Casos de uso e lógica de aplicação (CQRS com MediatR)
- **Infrastructure**: Implementações técnicas (EF Core, mensageria, etc.)
- **API**: Ponto de entrada da aplicação (endpoints RESTful)

> 💡 O upload de ficheiros é assíncrono utilizando **Pre-signed URL**, garantindo alta performance e escalabilidade.

---

## 🛠️ Tecnologias Utilizadas

| Categoria             | Tecnologia/Ferramenta           |
|-----------------------|----------------------------------|
| **Linguagem & Framework** | C# e .NET 8                    |
| **Banco de Dados**    | PostgreSQL (via Docker)          |
| **ORM**               | Entity Framework Core            |
| **Mensageria**        | RabbitMQ                         |
| **Storage de Ficheiros** | MinIO                        |
| **Testes**            | xUnit, Moq, FluentAssertions, Testcontainers |
| **Containerização**   | Docker, Docker Compose           |

---

## ✅ Desafio Concluído

Este projeto cumpre todos os **requisitos funcionais e não funcionais** do desafio, incluindo:

- Implementação de testes unitários e de integração
- Upload assíncrono com URL assinada
- Utilização de Clean Architecture
- Estrutura escalável e de fácil manutenção

---
