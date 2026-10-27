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

## Build
Requires **.NET 10 SDK**, SQL Server, and NuGet access. Not compiled/tested in this environment (SDK unavailable).

```bash
dotnet restore EriccoreTech.IntegrationFlow.slnx
dotnet build EriccoreTech.IntegrationFlow.slnx
dotnet test EriccoreTech.IntegrationFlow.slnx
# create the database schema (EF CLI required)
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet ef migrations add Initial --project src/EriccoreTech.IntegrationFlow.Persistence --startup-project src/EriccoreTech.IntegrationFlow.Api
dotnet ef database update --project src/EriccoreTech.IntegrationFlow.Persistence --startup-project src/EriccoreTech.IntegrationFlow.Api
dotnet run --project src/EriccoreTech.IntegrationFlow.Api
dotnet run --project src/EriccoreTech.IntegrationFlow.Worker
dotnet run --project src/EriccoreTech.IntegrationFlow.Designer
```

## API
- GET `/` and `/health`
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
- API and dashboard have **no authentication/authorization**. Never expose them publicly in this state.
- No broker adapters, distributed tracing, schema evolution enforcement, transactional end-to-end guarantees, or performance benchmarks.
- Never use arbitrary JSON-defined HTTP/SQL execution without allowlists and authorization.

## Design notes
This package expands Phase 1 into a multi-project, executable-source *foundation* for phases 2–7. The remaining integration and hardening work is substantial; don't deploy to production without completing it.
