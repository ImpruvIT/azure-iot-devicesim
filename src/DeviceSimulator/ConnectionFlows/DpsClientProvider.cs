using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows
{
    internal abstract class DpsClientProvider : IProvideServiceAsync<DeviceClient>
    {
        private readonly ProvisioningTransportHandler dpsTransport;
        private readonly ITransportSettings[] hubTransports;
        private readonly ClientOptions clientOptions;

        protected DpsClientProvider(
            Options options,
            ProvisioningTransportHandler dpsTransport,
            IEnumerable<ITransportSettings> hubTransports,
            ClientOptions clientOptions)
        {
            Options = options;
            this.dpsTransport = dpsTransport;
            this.hubTransports = hubTransports.ToArray();
            this.clientOptions = clientOptions;
        }
        
        protected Options Options { get; }

        public async Task<DeviceClient> Provide(CancellationToken cancellationToken = default)
        {
            using var security = GetDpsAuthentication();

            var provClient = ProvisioningDeviceClient.Create(
                !string.IsNullOrEmpty(Options.DpsHostname) ? Options.DpsHostname : "global.azure-devices-provisioning.net",
                Options.ScopeId,
                security,
                dpsTransport);

            Console.WriteLine("Registering with the DPS...");
            DeviceRegistrationResult result = await provClient.RegisterAsync(cancellationToken);

            Console.WriteLine($"Registration status: {result.Status}.");
            if (result.Status != ProvisioningRegistrationStatusType.Assigned)
            {
                Console.WriteLine($"Registration status did not assign a hub, so exiting this sample.");
                throw new InvalidOperationException();
            }

            Console.WriteLine($"Device {result.DeviceId} registered to {result.AssignedHub}.");

            Console.WriteLine("Creating SAS authentication for IoT Hub...");
            var auth = GetDeviceAuthentication(result);

            Console.WriteLine($"Testing the provisioned device with IoT Hub...");
            return DeviceClient.Create(result.AssignedHub, Options.GatewayHostname, auth, hubTransports, clientOptions);
        }

        protected abstract SecurityProvider GetDpsAuthentication();

        protected abstract IAuthenticationMethod GetDeviceAuthentication(DeviceRegistrationResult result);
    }
}