namespace ApiTestsWithWebApplicationFactory;

/* This test class uses a customized WebApplicationFactory (CustomWebApplicationFactory) to create a test server for
   the TodoApi. The customized WebApplicationFactory creates and initializes a test database. */

public class TestsWithTestDatabase : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public TestsWithTestDatabase(CustomWebApplicationFactory<Program> factory)
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