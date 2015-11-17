using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace NuGet
{
    public class NullClientCertificateProvider : IClientCertificateProvider
    {
        private static readonly NullClientCertificateProvider _instance = new NullClientCertificateProvider();

        public static IClientCertificateProvider Instance
        {
            get
            {
                return _instance;
            }
        }

        private NullClientCertificateProvider()
        {

        }

        public X509Certificate GetCertificate(Uri uri)
        {
            return null;
        }
    }
}
