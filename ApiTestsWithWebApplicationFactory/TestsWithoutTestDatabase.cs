using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ApiTestsWithWebApplicationFactory;

/* This test class uses WebApplicationFactory to create a test server for the TodoApi application and tests its
   endpoints. No test database is created or initialized. In a real-world scenario, a test database should be 
   set up and initialized. This is demonstrated with the CustomWebApplicationFactory class in the 
   CustomWebApplicationFactory.cs file. */

public class TestsWithoutTestDatabase : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public TestsWithoutTestDatabase(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_EndpointsReturnSuccess()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/todo");

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
}