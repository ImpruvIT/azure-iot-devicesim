using System.Collections.Generic;
using ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.Configuration
{
    internal static class Configuration
    {
        public static IServiceCollection Configure(this IServiceCollection services, Options options)
        {
            return services
                .AddLogging(builder =>
                {
                    builder
                        .AddConsole(o => o.FormatterName = ConsoleFormatterNames.Simple)
                        .AddSimpleConsole(o =>
                        {
                            o.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                            o.SingleLine = true;
                        });
                })
                .AddSingleton(options)
                .AddTransports()
                .AddTransient<ClientOptions>(_ => null)
                .AddConnectionFlows();
        }

        public static IServiceCollection AddTransports(this IServiceCollection services)
        {
            return services
                .AddTransient<TransportBuilder>()
                .AddTransient<IProvideService<ITransportSettings[]>, TransportBuilder>()
                .AddTransient<IEnumerable<ITransportSettings>>(p => p.GetRequiredService<IProvideService<ITransportSettings[]>>().Provide())
                .AddTransient<IProvideService<ProvisioningTransportHandler>, TransportBuilder>()
                .AddTransient(p => p.GetRequiredService<IProvideService<ProvisioningTransportHandler>>().Provide());
        }

        public static IServiceCollection AddConnectionFlows(this IServiceCollection services)
        {
            return services
                .AddTransient<SymmetricKeyManualClientProvider>()
                .AddTransient<CertificateManualClientProvider>()
                .AddTransient<SymmetricKeyDpsClientProvider>()
                .AddTransient<CertificateDpsClientProvider>()
                .AddTransient<IProvideService<IProvideServiceAsync<DeviceClient>>, ConnectionFlowBuilder>()
                .AddTransient(p => p.GetRequiredService<IProvideService<IProvideServiceAsync<DeviceClient>>>().Provide());
        }
    }
}