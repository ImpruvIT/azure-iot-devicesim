using System;
using ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.Configuration
{
    internal class ConnectionFlowBuilder : IProvideService<IProvideServiceAsync<DeviceClient>>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Options options;

        public ConnectionFlowBuilder(IServiceProvider serviceProvider, Options options)
        {
            this.serviceProvider = serviceProvider;
            this.options = options;
        }

        public IProvideServiceAsync<DeviceClient> Provide()
        {
            if (options.ScopeId != null)
            {
                if (options.SymmetricKey != null)
                    return serviceProvider.GetRequiredService<SymmetricKeyDpsClientProvider>();
                if (options.CertificatePath != null)
                    return serviceProvider.GetRequiredService<CertificateDpsClientProvider>();
            }

            if (options.ConnectionString != null || options.SymmetricKey != null)
                return serviceProvider.GetRequiredService<SymmetricKeyManualClientProvider>();
            if (options.CertificatePath != null)
                return serviceProvider.GetRequiredService<CertificateManualClientProvider>();

            throw new NotSupportedException();
        }
    }
}