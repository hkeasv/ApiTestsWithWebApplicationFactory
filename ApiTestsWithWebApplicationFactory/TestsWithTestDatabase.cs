using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Data;
using TodoApi.Models;

namespace ApiTestsWithWebApplicationFactory;

/* This test class uses a customized WebApplicationFactory (CustomWebApplicationFactory) to create a test server for
   the TodoApi. The customized WebApplicationFactory creates a test database. */

public class TestsWithTestDatabase : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public TestsWithTestDatabase(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        
        // Re-seed the test database (happens for each test because a new test class instance is created for each test)  
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TodoContext>();
            
            // Seed the database with test data.
            List<TodoItem> items = new List<TodoItem>
            {
                new TodoItem {Id=1, IsComplete=true, Name="Use SqLite"},
                new TodoItem {Id=2, IsComplete=false, Name="Exam project"}
            };
            
            db.TodoItems.AddRange(items);
            db.SaveChanges();
        }
    }
    
    public void Dispose()
    {
        // Delete the records in the test database (happens for each test because the test class instance is
        // disposed after each test)
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;

            var db = scopedServices.GetRequiredService<TodoContext>();
            db.TodoItems.RemoveRange(db.TodoItems);
            db.SaveChanges();
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

}