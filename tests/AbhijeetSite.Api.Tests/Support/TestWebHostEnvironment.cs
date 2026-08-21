using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace AbhijeetSite.Api.Tests.Support;

internal sealed class TestWebHostEnvironment : IWebHostEnvironment
{
    internal TestWebHostEnvironment(string environmentName)
    {
        EnvironmentName = environmentName;
    }

    public string ApplicationName { get; set; } = nameof(AbhijeetSite);

    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();

    public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();

    public string EnvironmentName { get; set; }

    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();

    public string WebRootPath { get; set; } = Directory.GetCurrentDirectory();
}
