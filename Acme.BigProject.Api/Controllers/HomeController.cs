using System.IdentityModel.Tokens.Jwt;
using Acme.BigProject.Api.Domain;
using Acme.BigProject.Api.Models;
using Acme.BigProject.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.BigProject.Api.Controllers;

[ApiController]
[Route("/")]
public class HomeController(
    AcmeDbContext dbContext,
    ISendGridService sendGridService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult Get()
    {
        return Ok("Hello World");
    }

    [HttpGet("users/me")]
    public ActionResult Get_Users_Me()
    {
        var nameClaim = HttpContext.User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Name);
        return nameClaim != null
            ? Ok(nameClaim.Value)
            : throw new NullReferenceException("Name Claim should not be null");
    }

    [HttpPost("/users/me")]
    public ActionResult Post_Users_Me()
    {

    }

    [HttpGet("products/{productName}")]
    public ActionResult Get_Product_By_Name(string productName)
    {
        var product = dbContext.Products.SingleOrDefault(p => p.UrlName == productName);

        return product == null
            ? NotFound()
            : Ok(new ProductModel(product));
    }

    public record NewsletterSubscriptionRequest(string EmailAddress);
    [HttpPost("newsletter/subscribe")]
    public ActionResult Subscribe_Newsletter(NewsletterSubscriptionRequest request)
    {
        sendGridService.SendNewsletterConfirmation(request.EmailAddress);
        return Ok();
    }
}