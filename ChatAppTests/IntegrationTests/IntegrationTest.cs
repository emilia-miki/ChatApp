using NUnit.Framework;
using Respawn;

namespace ChatAppTests.IntegrationTests;

[TestFixture]
public abstract class IntegrationTest
{
    private readonly RespawnerOptions _options = new()
    {
        SchemasToInclude = new[]
        {
            "dbo"
        },
        WithReseed = true
    };

    private readonly Respawner _respawner;
    protected readonly string ConnString;
    
    protected readonly ApiWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    public IntegrationTest()
    {
        Factory = new ApiWebApplicationFactory();
        Client = Factory.CreateClient();
        
        ConnString = Factory.Configuration
            .GetSection("ConnectionStrings")["Default"];
        _respawner = Respawner.CreateAsync(ConnString, _options).Result;
    }

    protected async Task ResetDatabase()
    {
        await _respawner.ResetAsync(ConnString);
    }
}