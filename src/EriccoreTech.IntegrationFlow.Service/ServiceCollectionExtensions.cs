using EriccoreTech.IntegrationFlow.Engine;
using EriccoreTech.IntegrationFlow.Management;
using EriccoreTech.IntegrationFlow.Messaging;
using EriccoreTech.IntegrationFlow.Patterns;
using EriccoreTech.IntegrationFlow.Service.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace EriccoreTech.IntegrationFlow.Service {
    public static class ServiceCollectionExtensions {

        // The caller registers IntegrationDbContext, since the connection string is host configuration.
        public static IServiceCollection AddIntegrationFlowServices(this IServiceCollection services) {
            services.AddSingleton<IIntegrationComponent, EchoComponent>();
            services.AddSingleton<IIntegrationComponent, UppercaseComponent>();
            services.AddSingleton<IIntegrationComponent, ContentFilterComponent>();
            services.AddSingleton<IIntegrationComponent, ContentBasedRouterComponent>();
            services.AddSingleton<WorkflowEngine>();
            services.AddSingleton<InMemoryMessageBus>();
            services.AddSingleton<ControlBus>();

            services.AddSingleton<IWorkflowService, WorkflowService>();
            services.AddSingleton<IComponentCatalogService, ComponentCatalogService>();
            services.AddSingleton<IMessagingService, MessagingService>();
            services.AddSingleton<IManagementService, ManagementService>();
            services.AddScoped<IExecutionHistoryService, ExecutionHistoryService>();
            return services;
        }

    }
}
