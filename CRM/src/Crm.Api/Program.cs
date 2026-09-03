using Crm.Infrastructure;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Injeção da infraestrutura do CRM (EF Core 10, Npgsql, Repositories, Services, QuestPDF)
builder.Services.AddCrmInfrastructure(builder.Configuration);

// 2. Controladores e Formatação JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// 3. Documentação OpenAPI nativa do .NET 10
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Total-Count");
    });
});

var app = builder.Build();

// 5. Inicialização e Migrações / Seed Automático
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<CrmDbContext>();

    try
    {
        logger.LogInformation("Verificando migrações e banco de dados...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Populando dados iniciais com base no modelo real Inprima...");
        await CrmDataSeeder.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro na migração ou seed do banco de dados.");
    }
}

// 6. Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
