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
using Microsoft.Extensions.Logging;

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
            ClientOptions clientOptions,
            ILogger logger)
        {
            Options = options;
            Logger = logger;
            this.dpsTransport = dpsTransport;
            this.hubTransports = hubTransports.ToArray();
            this.clientOptions = clientOptions;
        }

        protected Options Options { get; }
        protected ILogger Logger { get; }

        public async Task<DeviceClient> Provide(CancellationToken cancellationToken = default)
        {
            using var security = GetDpsAuthentication();

            var dpsEndpoint = !string.IsNullOrEmpty(Options.DpsHostname) ? Options.DpsHostname : "global.azure-devices-provisioning.net";
            var provClient = ProvisioningDeviceClient.Create(
                dpsEndpoint,
                Options.ScopeId,
                security,
                dpsTransport);

            Logger.LogDebug("Registering with the DPS endpoint '{DpsEndpoint}'", dpsEndpoint);
            DeviceRegistrationResult result = await provClient.RegisterAsync(cancellationToken);

            if (result.Status != ProvisioningRegistrationStatusType.Assigned)
            {
                Logger.LogError("Registration with DPS failed with status '{Status}' and error '{Error}' ({ErrorCode})", result.Status, result.ErrorMessage,
                    result.ErrorCode);
                throw new InvalidOperationException();
            }
            else
            {
                Logger.LogDebug("Registering with DPS succeeded.");
            }

            Logger.LogInformation("The device '{DeviceId}' is successfully registered to '{AssignedHub}'.", result.DeviceId, result.AssignedHub);

            var auth = GetDeviceAuthentication(result);
            return DeviceClient.Create(result.AssignedHub, Options.GatewayHostname, auth, hubTransports, clientOptions);
        }

        protected abstract SecurityProvider GetDpsAuthentication();

        protected abstract IAuthenticationMethod GetDeviceAuthentication(DeviceRegistrationResult result);
    }
}