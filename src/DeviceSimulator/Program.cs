using System;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ImpruvIT.Azure.IoT.DeviceSimulator
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var parser = new Parser(settings => { settings.HelpWriter = Console.Out; });

            var parseResult = parser.ParseArguments<Options>(args);

            var options = (parseResult as Parsed<Options>)?.Value;
            if (options == null)
                return -1;

            var services = new ServiceCollection();
            services.Configure(options);

            var provider = services.BuildServiceProvider();

            var deviceClient = await provider.GetRequiredService<IProvideServiceAsync<DeviceClient>>().Provide();

            await deviceClient.OpenAsync();

            Console.WriteLine($"Sending a telemetry message...");
            using var message = new Message(Encoding.UTF8.GetBytes("TestMessage"));
            await deviceClient.SendEventAsync(message);

            return 0;
        }
    }
}