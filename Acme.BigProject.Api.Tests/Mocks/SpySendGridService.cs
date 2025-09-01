using Acme.BigProject.Api.Services;
using Moq;

namespace Acme.BigProject.Api.Tests.Mocks;

public class SpySendGridService : ISendGridService
{
    public static readonly Mock<ISendGridService> Mock = new();

    public Task SendNewsletterConfirmation(string emailAddress)
    {
        return Mock.Object.SendNewsletterConfirmation(emailAddress);
    }
}