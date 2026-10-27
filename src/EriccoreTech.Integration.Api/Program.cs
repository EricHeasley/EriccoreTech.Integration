using EriccoreTech.Integration.Api;
using EriccoreTech.Integration.Persistence;
using EriccoreTech.Integration.Transport;
using EriccoreTech.Integration.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddIntegrationAuthentication(builder.Configuration);
builder.Services.Configure<HttpComponentOptions>(builder.Configuration.GetSection("HttpComponent"));
builder.Services.AddIntegrationServices();
builder.Services.AddDbContext<IntegrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IntegrationFlow")));

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();
app.MapGet("/", () => Results.Ok(new { name = "IntegrationFlow", phase = 1 })).AllowAnonymous();
app.MapControllers();
app.Run();

public partial class Program {
}
