using GerenciarAssinaturas.Application.Interfaces;
using GerenciarAssinaturas.Application.Services;
using GerenciarAssinaturas.Domain.Exceptions;
using GerenciarAssinaturas.Domain.Interfaces;
using GerenciarAssinaturas.Infrastructure.Data;
using GerenciarAssinaturas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "GerenciarAssinaturas API", Version = "v1" });
});

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injeção de dependência
builder.Services.AddScoped<IAssinanteRepository, AssinanteRepository>();
builder.Services.AddScoped<IAssinanteService, AssinanteService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware global para tratar DomainException → 400 Bad Request
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DomainException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { erro = ex.Message });
    }
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
