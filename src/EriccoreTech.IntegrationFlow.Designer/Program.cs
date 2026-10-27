using EriccoreTech.IntegrationFlow.Designer.Components;
using EriccoreTech.IntegrationFlow.Management;
using EriccoreTech.IntegrationFlow.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddDbContextFactory<IntegrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IntegrationFlow")));
builder.Services.AddSingleton<ControlBus>();

var app = builder.Build();
if (!app.Environment.IsDevelopment()) {
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
