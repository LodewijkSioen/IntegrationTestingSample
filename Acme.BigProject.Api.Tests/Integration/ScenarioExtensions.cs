using System.Runtime.CompilerServices;
using Alba;

namespace Acme.BigProject.Api.Tests.Integration;

public static class ScenarioExtensions
{
    public static void Anonymous(this Scenario scenario)
    {
        scenario.RemoveRequestHeader("Authorization");
    }

    [ModuleInitializer]
    public static void Initialize() =>
        VerifyDiffPlex.Initialize();
}