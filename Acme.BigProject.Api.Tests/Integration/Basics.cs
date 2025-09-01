using Alba;

namespace Acme.BigProject.Api.Tests.Integration;

public class Basics
{
    [Test]
    public async Task Root_HappyPath()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/");
            s.StatusCodeShouldBeOk();
        });
    }
    
    [Test]
    public async Task Root_CheckResult()
    {
        var result = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/");
            s.StatusCodeShouldBeOk();
        });

        var response = await result.ReadAsTextAsync();
        Assert.That(response, Is.EqualTo("Hello World"));
    }

    [Test]
    public async Task Root_OtherHttpMethods()
    {
        var result = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Post.Json(new
            {
                name = "New Name"
            }).ToUrl("/users/me");
        });
    }
}