namespace NuGet
{
    using System;
    using System.IO;

    public interface IPackageDownloader : IHttpClientEvents
    {
        string CurrentDownloadPackageId { get; }
        void DownloadPackage(Uri uri, IPackageMetadata package, Stream targetStream);
        void DownloadPackage(IHttpClient downloadClient, IPackageName package, Stream targetStream);
    }
}