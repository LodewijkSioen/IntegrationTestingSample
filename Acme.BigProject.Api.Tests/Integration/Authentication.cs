using Alba;

namespace Acme.BigProject.Api.Tests.Integration;

public class Authentication 
{
    [Test]
    public async Task Users_Me_Authorized()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/users/me");
            s.StatusCodeShouldBeOk();
        });
    }

    [Test]
    public async Task Users_Me_Unauthorized()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/users/me");
            s.Anonymous();
            s.StatusCodeShouldBe(401);
        });
    }
}