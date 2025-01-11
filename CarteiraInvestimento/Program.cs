using CarteiraInvestimento.Application.Services;
using CarteiraInvestimento.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CarteiraInvestimento.Domain.Interfaces.IServices;
using OfficeOpenXml;

using LicenseContext = OfficeOpenXml.LicenseContext;
using CarteiraInvestimento.Domain.Interfaces;
using CarteiraInvestimento.Infrastructure.Repositories;
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

// services 
builder.Services.AddTransient<IBlobService, BlobService>();
builder.Services.AddTransient<IExcelService, ExcelService>();
builder.Services.AddTransient<ITickerService, TickerService>();
builder.Services.AddTransient<IMovimentacoesResumoService, MovimentacoesResumoService>();


// repositories
builder.Services.AddTransient<IMovimentacoesRepository, MovimentacoesRepository>();
builder.Services.AddTransient<ITickerRepository, TickerRepository>();
builder.Services.AddTransient<IMovimentacoesResumoRepository, MovimentacoesResumoRepository>();

var connectionString = builder.Configuration.GetValue<string>("DefaultConnection");

// Configuração do banco de dados (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=localhost,1433;Database=CarteiraInvestimentoDb;User Id=sa;Password=P@55w0rd;TrustServerCertificate=True;")
);

builder.Build().Run();
