using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Models.DTO;

namespace TodoApi.Tests
{
    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        public IntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _factory.ResetDatabase();
        }

        [Fact]
        public async Task GET_All_WhenEmpty_ReturnsEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/api/Todos");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
            Assert.NotNull(content);
            Assert.Empty(content);
        }

        [Fact]
        public async Task GET_All_WhenPopulated_ReturnsAllItems()
        {
            // Arrange
            var testItems = new List<TodoItem>
            {
                new() { Title = "Tytul 1", Description = "Opis 1", DueDateTime = DateTime.UtcNow.AddDays(1) },
                new() { Title = "Tytul 2", Description = "Opis 2", DueDateTime = DateTime.UtcNow.AddDays(2) }
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                await db.TodoItems.AddRangeAsync(testItems);
                await db.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync("/api/Todos");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<List<TodoItem>>();

            Assert.NotNull(content);
            Assert.Equal(testItems.Count, content.Count);
            Assert.Contains(content, x => x.Title == "Tytul 1");
            Assert.Contains(content, x => x.Title == "Tytul 2");
        }

        [Fact]
        public async Task GET_Todo_WhenPopulated_ReturnsOneItem()
        {
            // Arrange
            var testItems = new List<TodoItem>
            {
                new() { Title = "Test 1", Description = "Desc 1", DueDateTime = DateTime.UtcNow.AddDays(1) },
                new() { Title = "Test 2", Description = "Desc 2", DueDateTime = DateTime.UtcNow.AddDays(2) }
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                await db.TodoItems.AddRangeAsync(testItems);
                await db.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync("/api/Todos/1");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<TodoItem>();

            Assert.NotNull(content);
            Assert.Equal("Test 1", content.Title);
        }

        [Fact]
        public async Task POST_Todo_ReturnsCreated()
        {
            // Arrange
            var testItems = new PostTodoItemDto
            {
                Title = "Test 1", Description = "Desc 1", DueDateTime = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Todos", testItems);

            // Assert
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Created, "Z³y kod statusu");
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                var itemInDb = await db.TodoItems.FindAsync(1);
                Assert.NotNull(itemInDb);
                Assert.Equal(itemInDb.Title, testItems.Title);
            }
        }

        [Fact]
        public async Task POST_Todo_ReturnsBadRequest()
        {
            // Arrange
            var testItems = new PostTodoItemDto
            {
                Title = "Test 1", Description = "Desc 1", DueDateTime = DateTime.UtcNow.AddTicks(-1)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Todos", testItems);

            // Assert
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest, "Poprawny kod statusu");
        }

        [Fact]
        public async Task GET_IncomingTodo_ReturnsTodoItem()
        {
            // Arrange
            var testItems = new List<TodoItem>
            {
                new() { Title = "Jutrzejsze zadanie", Description = "Desc 1", DueDateTime = DateTime.UtcNow.AddDays(1) },
                new() { Title = "Zadanie na za 2 dni", Description = "Desc 2", DueDateTime = DateTime.UtcNow.AddDays(2) }
            };

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                await db.TodoItems.AddRangeAsync(testItems);
                await db.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync("/api/Todos/incoming/tomorrow");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
            Assert.NotNull(content);
            Assert.Contains(content, x => x.Title == "Jutrzejsze zadanie");
            Assert.DoesNotContain(content, x => x.Title == "Zadanie na za 2 dni");
        }
    }
}