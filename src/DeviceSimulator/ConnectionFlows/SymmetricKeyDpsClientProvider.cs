using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.ConnectionFlows
{
    internal class SymmetricKeyDpsClientProvider : DpsClientProvider
    {
        private readonly Lazy<SecurityProviderSymmetricKey> dpsAuthentication;

        public SymmetricKeyDpsClientProvider(
            Options options,
            ProvisioningTransportHandler dpsTransport,
            IEnumerable<ITransportSettings> hubTransports,
            ClientOptions clientOptions)
            : base(options, dpsTransport, hubTransports, clientOptions)
        {
            dpsAuthentication = new Lazy<SecurityProviderSymmetricKey>(CreateDpsAuthentication);
        }

        protected override SecurityProvider GetDpsAuthentication() => dpsAuthentication.Value;

        protected override IAuthenticationMethod GetDeviceAuthentication(DeviceRegistrationResult result)
        {
            var dpsAuth = dpsAuthentication.Value;

            var deviceKey = dpsAuth.GetPrimaryKey();
            if (string.IsNullOrEmpty(deviceKey))
                deviceKey = dpsAuth.GetSecondaryKey();

            return new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId, deviceKey);
        }

        private SecurityProviderSymmetricKey CreateDpsAuthentication() => new(
            Options.RegistrationId,
            GetDeviceKey(Options.SymmetricKey),
            Options.SymmetricKey2 != null ? GetDeviceKey(Options.SymmetricKey2) : null);

        private string GetDeviceKey(string symmetricKey) => 
            Options.IsIndividual ? symmetricKey : ComputeDerivedSymmetricKey(symmetricKey, Options.RegistrationId);

        private static string ComputeDerivedSymmetricKey(string masterKey, string registrationId)
        {
            using var hmac = new HMACSHA256(Convert.FromBase64String(masterKey));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationId)));
        }
    }
}