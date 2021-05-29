using System;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ImpruvIT.Azure.IoT.DeviceSimulator
{
    internal class Program
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

            var logger = provider.GetRequiredService<ILogger<Program>>();

            try
            {
                var deviceClient = await provider.GetRequiredService<IProvideServiceAsync<DeviceClient>>().Provide();
                
                logger.LogDebug("Connecting to the IoT Hub...");
                await deviceClient.OpenAsync();
                logger.LogInformation("The connection to the IoT Hub successfully established.");

                logger.LogDebug("Sending a telemetry message...");
                using var message = new Message(Encoding.UTF8.GetBytes("TestMessage"));
                await deviceClient.SendEventAsync(message);
                logger.LogDebug("A telemetry message was successfully published.");
            }
            catch (ConfigurationException ex)
            {
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                logger.LogError(ex.Message);
                return 1;
            }

            return 0;
        }
    }
}