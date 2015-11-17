using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace NuGet
{
    /// <summary>
    /// This interface represents the basic interface that one needs to implement in order to
    /// support client certificate authentication. 
    /// </summary>
    public interface IClientCertificateProvider
    {
        /// <summary>
        /// </summary>
        X509Certificate GetCertificate(Uri uri);
    }
}