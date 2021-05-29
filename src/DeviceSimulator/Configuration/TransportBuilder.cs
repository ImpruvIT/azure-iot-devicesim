using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.Configuration
{
    internal class TransportBuilder : IProvideService<ITransportSettings[]>, IProvideService<ProvisioningTransportHandler>
    {
        private readonly Options options;

        public TransportBuilder(Options options)
        {
            this.options = options;
        }

        ITransportSettings[] IProvideService<ITransportSettings[]>.Provide()
        {
            return new ITransportSettings[]
            {
                new AmqpTransportSettings(TransportType.Amqp_Tcp_Only),
                new MqttTransportSettings(TransportType.Mqtt_Tcp_Only)
            };
        }

        ProvisioningTransportHandler IProvideService<ProvisioningTransportHandler>.Provide()
        {
            return new ProvisioningTransportHandlerAmqp();
        }
    }
}