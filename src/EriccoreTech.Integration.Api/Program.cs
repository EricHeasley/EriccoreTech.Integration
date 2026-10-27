using EriccoreTech.Integration.Persistence;
using EriccoreTech.Integration.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddIntegrationServices();
builder.Services.AddDbContext<IntegrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IntegrationFlow")));

var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/", () => Results.Ok(new { name = "IntegrationFlow", phase = 1 }));
app.MapControllers();
app.Run();

public partial class Program {
}
