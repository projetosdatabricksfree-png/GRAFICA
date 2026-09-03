using Crm.Application.Interfaces;
using Crm.Application.Services;
using Crm.Application.Validators;
using Crm.Infrastructure.Data;
using Crm.Infrastructure.Pdf;
using Crm.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCrmInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") 
            ?? "Host=localhost;Port=5432;Database=crm_db;Username=ags_user;Password=ags_pass";

        services.AddDbContext<CrmDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(CrmDbContext).Assembly.FullName);
            });
        });

        // Repositórios
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPropostaRepository, PropostaRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Serviços da Aplicação
        services.AddScoped<IPropostaService, PropostaService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IDashboardService, DashboardService>();

        // Motor de PDF QuestPDF
        services.AddScoped<IPdfService, PdfService>();

        // Validadores
        services.AddValidatorsFromAssemblyContaining<CriarPropostaRequestValidator>();

        return services;
    }
}
