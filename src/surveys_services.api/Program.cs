using Microsoft.EntityFrameworkCore;
using surveys_services.application.Commands.Commands;
using surveys_services.application.Interfaces;
using surveys_services.application.Queries.Handlers;
using surveys_services.application.Queries.Queries;
using surveys_services.domain.Interfaces;
using surveys_services.infrastructure.Persistence.Context;
using surveys_services.infrastructure.Persistence.Repositories;
using surveys_services.infrastructure.Services;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// crear variable para la cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("ConnectionPostgre"); //ConnectionPostgre es el parametro de conexion que creamos en el appsetting
//registrar servicio para la conexion


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly("surveys_services.infrastructure")));

builder.Services.AddHttpClient<EventosService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/api/Eventos/");
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterAnswerCommand).Assembly));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetPendingSurveysByUserQuery).Assembly));

builder.Services.AddScoped<IAnswerRepository, AnswerRepositoryPostgres>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepositoryPostgres>();
builder.Services.AddScoped<ISurveysRepository, SurveysRepositoryPostgres>();
builder.Services.AddScoped<IEventosService, EventosService>(); 
builder.Services.AddScoped<IPagosService, PagosService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Obtiene el DbContext
        var context = services.GetRequiredService<AppDbContext>();

        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones a la base de datos.");
    }
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost3000");


app.UseAuthorization();

app.MapControllers();

app.Run();
