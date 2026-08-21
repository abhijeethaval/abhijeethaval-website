using AbhijeetSite.Api.Infrastructure.Persistence;
using AbhijeetSite.Api.Tests.Support;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AbhijeetSite.Api.Tests;

public sealed class PersistenceConfigurationTests
{
    private const string ExpectedMessage =
        "ConnectionStrings__abhijeetsite-db must be configured outside Development.";

    [Fact]
    public void AddPersistence_ProductionWithoutConnectionString_ThrowsActionableError()
    {
        ServiceCollection services = new();
        IConfiguration configuration = new ConfigurationBuilder().Build();
        TestWebHostEnvironment environment = new(Environments.Production);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => services.AddPersistence(configuration, environment));

        Assert.Equal(ExpectedMessage, exception.Message);
    }
}
