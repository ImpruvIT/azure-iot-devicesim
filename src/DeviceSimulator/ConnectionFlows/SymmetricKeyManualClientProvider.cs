using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows
{
    internal class SymmetricKeyManualClientProvider : IProvideServiceAsync<DeviceClient>
    {
        private readonly Options options;
        private readonly ITransportSettings[] transportSettings;
        private readonly ClientOptions clientOptions;

        public SymmetricKeyManualClientProvider(Options options, IEnumerable<ITransportSettings> transportSettings, ClientOptions clientOptions)
        {
            this.options = options;
            this.transportSettings = transportSettings.ToArray();
            this.clientOptions = clientOptions;
        }

        public Task<DeviceClient> Provide(CancellationToken cancellationToken = default)
        {
            if (options.ConnectionString != null)
            {
                return options.DeviceId == null
                    ? Task.FromResult(DeviceClient.CreateFromConnectionString(options.ConnectionString, transportSettings, clientOptions))
                    : Task.FromResult(DeviceClient.CreateFromConnectionString(options.ConnectionString, options.DeviceId, transportSettings, clientOptions));
            }
            else
            {
                var auth = new DeviceAuthenticationWithRegistrySymmetricKey(options.DeviceId, options.SymmetricKey);
                return Task.FromResult(DeviceClient.Create(options.HubHostname, options.GatewayHostname, auth, transportSettings, clientOptions));
            }
        }
    }
}