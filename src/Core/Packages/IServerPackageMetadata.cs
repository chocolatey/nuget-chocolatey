using System;

namespace NuGet
{
    public partial interface IServerPackageMetadata
    {
        Uri ReportAbuseUrl { get; }
        int DownloadCount { get; }
    }
}
