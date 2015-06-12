namespace NuGet
{
    using System;
    using System.Collections.Generic;

    public partial interface IPackageMetadata
    {
        Uri ProjectSourceUrl { get; }
        Uri PackageSourceUrl { get; }
        Uri DocsUrl { get; }
        Uri WikiUrl { get; }
        Uri MailingListUrl { get; }
        Uri BugTrackerUrl { get; }
        IEnumerable<string> Replaces { get; }
        IEnumerable<string> Provides { get; }
        IEnumerable<string> Conflicts { get; }
    }
}