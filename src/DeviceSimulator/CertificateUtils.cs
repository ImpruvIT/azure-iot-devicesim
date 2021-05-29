using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ImpruvIT.Azure.IoT.DeviceSimulator.Configuration;

namespace ImpruvIT.Azure.IoT.DeviceSimulator
{
    public static class CertificateUtils
    {
        public static X509Certificate2Collection LoadIdentityCertificateChain(this Options options)
        {
            return LoadCertificateChain(options.CertificatePath, options.CertificatePassword);
        }
        
        private static X509Certificate2Collection LoadCertificateChain(string path, string? password)
        {
            var chain = new X509Certificate2Collection();
            chain.Import(path, password);
            return chain;
        }

        public static X509Certificate2 GetIdentityCertificate(this X509Certificate2Collection chain)
        {
            var certificate = chain.Cast<X509Certificate2>().Last();
            if (!certificate.HasPrivateKey)
            {
                throw new InvalidOperationException("The leaf certificate has to have private key attached.");
            }

            return certificate;
        }
    }
}