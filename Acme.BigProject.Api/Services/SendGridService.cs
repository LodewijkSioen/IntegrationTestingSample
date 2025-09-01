namespace Acme.BigProject.Api.Services;

public interface ISendGridService
{
    Task SendNewsletterConfirmation(string emailAddress);
}

public class SendGridService : ISendGridService
{
    public Task SendNewsletterConfirmation(string emailAddress)
    {
        throw new("Sendgrid not configured.");
    }
}