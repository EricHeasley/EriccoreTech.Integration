# IntegrationFlow — Phases 1–7 source implementation

**Status: development scaffold / reference implementation, NOT production ready.**

## Modules and phase coverage
1. Foundation: core envelope, DAG compiler, bounded-parallel executor, in-memory pub/sub.
2. Reliability: EF Core SQL Server execution, step, outbox and inbox schema; execution store.
3. Messaging: SQL-backed queue with lease claim, ack, retry/dead-letter state; background outbox pump.
4. Patterns: splitter, aggregator, resequencer, translator, recipient-list, routing slip, content-based router, filter.
5. Advanced execution: condition evaluator, process state-machine transitions; DAG runtime from phase 1.
6. Management: control bus pause/resume state, health endpoint, execution history endpoint.
7. Blazor: interactive server dashboard displaying recent SQL execution records.

## Solution layout
Each project uses one namespace (its project name); the folders inside are organisational.

| Project | Contents |
|---|---|
| `Core` | Plain data: envelope, workflow/step definitions and results, request/response DTOs. No references. |
| `Contracts` | Interfaces only: `IIntegrationComponent`, `IMessagePublisher`/`IMessageConsumer`/`IMessageTransport`, the `I*Service` contracts. |
| `Messaging` | Channels, envelope helpers/serializer, publishers, consumers, subscriptions, delivery policy. |
| `Routing` | ContentBased, Dynamic, RecipientList, RoutingSlip. |
| `Transformation` | Translators, Normalizers, Enrichers, CanonicalModels. |
| `Processing` | Splitter, Aggregator, Resequencer, ScatterGather. |
| `Orchestration` | WorkflowCompiler, ExecutionScheduler, ProcessManager, StateMachine. |
| `Reliability` | Retry, Idempotency, DeadLetter, Outbox (+ relay), Inbox. |
| `Transport` | InMemory, SqlServer queue, RabbitMQ, AzureServiceBus. |
| `Management` | ControlBus, MessageHistory, MessageStore, Diagnostics. |
| `Persistence`, `Service` | Supporting layers outside the diagram: EF Core model/stores, and the service implementations the API calls. |
| `Api`, `Worker`, `Designer` | Hosts. `Api` has thin controllers over the `I*Service` contracts. |

## Build
Requires **.NET 10 SDK**, SQL Server, and NuGet access. Builds with zero warnings and the unit tests pass (xunit, EF Core InMemory). The RabbitMQ and Azure Service Bus transports compile but have not been exercised against a live broker, and nothing has been run against SQL Server.

```bash
dotnet restore EriccoreTech.Integration.slnx
dotnet build EriccoreTech.Integration.slnx
dotnet test EriccoreTech.Integration.slnx
# create the database schema (EF CLI required)
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet ef migrations add Initial --project src/EriccoreTech.Integration.Persistence --startup-project src/EriccoreTech.Integration.Api
dotnet ef database update --project src/EriccoreTech.Integration.Persistence --startup-project src/EriccoreTech.Integration.Api
dotnet run --project src/EriccoreTech.Integration.Api
dotnet run --project src/EriccoreTech.Integration.Worker
dotnet run --project src/EriccoreTech.Integration.Designer
```

## Configuration
The Api fails to start unless JWT validation is configured (set via environment variables or user secrets, never committed):
- `Authentication__Authority` — HTTPS URL of the identity provider
- `Authentication__Audience`
- `ConnectionStrings__IntegrationFlow`
- `HttpComponent__AllowedHosts__0`, `__1`, … — hosts the `http` component may call (empty means every call is refused); `HttpComponent__TimeoutSeconds` defaults to 30

All endpoints require a valid bearer token except `/` and `/health`.

## API
- GET `/` and `/health` (anonymous)
- GET `/components`
- POST `/workflows/execute` — synchronous, request-scoped DAG execution
- POST `/messages/{topic}` — in-memory fanout; subscriptions must be active
- GET `/executions` — SQL-backed execution history (not automatically populated by synchronous endpoint)
- GET `/management/channels`, POST `/management/channels/{topic}/pause|resume`

## Important limitations and next engineering work
- **Not all 65 EIP patterns are implemented.** Some pattern helpers are demonstrations rather than complete EIP semantics.
- The workflow API does **not** persist execution history automatically; durable scheduling/recovery and long-running workflow resumption remain to be integrated.
- SQL outbox pump is single-worker only. It needs distributed claiming before multi-instance use.
- SQL queue lease expiry may cause redelivery; consumers must be idempotent. Inbox table exists but inbox processing is not wired.
- Management pause state is in-memory and not enforced by all transports.
- Dashboard is read-only (not yet a drag-and-drop designer).
- The Api requires a JWT bearer token (see Configuration). Role checks are not applied yet (an `IntegrationOperator` policy exists but is unused), and the **Designer dashboard has no authentication**. Never expose it publicly.
- No broker adapters, distributed tracing, schema evolution enforcement, transactional end-to-end guarantees, or performance benchmarks.
- The `http` workflow component calls web services (https only, host allowlist, no redirects, non-2xx fails the step so `MaxAttempts` retries it). It has no built-in credential store: any auth header comes from the step input.

## Design notes
This package expands Phase 1 into a multi-project, executable-source *foundation* for phases 2–7. The remaining integration and hardening work is substantial; don't deploy to production without completing it.
