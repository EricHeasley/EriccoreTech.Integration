using System.Text.Json;
using EriccoreTech.IntegrationFlow.Api;
using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Engine;
using EriccoreTech.IntegrationFlow.Management;
using EriccoreTech.IntegrationFlow.Messaging;
using EriccoreTech.IntegrationFlow.Patterns;
using EriccoreTech.IntegrationFlow.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IIntegrationComponent, EchoComponent>();
builder.Services.AddSingleton<IIntegrationComponent, UppercaseComponent>();
builder.Services.AddSingleton<IIntegrationComponent, ContentFilterComponent>();
builder.Services.AddSingleton<IIntegrationComponent, ContentBasedRouterComponent>();
builder.Services.AddSingleton<WorkflowEngine>();
builder.Services.AddSingleton<InMemoryMessageBus>();
builder.Services.AddSingleton<ControlBus>();
builder.Services.AddDbContext<IntegrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IntegrationFlow")));

var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/management/channels", (ControlBus bus) => Results.Ok(bus.Snapshot()));
app.MapPost("/management/channels/{topic}/pause", (string topic, ControlBus bus) => {
    bus.Pause(topic);
    return Results.Ok();
});
app.MapPost("/management/channels/{topic}/resume", (string topic, ControlBus bus) => {
    bus.Resume(topic);
    return Results.Ok();
});
app.MapGet("/executions", async (IntegrationDbContext db, CancellationToken ct) => {
    return Results.Ok(await db.Executions.AsNoTracking().OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync(ct));
});
app.MapGet("/", () => Results.Ok(new { name = "IntegrationFlow", phase = 1 }));
app.MapGet("/components", (IEnumerable<IIntegrationComponent> components) => {
    return Results.Ok(components.Select(x => x.Type));
});
app.MapPost("/workflows/execute", async (ExecuteRequest request, WorkflowEngine engine, CancellationToken ct) => {
    try {
        return Results.Ok(await engine.ExecuteAsync(request.Definition, request.Input, ct));
    } catch (ArgumentException ex) {
        return Results.BadRequest(new { error = ex.Message });
    }
});
app.MapPost("/messages/{topic}", async (string topic, JsonElement payload, InMemoryMessageBus bus, CancellationToken ct) => {
    var envelope = MessageEnvelope<object>.Create(topic, payload.Clone());
    await bus.PublishAsync(topic, envelope, ct);
    return Results.Accepted(value: new { envelope.MessageId, envelope.CorrelationId });
});
app.Run();

public partial class Program {
}
