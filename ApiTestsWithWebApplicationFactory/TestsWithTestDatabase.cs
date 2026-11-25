using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Data;
using TodoApi.Models;

namespace ApiTestsWithWebApplicationFactory;

/* This test class uses a customized WebApplicationFactory (CustomWebApplicationFactory) to create a test server for
   the TodoApi. The customized WebApplicationFactory creates a test database. */

public class TestsWithTestDatabase : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    
    public TestsWithTestDatabase(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        
        // Seed the database with test data if the database is empty.
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TodoContext>();
            
            if (!db.TodoItems.Any())
            {
                List<TodoItem> items = new List<TodoItem>
                {
                    new TodoItem {Id=1, IsComplete=true, Name="Use SqLite"},
                    new TodoItem {Id=2, IsComplete=false, Name="Exam project"}
                };
                db.TodoItems.AddRange(items);
                db.SaveChanges();
            }
        }
    }
    
    [Fact]
    public async Task Get_All_EndpointReturnsSuccess()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/todo");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var items = System.Text.Json.JsonSerializer.Deserialize<List<TodoItem>>(content);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task Get_Item2_EndpointReturnsSuccess()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/todo/2");

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Get_Item3_EndpointReturnsNotFound()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/todo/3");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_Item_EndpointReturnsSuccess()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        var newItem = new TodoItem {IsComplete = false, Name = "Test"};
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(
            newItem), System.Text.Encoding.UTF8, "application/json");
        
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var db = scopedServices.GetRequiredService<TodoContext>();
            
            // Begin a transaction so that the database changes can be rolled back after the test is complete.
            var transaction = await db.Database.BeginTransactionAsync();
    
            // Act
            var response = await client.PostAsync("/todo", content);

            // Assert
            response.EnsureSuccessStatusCode();
            
            // Rollback the transaction
            await transaction.RollbackAsync();
        }
    }

}