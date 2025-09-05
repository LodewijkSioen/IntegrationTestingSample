using System.Runtime.CompilerServices;

namespace Acme.BigProject.Api.Tests.Integration;

public static class ScenarioExtensions
{
    [ModuleInitializer]
    public static void Initialize() =>
        VerifyDiffPlex.Initialize();
}