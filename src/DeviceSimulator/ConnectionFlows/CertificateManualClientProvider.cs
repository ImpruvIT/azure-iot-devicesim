using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows
{
    internal class CertificateManualClientProvider : IProvideServiceAsync<DeviceClient>
    {
        private readonly Options options;
        private readonly ITransportSettings[] transportSettings;
        private readonly ClientOptions clientOptions;

        public CertificateManualClientProvider(Options options, IEnumerable<ITransportSettings> transportSettings, ClientOptions clientOptions)
        {
            this.options = options;
            this.transportSettings = transportSettings.ToArray();
            this.clientOptions = clientOptions;
        }

        public Task<DeviceClient> Provide(CancellationToken cancellationToken = default)
        {
            var chain = options.LoadIdentityCertificateChain();
            using var auth = new DeviceAuthenticationWithX509Certificate(
                options.DeviceId,
                chain.GetIdentityCertificate(),
                chain);

            return Task.FromResult(DeviceClient.Create(options.HubHostname, options.GatewayHostname, auth, transportSettings, clientOptions));
        }
    }
}