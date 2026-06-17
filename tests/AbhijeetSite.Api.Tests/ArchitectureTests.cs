using System.Reflection;
using NetArchTest.Rules;

namespace AbhijeetSite.Api.Tests;

public sealed class ArchitectureTests
{
    private const string ArticlesNamespace = "AbhijeetSite.Api.Features.Articles";
    private const string CommentsNamespace = "AbhijeetSite.Api.Features.Comments";
    private const string FeaturesNamespace = "AbhijeetSite.Api.Features";
    private const string IdentityNamespace = "AbhijeetSite.Api.Features.Identity";
    private const string SharedKernelNamespace = "AbhijeetSite.Api.SharedKernel";

    private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

    [Fact]
    public void SharedKernel_ShouldNotDependOnFeatures()
    {
        TestResult result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespace(SharedKernelNamespace)
            .Should()
            .NotHaveDependencyOn(FeaturesNamespace)
            .GetResult();

        AssertArchitectureRule(result);
    }

    [Fact]
    public void Identity_ShouldNotDependOnArticles()
    {
        TestResult result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespace(IdentityNamespace)
            .Should()
            .NotHaveDependencyOn(ArticlesNamespace)
            .GetResult();

        AssertArchitectureRule(result);
    }

    [Fact]
    public void Identity_ShouldNotDependOnComments()
    {
        TestResult result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespace(IdentityNamespace)
            .Should()
            .NotHaveDependencyOn(CommentsNamespace)
            .GetResult();

        AssertArchitectureRule(result);
    }

    [Fact]
    public void Articles_ShouldNotDependOnComments()
    {
        TestResult result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespace(ArticlesNamespace)
            .Should()
            .NotHaveDependencyOn(CommentsNamespace)
            .GetResult();

        AssertArchitectureRule(result);
    }

    private static void AssertArchitectureRule(TestResult result)
    {
        string failingTypes = string.Join(", ", result.FailingTypeNames ?? []);
        Assert.True(result.IsSuccessful, failingTypes);
    }
}
