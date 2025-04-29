using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TodoApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); //Konfiguracja po³¹czenia z baz¹ danych postgresql

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>{
    //Dodanie komentarzy XML do Swaggera
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider; //Tworzenie zakresu us³ug
    var logger = services.GetRequiredService<ILogger<Program>>(); //Pobranie loggera

    for (int i = 0; i < 5; i++)
    {
        try
        {
            logger.LogInformation("Próba dodania migracji (próba {0}/5)", i + 1);
            var db = services.GetRequiredService<TodoDbContext>(); //Pobranie kontekstu bazy danych
            db.Database.Migrate(); //Wykonanie migracji
            logger.LogInformation("Migracje dodane");
            break;
        }
        catch (Npgsql.NpgsqlException ex)
        {
            logger.LogError(ex, "Nieudana próba migracji numer {0}", i + 1);
            if (i == 4) throw; //Wyj¹tek jeœli to ostatnia próba
            await Task.Delay(5000); //Czeka 5 sekund przed kolejn¹ prób¹
        }
    }
}

app.Run();

public partial class Program { }