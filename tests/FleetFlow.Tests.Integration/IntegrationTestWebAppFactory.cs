using FleetFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
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
            // Remove a configuração do DbContext original da API.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FleetFlowDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Adiciona um novo DbContext que usa a connection string do nosso contêiner de teste.
            services.AddDbContext<FleetFlowDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
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