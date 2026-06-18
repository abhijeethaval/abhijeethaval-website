using System.IO.Pipes;

namespace AbhijeetSite.Api.Tests.Support;

public sealed class DockerRequiredFactAttribute : FactAttribute
{
    private const int DockerPipeTimeoutMilliseconds = 500;
    private const string DockerHostVariable = "DOCKER_HOST";
    private const string DockerUnavailableReason = "Docker is required for Testcontainers PostgreSQL tests.";
    private const string WindowsDockerPipeName = "docker_engine";

    public DockerRequiredFactAttribute()
    {
        if (!IsDockerAvailable())
        {
            Skip = DockerUnavailableReason;
        }
    }

    private static bool IsDockerAvailable()
    {
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(DockerHostVariable)))
        {
            return true;
        }

        return OperatingSystem.IsWindows() ? IsWindowsDockerPipeAvailable() : IsUnixDockerSocketAvailable();
    }

    private static bool IsWindowsDockerPipeAvailable()
    {
        try
        {
            using NamedPipeClientStream pipe = new(".", WindowsDockerPipeName, PipeDirection.InOut);
            pipe.Connect(DockerPipeTimeoutMilliseconds);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static bool IsUnixDockerSocketAvailable()
    {
        return File.Exists("/var/run/docker.sock");
    }
}
