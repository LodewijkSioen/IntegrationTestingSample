using Acme.BigProject.Api.Tests.Mocks;

namespace Acme.BigProject.Api.Tests.Integration;

public class Dependencies : DependencyFixture
{
    [Test]
    public async Task SubscribeNewsletter_HappyPath()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Post
                .Json(new
                {
                    EmailAddress = "test@example.org"
                })
                .ToUrl("/newsletter/subscribe");
            s.StatusCodeShouldBe(200);
        });

        #region Assert

        MockSendGridService.Mock.Verify(m => m.SendNewsletterConfirmation("test@example.org"));

        #endregion
    }
}