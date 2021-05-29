using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows
{
    internal class SymmetricKeyManualClientProvider : IProvideServiceAsync<DeviceClient>
    {
        private readonly Options options;
        private readonly ITransportSettings[] transportSettings;
        private readonly ClientOptions clientOptions;
        private readonly ILogger logger;

        public SymmetricKeyManualClientProvider(
            Options options,
            IEnumerable<ITransportSettings> transportSettings,
            ClientOptions clientOptions,
            ILogger<SymmetricKeyManualClientProvider> logger)
        {
            this.options = options;
            this.transportSettings = transportSettings.ToArray();
            this.clientOptions = clientOptions;
            this.logger = logger;
        }

        public Task<DeviceClient> Provide(CancellationToken cancellationToken = default)
        {
            if (options.ConnectionString != null)
            {
                logger.LogDebug("Connecting to IoT Hub using connection string");
                return options.DeviceId == null
                    ? Task.FromResult(DeviceClient.CreateFromConnectionString(options.ConnectionString, transportSettings, clientOptions))
                    : Task.FromResult(DeviceClient.CreateFromConnectionString(options.ConnectionString, options.DeviceId, transportSettings, clientOptions));
            }

            if (string.IsNullOrEmpty(options.HubHostname))
                throw new ConfigurationException("The IoT Hub hostname has to be specified.");
            if (string.IsNullOrEmpty(options.DeviceId))
                throw new ConfigurationException("The IoT device name has to be specified.");
            if (string.IsNullOrEmpty(options.SymmetricKey))
                throw new ConfigurationException("The authentication symmetric key has to be specified.");

            logger.LogDebug(
                "Connecting to '{IoTHub}' through gateway '{Gateway}' as device '{DeviceId}' using a symmetric key authentication.",
                options.HubHostname,
                options.GatewayHostname,
                options.DeviceId);

            var auth = new DeviceAuthenticationWithRegistrySymmetricKey(options.DeviceId, options.SymmetricKey);
            return Task.FromResult(DeviceClient.Create(options.HubHostname, options.GatewayHostname, auth, transportSettings, clientOptions));
        }
    }
}