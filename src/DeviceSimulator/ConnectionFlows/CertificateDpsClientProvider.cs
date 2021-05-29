using System;
using System.Collections.Generic;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows
{
    internal class CertificateDpsClientProvider : DpsClientProvider
    {
        private readonly Lazy<SecurityProviderX509Certificate> dpsAuthentication;

        public CertificateDpsClientProvider(
            Options options,
            ProvisioningTransportHandler dpsTransport,
            IEnumerable<ITransportSettings> hubTransports,
            ClientOptions clientOptions,
            ILogger<CertificateDpsClientProvider> logger)
            : base(options, dpsTransport, hubTransports, clientOptions, logger)
        {
            dpsAuthentication = new Lazy<SecurityProviderX509Certificate>(CreateDpsAuthentication);
        }

        protected override SecurityProvider GetDpsAuthentication()
        {
            var auth = dpsAuthentication.Value;

            Logger.LogDebug(
                "Registering as registration '{RegistrationId}' using certificate authentication.",
                auth.GetRegistrationID());

            return auth;
        }

        protected override IAuthenticationMethod GetDeviceAuthentication(DeviceRegistrationResult result)
        {
            var dpsAuth = dpsAuthentication.Value;

            Logger.LogDebug("Authenticating to IoT Hub using a certificate authentication.");

            return new DeviceAuthenticationWithX509Certificate(
                result.DeviceId,
                dpsAuth.GetAuthenticationCertificate(),
                dpsAuth.GetAuthenticationCertificateChain());
        }

        private SecurityProviderX509Certificate CreateDpsAuthentication()
        {
            if (string.IsNullOrEmpty(Options.CertificatePath))
                throw new ConfigurationException("The path to certificate has to be specified.");

            var chain = Options.LoadIdentityCertificateChain();
            return new SecurityProviderX509Certificate(chain.GetIdentityCertificate(), chain);
        }
    }
}