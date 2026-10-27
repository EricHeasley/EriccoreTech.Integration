using EriccoreTech.IntegrationFlow.Persistence;
using EriccoreTech.IntegrationFlow.Reliability;
using EriccoreTech.IntegrationFlow.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<IntegrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:IntegrationFlow"]));
builder.Services.AddScoped<OutboxRelay>();
builder.Services.AddHostedService<OutboxWorker>();
await builder.Build().RunAsync();
