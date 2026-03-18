using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Runtime.Versioning;
using NuGet.Server.Infrastructure;

namespace NuGet.Server.DataServices
{
    [DataServiceKey("Id", "Version")]
    [EntityPropertyMapping("Id", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, keepInContent: false)]
    [EntityPropertyMapping("Authors", SyndicationItemProperty.AuthorName, SyndicationTextContentKind.Plaintext, keepInContent: false)]
    [EntityPropertyMapping("LastUpdated", SyndicationItemProperty.Updated, SyndicationTextContentKind.Plaintext, keepInContent: false)]
    [EntityPropertyMapping("Summary", SyndicationItemProperty.Summary, SyndicationTextContentKind.Plaintext, keepInContent: false)]
    [HasStream]
    public class Package
    {
        public Package(IPackage package, DerivedPackageData derivedData)
        {
            Id = package.Id;
            Version = package.Version.ToString();
            IsPrerelease = !String.IsNullOrEmpty(package.Version.SpecialVersion);
            Title = package.Title;
            Authors = String.Join(",", package.Authors);
            Owners = String.Join(",", package.Owners);
            
            if (package.IconUrl != null)
            {
                IconUrl = package.IconUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            }
            if (package.LicenseUrl != null)
            {
                LicenseUrl = package.LicenseUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            }
            if (package.ProjectUrl != null)
            {
                ProjectUrl = package.ProjectUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            }
            RequireLicenseAcceptance = package.RequireLicenseAcceptance;
            DevelopmentDependency = package.DevelopmentDependency;
            Description = package.Description;
            Summary = package.Summary;
            ReleaseNotes = package.ReleaseNotes;
            Tags = package.Tags;
            Dependencies = String.Join("|", package.DependencySets.SelectMany(ConvertDependencySetToStrings));
            PackageHash = derivedData.PackageHash;
            PackageHashAlgorithm = "SHA512";
            PackageSize = derivedData.PackageSize;
            PackageSource = derivedData.PackageSource;
            LastUpdated = derivedData.LastUpdated.UtcDateTime;
            Published = derivedData.Created.UtcDateTime;
            Path = derivedData.Path;
            FullPath = derivedData.FullPath;
            MinClientVersion = package.MinClientVersion == null ? null : package.MinClientVersion.ToString();
            Listed = package.Listed;

            // set the latest flags based on the derived data
            IsAbsoluteLatestVersion = derivedData.IsAbsoluteLatestVersion;
            IsLatestVersion = derivedData.IsLatestVersion;
          
            //enhancements
            if (package.ProjectSourceUrl != null)
            {
                ProjectSourceUrl = package.ProjectSourceUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            } 
            if (package.PackageSourceUrl != null)
            {
                PackageSourceUrl = package.PackageSourceUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            } 
            if (package.DocsUrl != null)
            {
                DocsUrl = package.DocsUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            } 
            if (package.WikiUrl != null)
            {
                WikiUrl = package.WikiUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            } 
            if (package.MailingListUrl != null)
            {
                MailingListUrl = package.MailingListUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            }
            if (package.BugTrackerUrl != null)
            {
                BugTrackerUrl = package.BugTrackerUrl.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);
            }

            Replaces = String.Join(",", package.Replaces);
            Provides = String.Join(",", package.Provides);
            Conflicts = String.Join(",", package.Conflicts);

            // server metadata                
            IsApproved = package.IsApproved;
            PackageStatus = package.PackageStatus;
            PackageSubmittedStatus = package.PackageSubmittedStatus;
            PackageTestResultStatus = package.PackageTestResultStatus;
            PackageTestResultStatusDate = package.PackageTestResultStatusDate;
            PackageValidationResultStatus = package.PackageValidationResultStatus;
            PackageValidationResultDate = package.PackageValidationResultDate;
            PackageCleanupResultDate = package.PackageCleanupResultDate;
            PackageReviewedDate = package.PackageReviewedDate;
            PackageApprovedDate = package.PackageApprovedDate;
            PackageReviewer = package.PackageReviewer;
            IsDownloadCacheAvailable = package.IsDownloadCacheAvailable;
            DownloadCacheDate = package.DownloadCacheDate;
            DownloadCache = String.Join("|", package.DownloadCache.Select(ConvertDownloadCacheToStrings));

            SoftwareDisplayName = package.SoftwareDisplayName;
            SoftwareDisplayVersion = package.SoftwareDisplayVersion;
        }

