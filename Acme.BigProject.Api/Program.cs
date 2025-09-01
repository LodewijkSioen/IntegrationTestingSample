using Acme.BigProject.Api.Domain;
using Acme.BigProject.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var conString = builder.Configuration.GetConnectionString("AcmeDatabase") ??
                throw new InvalidOperationException("Connection string 'AcmeDatabase' not found.");
builder.Services.AddDbContext<AcmeDbContext>(opts =>
{
    opts.UseSqlServer(conString);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
    {
        opts.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = false,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddTransient<ISendGridService, SendGridService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.Run();
