namespace NuGet
{
    using System;
    using System.Collections.Generic;

    public partial interface IServerPackageMetadata
    {
        /*
            Created = p.Created,
            GalleryDetailsUrl = siteRoot + "packages/" + p.PackageRegistration.Id + "/" + p.Version,
            IsPrerelease = p.IsPrerelease,
            LastUpdated = p.LastUpdated,
         */

        string PackageSource { get; }
        string PackageHash { get; }
        string PackageHashAlgorithm { get; }
        long PackageSize { get; }
        int VersionDownloadCount { get; }

        bool IsApproved { get;  }
        string PackageStatus { get;  }
        string PackageSubmittedStatus { get;  }
        string PackageTestResultStatus { get;  }
        DateTime? PackageTestResultStatusDate { get;  }
        string PackageValidationResultStatus { get;  }
        DateTime? PackageValidationResultDate { get;  }
        DateTime? PackageCleanupResultDate { get;  }
        DateTime? PackageReviewedDate { get;  }
        DateTime? PackageApprovedDate { get;  }
        string PackageReviewer { get;  }

        bool IsDownloadCacheAvailable { get; }
        DateTime? DownloadCacheDate { get; }
        IEnumerable<DownloadCache> DownloadCache { get; }
    }
}