        internal string FullPath
        {
            get;
            set;
        }

        internal string Path
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public bool IsPrerelease
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Authors
        {
            get;
            set;
        }

        public string Owners
        {
            get;
            set;
        }

        public string IconUrl
        {
            get;
            set;
        }

        public string LicenseUrl
        {
            get;
            set;
        }

        public string ProjectUrl
        {
            get;
            set;
        }

        public int DownloadCount
        {
            get;
            set;
        }

        public bool RequireLicenseAcceptance
        {
            get;
            set;
        }

        public bool DevelopmentDependency
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }

        public string ReleaseNotes
        {
            get;
            set;
        }

        public DateTime Published
        {
            get;
            set;
        }

        public DateTime LastUpdated
        {
            get;
            set;
        }

        public string Dependencies
        {
            get;
            set;
        }

        public string PackageSource
        {
            get;
            set;
        } 
        
        public string PackageHash
        {
            get;
            set;
        }

        public string PackageHashAlgorithm
        {
            get;
            set;
        }

        public long PackageSize
        {
            get;
            set;
        }

        public string Copyright
        {
            get;
            set;
        }

        public string Tags
        {
            get;
            set;
        }

        public bool IsAbsoluteLatestVersion
        {
            get;
            set;
        }

        public bool IsLatestVersion
        {
            get;
            set;
        }

        public bool Listed
        {
            get;
            set;
        }

        public int VersionDownloadCount
        {
            get;
            set;
        }

        public string MinClientVersion
        {
            get;
            set;
        }

        #region NuSpec Enhancements
        public string ProjectSourceUrl { get; set; }
        public string PackageSourceUrl { get; set; }
        public string DocsUrl { get; set; }
        public string WikiUrl { get; set; }
        public string MailingListUrl { get; set; }
        public string BugTrackerUrl { get; set; }
        public string Replaces { get; set; }
        public string Provides { get; set; }
        public string Conflicts { get; set; }
        // round 2
        public string SoftwareDisplayName { get; set; }
        public string SoftwareDisplayVersion { get; set; }
        #endregion

        #region Server Metadata Only

        public bool IsApproved { get; set; }
        public string PackageStatus { get; set; }
        public string PackageSubmittedStatus { get; set; }
        public string PackageTestResultStatus { get; set; }
        public DateTime? PackageTestResultStatusDate { get; set; }
        public string PackageValidationResultStatus { get; set; }
        public DateTime? PackageValidationResultDate { get; set; }
        public DateTime? PackageCleanupResultDate { get; set; }
        public DateTime? PackageReviewedDate { get; set; }
        public DateTime? PackageApprovedDate { get; set; }
        public string PackageReviewer { get; set; }
        public bool IsDownloadCacheAvailable { get; set; }
        public DateTime? DownloadCacheDate { get; set; }
        public string DownloadCache { get; set; }
        
        #endregion

        private string ConvertDownloadCacheToStrings(DownloadCache cache)
        {
            return String.Format("{0}^{1}^{2}", cache.OriginalUrl, cache.FileName, cache.Checksum);
        }

        private IEnumerable<string> ConvertDependencySetToStrings(PackageDependencySet dependencySet)
        {
            if (dependencySet.Dependencies.Count == 0)
            {
                if (dependencySet.TargetFramework != null)
                {
                    // if this Dependency set is empty, we still need to send down one string of the form "::<target framework>",
                    // so that the client can reconstruct an empty group.
                    return new string[] { String.Format("::{0}", VersionUtility.GetShortFrameworkName(dependencySet.TargetFramework)) };
                }
            }
            else
            {
                return dependencySet.Dependencies.Select(dependency => ConvertDependency(dependency, dependencySet.TargetFramework));
            }

            return new string[0];
        }

        private string ConvertDependency(PackageDependency packageDependency, FrameworkName targetFramework)
        {
            if (targetFramework == null)
            {
                if (packageDependency.VersionSpec == null)
                {
                    return packageDependency.Id;
                }
                else
                {
                    return String.Format("{0}:{1}", packageDependency.Id, packageDependency.VersionSpec);
                }
            }
            else
            {
                return String.Format("{0}:{1}:{2}", packageDependency.Id, packageDependency.VersionSpec, VersionUtility.GetShortFrameworkName(targetFramework));
            }
        }
    }
}