using CommandLine;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.Configuration
{
    public class Options
    {
        [Option('c', "connection", Required = false, HelpText = "An IoT Hub connection string.")]
        public string? ConnectionString { get; set; }
        
        [Option('h', "host", Required = false, HelpText = "An IoT Hub hostname.")]
        public string? HubHostname { get; set; }
        
        [Option('g', "gateway", Required = false, HelpText = "A gateway hostname.")]
        public string? GatewayHostname { get; set; }
        
        [Option('d', "device", Required = false, HelpText = "A device id.")]
        public string? DeviceId { get; set; }
        
        [Option('k', "key", Required = false, HelpText = "A symmetric key for authentication.")]
        public string? SymmetricKey { get; set; }
        
        [Option("key2", Required = false, HelpText = "A secondary symmetric key for authentication.")]
        public string? SymmetricKey2 { get; set; }
        
        [Option('x', "cert", Required = false, HelpText = "A path to PFX file containing identity certificate.")]
        public string? CertificatePath { get; set; }
        
        [Option('p', "password", Required = false, HelpText = "Certificate PFX file password.")]
        public string? CertificatePassword { get; set; }
        
        [Option("dps", Required = false, HelpText = "A DPS endpoint.")]
        public string? DpsHostname { get; set; }
        
        [Option('s', "scope", Required = false, HelpText = "A DPS scope id.")]
        public string? ScopeId { get; set; }
        
        [Option('r', "registration", Required = false, HelpText = "A DPS registration id.")]
        public string? RegistrationId { get; set; }

        [Option('i', "individual", Required = false, HelpText = "The DPS enrollment is of individual type.")]
        public bool IsIndividual { get; set; }
    }
}