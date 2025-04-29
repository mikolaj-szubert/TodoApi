using Microsoft.AspNetCore.Hosting;
using TodoApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TodoApi.Tests;
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private const string TestDbName = "todos_test";
    private static bool _databaseInitialized;
    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //Usuwa istniejącą konfigurację
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            //Tworzy bazę jeśli nie istnieje
            CreateTestDatabase();

            //Konfiguracja połączenia
            services.AddDbContext<TodoDbContext>(options =>
                options.UseNpgsql(GetConnectionString()));
        });

        builder.UseEnvironment("Development");
    }

    private static void CreateTestDatabase()
    {
        if (_databaseInitialized) return;

        using var connection = new NpgsqlConnection(
            "Host=localhost;Username=postgres;Password=postgrespw");

        connection.Open();

        //Tworzy bazę jeśli nie istnieje
        using var cmd = new NpgsqlCommand(
            $"CREATE DATABASE {TestDbName} WITH OWNER = postgres", connection);
        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (PostgresException ex) when (ex.SqlState == "42P04")
        {
            //Baza już istnieje
        }

        //Zastosowanie migracji
        using var context = new TodoDbContext(
            new DbContextOptionsBuilder<TodoDbContext>()
                .UseNpgsql(GetConnectionString())
                .Options);

        context.Database.Migrate();
        _databaseInitialized = true;
    }

    private static string GetConnectionString() =>
        $"Host=localhost;Database={TestDbName};Username=postgres;Password=postgrespw";

    protected override void Dispose(bool disposing)
    {
        //Czyści bazę po testach
        using var context = new TodoDbContext(
            new DbContextOptionsBuilder<TodoDbContext>()
                .UseNpgsql(GetConnectionString())
                .Options);

        context.Database.EnsureDeleted();
        base.Dispose(disposing);
    }
}
