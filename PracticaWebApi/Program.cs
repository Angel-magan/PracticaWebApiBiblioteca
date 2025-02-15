using Microsoft.EntityFrameworkCore;
using PracticaWebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Inyección por dependencias del string de conexión al contexto
builder.Services.AddDbContext<bibliotecaContext>(options =>
        options.UseSqlServer(
                builder.Configuration.GetConnectionString("bibliotecaDbConnection") //Este es el nombre de la conexión
            )
        );
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
