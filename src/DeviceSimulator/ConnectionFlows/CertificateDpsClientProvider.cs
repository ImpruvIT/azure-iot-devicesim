using System;
using System.Collections.Generic;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows
{
    internal class CertificateDpsClientProvider : DpsClientProvider
    {
        private readonly Lazy<SecurityProviderX509Certificate> dpsAuthentication;

        public CertificateDpsClientProvider(
            Options options,
            ProvisioningTransportHandler dpsTransport,
            IEnumerable<ITransportSettings> hubTransports,
            ClientOptions clientOptions)
            : base(options, dpsTransport, hubTransports, clientOptions)
        {
            dpsAuthentication = new Lazy<SecurityProviderX509Certificate>(CreateDpsAuthentication);
        }

        protected override SecurityProvider GetDpsAuthentication() => dpsAuthentication.Value;

        protected override IAuthenticationMethod GetDeviceAuthentication(DeviceRegistrationResult result)
        {
            var dpsAuth = dpsAuthentication.Value;

            return new DeviceAuthenticationWithX509Certificate(
                result.DeviceId,
                dpsAuth.GetAuthenticationCertificate(),
                dpsAuth.GetAuthenticationCertificateChain());
        }

        private SecurityProviderX509Certificate CreateDpsAuthentication()
        {
            var chain = Options.LoadIdentityCertificateChain();
            return new SecurityProviderX509Certificate(chain.GetIdentityCertificate(), chain);
        }
    }
}