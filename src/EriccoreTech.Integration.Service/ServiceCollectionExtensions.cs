using EriccoreTech.Integration.Contracts;
using EriccoreTech.Integration.Management;
using EriccoreTech.Integration.Orchestration;
using EriccoreTech.Integration.Routing;
using EriccoreTech.Integration.Transformation;
using EriccoreTech.Integration.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EriccoreTech.Integration.Service {
    public static class ServiceCollectionExtensions {

        // The caller registers IntegrationDbContext, since the connection string is host configuration.
        // The message transport defaults to in-memory; register another IMessageTransport before this call to replace it.
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services) {
            services.AddSingleton<IIntegrationComponent, EchoComponent>();
            services.AddSingleton<IIntegrationComponent, UppercaseComponent>();
            services.AddSingleton<IIntegrationComponent, ContentFilterComponent>();
            services.AddSingleton<IIntegrationComponent, ContentBasedRouterComponent>();
            services.AddHttpClient(HttpRequestComponent.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { AllowAutoRedirect = false });
            services.AddSingleton<IIntegrationComponent, HttpRequestComponent>();
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
