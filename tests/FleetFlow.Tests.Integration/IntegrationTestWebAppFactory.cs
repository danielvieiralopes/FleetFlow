using FleetFlow.Api.Workers;
using FleetFlow.Application.Interfaces;
using FleetFlow.Infrastructure.Persistence;
using FleetFlow.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using RabbitMQ.Client;
using Testcontainers.PostgreSql;

namespace FleetFlow.Tests.Integration;

/// <summary>
/// Fábrica customizada para inicializar a aplicação em um ambiente de teste de integração.
/// Esta classe gerencia um contêiner Docker do PostgreSQL para garantir um banco de dados limpo e isolado para os testes.
/// </summary>
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Cria um contêiner PostgreSQL usando Testcontainers.
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("test_db")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    /// <summary>
    /// Configura os serviços da aplicação para os testes,
    /// substituindo a configuração do banco de dados original pela do contêiner de teste.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // --- 1. REMOVER DEPENDÊNCIAS EXTERNAS E CONFIGURAÇÕES REAIS ---
            services.RemoveAll(typeof(DbContextOptions<FleetFlowDbContext>));
            services.RemoveAll(typeof(IConnection));
            services.RemoveAll(typeof(IStorageService));
            services.RemoveAll(typeof(IMessageBus));
            services.RemoveAll(typeof(IHostedService));

            // Remove a configuração de autenticação JWT original.
            services.RemoveAll(typeof(IAuthenticationService));
            services.RemoveAll(typeof(IAuthenticationHandler));

            // --- 2. ADICIONAR SERVIÇOS DE TESTE ---
            // Adiciona um DbContext que usa o contêiner de teste.
            services.AddDbContext<FleetFlowDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            // Adiciona mocks para os serviços que não queremos testar.
            services.AddSingleton(new Mock<IStorageService>().Object);
            services.AddSingleton(new Mock<IMessageBus>().Object);

            // Adiciona um manipulador de autenticação falso e DEFINE-O COMO PADRÃO.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
                options.DefaultScheme = "TestScheme";
            })
                .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("TestScheme", _ => { });


            // --- 3. PREPARAR O BANCO DE DADOS DE TESTE ---
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FleetFlowDbContext>();
            dbContext.Database.Migrate();
        });
    }

    /// <summary>
    /// IAsyncLifetime: Executado antes de qualquer teste na classe.
    /// Inicia o contêiner do banco de dados.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    /// <summary>
    /// IAsyncLifetime: Executado depois de todos os testes na classe.
    /// Para e destrói o contêiner do banco de dados.
    /// </summary>
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
