using System;
using System.Globalization;
using System.IO;
using NuGet.Resources;

namespace NuGet
{
    public class PackageDownloader : IPackageDownloader 
    {
        private const string DefaultUserAgentClient = "NuGet Core";

        public event EventHandler<ProgressEventArgs> ProgressAvailable = delegate { };
        public event EventHandler<WebRequestEventArgs> SendingRequest = delegate { };

        public string CurrentDownloadPackageId
        {
            get;
            protected set;
        }

        public virtual void DownloadPackage(Uri uri, IPackageMetadata package, Stream targetStream)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            var downloadClient = new HttpClient(uri)
                                 {
                                     UserAgent = HttpUtility.CreateUserAgentString(DefaultUserAgentClient)
                                 };
            DownloadPackage(downloadClient, package, targetStream);
        }

        public virtual void DownloadPackage(IHttpClient downloadClient, IPackageName package, Stream targetStream)
        {
            if (downloadClient == null)
            {
                throw new ArgumentNullException("downloadClient");
            }

            if (targetStream == null)
            {
                throw new ArgumentNullException("targetStream");
            }

            // Get the operation display text
            string operation = String.Format(CultureInfo.CurrentCulture, NuGetResources.DownloadProgressStatus, package.Id, package.Version);
            CurrentDownloadPackageId = package.Id;

            EventHandler<ProgressEventArgs> progressAvailableHandler = (sender, e) =>
            {
                OnPackageDownloadProgress(new ProgressEventArgs(operation, e.PercentComplete));
            };

            try
            {
                downloadClient.ProgressAvailable += progressAvailableHandler;
                downloadClient.SendingRequest += OnSendingRequest;

                downloadClient.DownloadData(targetStream);
            }
            finally
            {
                downloadClient.ProgressAvailable -= progressAvailableHandler;
                downloadClient.SendingRequest -= OnSendingRequest;
                CurrentDownloadPackageId = null;
            }
        }

        protected virtual void OnPackageDownloadProgress(ProgressEventArgs e)
        {
            ProgressAvailable(this, e);
        }

        protected virtual void OnSendingRequest(object sender, WebRequestEventArgs webRequestArgs)
        {
            SendingRequest(this, webRequestArgs);
        }
    }
}