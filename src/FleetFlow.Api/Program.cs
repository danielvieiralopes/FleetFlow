using FleetFlow.Api.Workers;
using FleetFlow.Application.Interfaces;
using FleetFlow.Infrastructure.Messaging;
using FleetFlow.Infrastructure.Persistence;
using FleetFlow.Infrastructure.Persistence.Repositories;
using FleetFlow.Infrastructure.Security;
using FleetFlow.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


// --- REGISTO DE SERVIÇOS ---

// Adiciona o DbContext para injeção de dependência.
builder.Services.AddDbContext<FleetFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os clientes para serviços externos (MinIO, RabbitMQ).
builder.Services.AddSingleton<IMinioClient>(sp => new MinioClient()
    .WithEndpoint(builder.Configuration["MinioSettings:Endpoint"])
    .WithCredentials(builder.Configuration["MinioSettings:AccessKey"], builder.Configuration["MinioSettings:SecretKey"])
    .Build());

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration["RabbitMqSettings:Hostname"],
        UserName = builder.Configuration["RabbitMqSettings:Username"],
        Password = builder.Configuration["RabbitMqSettings:Password"]
    };
    return factory.CreateConnection();
});

// Adiciona os serviços de repositório e de aplicação.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStorageService, MinioStorageService>();
builder.Services.AddScoped<IMessageBus, RabbitMqService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Adiciona o worker como um serviço em background.
builder.Services.AddHostedService<DocumentProcessorWorker>();

// Adiciona o MediatR para o padrão CQRS.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(FleetFlow.Application.Interfaces.IUserRepository).Assembly));

// Adiciona a configuração de autenticação JWT.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });

// Adiciona os controllers e configura a serialização JSON.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT aqui."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// --- CONSTRUÇÃO E PIPELINE DA APLICAÇÃO ---

var app = builder.Build();

// Configura o pipeline de middleware HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Popula a base de dados com dados iniciais apenas em desenvolvimento.
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FleetFlowDbContext>();
    await DataSeeder.SeedAsync(dbContext);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Necessário para que o projeto de testes de integração possa referenciar a API.
public partial class Program { }
