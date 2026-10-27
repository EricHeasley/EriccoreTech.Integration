using EriccoreTech.IntegrationFlow.Contracts;
using EriccoreTech.IntegrationFlow.Management;
using EriccoreTech.IntegrationFlow.Orchestration;
using EriccoreTech.IntegrationFlow.Routing;
using EriccoreTech.IntegrationFlow.Transformation;
using EriccoreTech.IntegrationFlow.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EriccoreTech.IntegrationFlow.Service {
    public static class ServiceCollectionExtensions {

        // The caller registers IntegrationDbContext, since the connection string is host configuration.
        // The message transport defaults to in-memory; register another IMessageTransport before this call to replace it.
        public static IServiceCollection AddIntegrationFlowServices(this IServiceCollection services) {
            services.AddSingleton<IIntegrationComponent, EchoComponent>();
            services.AddSingleton<IIntegrationComponent, UppercaseComponent>();
            services.AddSingleton<IIntegrationComponent, ContentFilterComponent>();
            services.AddSingleton<IIntegrationComponent, ContentBasedRouterComponent>();
            services.AddSingleton<ExecutionScheduler>();
            services.AddSingleton<ProcessManager>();
            services.AddSingleton<ControlBus>();

            services.TryAddSingleton<IMessageTransport, InMemoryTransport>();
            services.AddSingleton<IMessagePublisher>(provider => provider.GetRequiredService<IMessageTransport>());
            services.AddSingleton<IMessageConsumer>(provider => provider.GetRequiredService<IMessageTransport>());

            services.AddSingleton<IWorkflowService, WorkflowService>();
            services.AddSingleton<IComponentCatalogService, ComponentCatalogService>();
            services.AddSingleton<IMessagingService, MessagingService>();
            services.AddSingleton<IManagementService, ManagementService>();
            services.AddScoped<IExecutionHistoryService, ExecutionHistoryService>();
            return services;
        }

    }
}
