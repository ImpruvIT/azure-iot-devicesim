using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows
{
    internal class CertificateManualClientProvider : IProvideServiceAsync<DeviceClient>
    {
        private readonly Options options;
        private readonly ITransportSettings[] transportSettings;
        private readonly ClientOptions clientOptions;
        private readonly ILogger<CertificateManualClientProvider> logger;

        public CertificateManualClientProvider(
            Options options,
            IEnumerable<ITransportSettings> transportSettings,
            ClientOptions clientOptions,
            ILogger<CertificateManualClientProvider> logger)
        {
            this.options = options;
            this.transportSettings = transportSettings.ToArray();
            this.clientOptions = clientOptions;
            this.logger = logger;
        }

        public Task<DeviceClient> Provide(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(options.DeviceId))
                throw new ConfigurationException("The IoT device name has to be specified.");
            if (string.IsNullOrEmpty(options.CertificatePath))
                throw new ConfigurationException("The path to certificate has to be specified.");

            var chain = options.LoadIdentityCertificateChain();
            using var auth = new DeviceAuthenticationWithX509Certificate(
                options.DeviceId,
                chain.GetIdentityCertificate(),
                chain);

            logger.LogDebug(
                "Connecting to '{IoTHub}' through gateway '{Gateway}' as device '{DeviceId}' using a certificate authentication.",
                options.HubHostname,
                options.GatewayHostname,
                auth.DeviceId);

            return Task.FromResult(DeviceClient.Create(options.HubHostname, options.GatewayHostname, auth, transportSettings, clientOptions));
        }
    }
}