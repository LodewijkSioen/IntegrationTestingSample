using System.IdentityModel.Tokens.Jwt;
using Alba;

namespace Acme.BigProject.Api.Tests.Integration;

public class Authentication 
{
    [Test]
    public async Task Users_Me_Authorized()
    {
        var result = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/users/me");
            s.StatusCodeShouldBeOk();
        });

        var response = await result.ReadAsTextAsync();
        Assert.That(response, Is.EqualTo("Willy E Coyote"));
    }

    [Test]
    public async Task Users_Me_Unauthorized()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/users/me");
            s.RemoveRequestHeader("Authorization");
            s.StatusCodeShouldBe(401);
        });
    }

    [Test]
    public async Task Users_Me_OtherClaim()
    {
        var result = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/users/me/email");
            s.WithClaim(new(JwtRegisteredClaimNames.Email, "test@example.org"));
            s.StatusCodeShouldBeOk();
        });

        var response = await result.ReadAsTextAsync();
        Assert.That(response, Is.EqualTo("test@example.org"));
    }

    [Test]
    public async Task Users_AdminOnly()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/users/");
            s.WithClaim(new("role", "admin"));
            s.StatusCodeShouldBeOk();
        });
    }

    [Test]
    public async Task Users_AdminOnly_NotAllowed()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/users/");
            s.StatusCodeShouldBe(403);
        });
    }
}