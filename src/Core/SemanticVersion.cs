﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NuGet.Resources;

namespace NuGet
{
    /// <summary>
    /// A hybrid implementation of SemVer that supports semantic versioning as described at http://semver.org while not strictly enforcing it to
    /// allow older 4-digit versioning schemes to continue working.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(SemanticVersionTypeConverter))]
    public sealed class SemanticVersion : IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        private const RegexOptions _flags = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace;
        // private const RegexOptions _flags = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

        // Versions containing up to 4 digits
        private static readonly Regex _semanticVersionRegex = new Regex(@"^
(?<Version>\d+(\s*\.\s*\d+){0,3})
(?<Prerelease>-([0]\b|[0]$|[0][0-9]*[A-Za-z-]+|[1-9A-Za-z-][0-9A-Za-z-]*)+(\.([0]\b|[0]$|[0][0-9]*[A-Za-z-]+|[1-9A-Za-z-][0-9A-Za-z-]*)+)*)?
(?<Metadata>\+[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?
(?<PackageVersion>_\d+)?
$", _flags);

//        private static readonly Regex _semanticVersionRegex = new Regex(@"^
//(?<Version>\d+(\s*\.\s*\d+){0,3})
//(?<Prerelease>-[a-z][0-9a-z-]*)?
//(?<PackageVersion>_\d+)?
//$", _flags);

        // Strict SemVer 2.0.0 format, this may contain only 3 digits.
        private static readonly Regex _strictSemanticVersionRegex = new Regex(@"^
(?<Version>([0-9]|[1-9][0-9]*)(\.([0-9]|[1-9][0-9]*)){2})
(?<Prerelease>-([0]\b|[0]$|[0][0-9]*[A-Za-z-]+|[1-9A-Za-z-][0-9A-Za-z-]*)+(\.([0]\b|[0]$|[0][0-9]*[A-Za-z-]+|[1-9A-Za-z-][0-9A-Za-z-]*)+)*)?
(?<Metadata>\+[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?
(?<PackageVersion>_\d+)?
$", _flags);

//        private static readonly Regex _strictSemanticVersionRegex = new Regex(@"^
//(?<Version>\d+(\.\d+){2})
//(?<Prerelease>-[a-z][0-9a-z-]*)?
//(?<PackageVersion>_\d+)?
//$", _flags);

        private readonly string _originalString;
        private string _normalizedVersionString;

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="version">Version string.</param>
        public SemanticVersion(string version)
            : this(Parse(version))
        {
            // The constructor normalizes the version string so that it we do not need to normalize it every time we need to operate on it.
            // The original string represents the original form in which the version is represented to be used when printing.
            _originalString = version;
        }

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="major">Major version X.y.z</param>
        /// <param name="minor">Minor version x.Y.z</param>
        /// <param name="build">Patch version x.y.Z</param>
        /// <param name="revision">Revision version x.y.z.R</param>
        public SemanticVersion(int major, int minor, int build, int revision)
            : this(new Version(major, minor, build, revision))
        {
        }

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="major">Major version X.y.z</param>
        /// <param name="minor">Minor version x.Y.z</param>
        /// <param name="build">Patch version x.y.Z</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        public SemanticVersion(int major, int minor, int build, string specialVersion)
            : this(new Version(major, minor, build), specialVersion, packageReleaseVersion:0)
        {
        }

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="major">Major version X.y.z</param>
        /// <param name="minor">Minor version x.Y.z</param>
        /// <param name="build">Patch version x.y.Z</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        /// <param name="packageReleaseVersion">Package release version</param>
        public SemanticVersion(int major, int minor, int build, string specialVersion, int packageReleaseVersion)
            : this(new Version(major, minor, build), specialVersion, packageReleaseVersion)
        {
        }

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="major">Major version X.y.z</param>
        /// <param name="minor">Minor version x.Y.z</param>
        /// <param name="build">Patch version x.y.Z</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        /// <param name="metadata">Build metadata</param>
        public SemanticVersion(int major, int minor, int build, string specialVersion, string metadata)
            : this(new Version(major, minor, build), specialVersion, metadata)
        {
        }
      
        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="major">Major version X.y.z</param>
        /// <param name="minor">Minor version x.Y.z</param>
        /// <param name="build">Patch version x.y.Z</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        /// <param name="metadata">Build metadata</param>
        /// <param name="packageReleaseVersion">Package release version</param>
        public SemanticVersion(int major, int minor, int build, string specialVersion, string metadata, int packageReleaseVersion)
            : this(new Version(major, minor, build), specialVersion, metadata, packageReleaseVersion, originalString: null)
        {
        }

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="version">Built-in version.</param>
        public SemanticVersion(Version version)
            : this(version, specialVersion: string.Empty, metadata: string.Empty, packageReleaseVersion: 0)
        {
        }

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="version">Built-in version</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        public SemanticVersion(Version version, string specialVersion)
            : this(version, specialVersion, metadata: string.Empty, packageReleaseVersion: 0, originalString: null)
        {
        }

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="version">Built-in version</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        /// <param name="metadata">Build metadata</param>
        public SemanticVersion(Version version, string specialVersion, string metadata)
         : this(version, specialVersion, metadata, packageReleaseVersion: 0, originalString: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="version">Built-in version</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        /// <param name="packageReleaseVersion">The package release version</param>
        public SemanticVersion(Version version, string specialVersion, int packageReleaseVersion)
            : this(version, specialVersion, metadata: string.Empty, packageReleaseVersion: packageReleaseVersion, originalString: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        /// <param name="metadata">Build metadata</param>
        /// <param name="packageReleaseVersion">The package release version.</param>
        public SemanticVersion(Version version, string specialVersion, string metadata, int packageReleaseVersion)
            : this(version, specialVersion, metadata, packageReleaseVersion, originalString:null)
        {
        }

        /// <summary>
        /// SemanticVersion
        /// </summary>
        /// <param name="version">Built-in version</param>
        /// <param name="specialVersion">Prerelease versioning</param>
        /// <param name="metadata">Build metadata</param>
        /// <param name="packageReleaseVersion">Package release version</param>
        /// <param name="originalString">Original version string</param>
        private SemanticVersion(Version version, string specialVersion, string metadata, int packageReleaseVersion, string originalString)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            Version = NormalizeVersionValue(version);
            SpecialVersion = specialVersion ?? string.Empty;
            Metadata = metadata ?? string.Empty;
            PackageReleaseVersion = packageReleaseVersion;

            _originalString = string.IsNullOrWhiteSpace(originalString) ? version.ToString()
                + (!string.IsNullOrWhiteSpace(specialVersion) ? '-' + specialVersion : null)
                + (!string.IsNullOrWhiteSpace(metadata) ? '+' + metadata : null)
                + (packageReleaseVersion != 0 ? '_' + packageReleaseVersion.ToString() : null) 
                : originalString;
        }

        internal SemanticVersion(SemanticVersion semVer)
        {
            _originalString = semVer.ToOriginalString();
            Version = semVer.Version;
            SpecialVersion = semVer.SpecialVersion;
            Metadata = semVer.Metadata;
            PackageReleaseVersion = semVer.PackageReleaseVersion;
        }

        /// <summary>
        /// Gets the normalized version portion.
        /// </summary>
        public Version Version { get; private set; }

        /// <summary>
        /// Gets the optional release label. For SemVer 2.0.0 this may contain multiple '.' separated parts.
        /// </summary>
        public string SpecialVersion { get; private set; }

        public int PackageReleaseVersion { get; private set; }

        /// <summary>
        /// SemVer 2.0.0 build metadata. This is not used for comparing or sorting.
        /// </summary>
        public string Metadata
        {
            get;
            private set;
        }

        public string[] GetOriginalVersionComponents()
        {
            if (!String.IsNullOrEmpty(_originalString))
            {
                string original = _originalString;

                // search the start of the SpecialVersion part or metadata, if any
                int labelIndex = original.IndexOfAny(new char[] { '-', '+' });
                if (labelIndex != -1)
                {
                    // remove the SpecialVersion or metadata part
                    original = original.Substring(0, labelIndex);
                }

                // search the start of the ReleaseVersion part, if any
                int packageFixIndex = original.IndexOf('_');
                if (packageFixIndex != -1)
                {
                    // remove the PackageReleaseVersion part
                    original = original.Substring(0, packageFixIndex);
                }

                return SplitAndPadVersionString(original);
            }
            else
            {
                return SplitAndPadVersionString(Version.ToString());
            }
        }

        private static string[] SplitAndPadVersionString(string version)
        {
            string[] a = version.Split('.');
            if (a.Length == 4)
            {
                return a;
            }
            else
            {
                // if 'a' has less than 4 elements, we pad the '0' at the end
                // to make it 4.
                var b = new string[4] { "0", "0", "0", "0" };
                Array.Copy(a, 0, b, 0, a.Length);
                return b;
            }
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
        /// </summary>
        public static SemanticVersion Parse(string version)
        {
            if (String.IsNullOrEmpty(version))
            {
                throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "version");
            }

            SemanticVersion semVer;
            if (!TryParse(version, out semVer))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, NuGetResources.InvalidVersionString, version), "version");
            }
            return semVer;
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
        /// </summary>
        public static bool TryParse(string version, out SemanticVersion value)
        {
            return TryParseInternal(version, _semanticVersionRegex, out value);
        }

        /// <summary>
        /// Parses a version string using strict semantic versioning rules that allows exactly 3 components and an optional special version.
        /// </summary>
        public static bool TryParseStrict(string version, out SemanticVersion value)
        {
            return TryParseInternal(version, _strictSemanticVersionRegex, out value);
        }

        private static bool TryParseInternal(string version, Regex regex, out SemanticVersion semVer)
        {
            semVer = null;
            if (String.IsNullOrEmpty(version))
            {
                return false;
            }

            var match = regex.Match(version.Trim());
            Version versionValue;
            if (!match.Success || !Version.TryParse(match.Groups["Version"].Value, out versionValue))
            {
                return false;
            }

            //semVer = new SemanticVersion(
            //    NormalizeVersionValue(versionValue),
            //    RemoveLeadingChar(match.Groups["Prerelease"].Value),
            //    RemoveLeadingChar(match.Groups["Metadata"].Value),
            //    version.Replace(" ", ""));
  
            semVer = new SemanticVersion(
                NormalizeVersionValue(versionValue), 
                RemoveLeadingChar(match.Groups["Prerelease"].Value),
                RemoveLeadingChar(match.Groups["Metadata"].Value), 
                TryParseNumeric(RemoveLeadingChar(match.Groups["PackageVersion"].Value)), 
                version.Replace(" ", ""));
          
            return true;
        }

        // Remove the -, _, or + from a version section.
        private static string RemoveLeadingChar(string s)
        {
            if (s != null && s.Length > 0)
            {
                return s.Substring(1, s.Length - 1);
            }

            return s;
        }

        private static int TryParseNumeric(string possibleNumeric)
        {
            if (string.IsNullOrWhiteSpace(possibleNumeric)) return 0;

            var numericalValue = 0;
            int.TryParse(possibleNumeric, out numericalValue);

            return numericalValue;
        }

        /// <summary>
        /// Attempts to parse the version token as a SemanticVersion.
        /// </summary>
        /// <returns>An instance of SemanticVersion if it parses correctly, null otherwise.</returns>
        public static SemanticVersion ParseOptionalVersion(string version)
        {
            SemanticVersion semVer;
            TryParse(version, out semVer);
            return semVer;
        }

        private static Version NormalizeVersionValue(Version version)
        {
            return new Version(version.Major,
                               version.Minor,
                               Math.Max(version.Build, 0),
                               Math.Max(version.Revision, 0));
        }

        public int CompareTo(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return 1;
            }
            SemanticVersion other = obj as SemanticVersion;
            if (other == null)
            {
                throw new ArgumentException(NuGetResources.TypeMustBeASemanticVersion, "obj");
            }
            return CompareTo(other);
        }

        public int CompareTo(SemanticVersion other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return 1;
            }

            int versionResult = Version.CompareTo(other.Version);
            if (versionResult != 0)
            {
                return versionResult;
            }

            int packageReleaseVersionResult = PackageReleaseVersion.CompareTo(other.PackageReleaseVersion);

            bool empty = string.IsNullOrEmpty(SpecialVersion);
            bool otherEmpty = string.IsNullOrEmpty(other.SpecialVersion);
            if (empty && otherEmpty)
            {
                return packageReleaseVersionResult;
            }
            if (empty && !otherEmpty)
            {
                return 1;
            }
            if (otherEmpty && !empty)
            {
                return -1;
            }
            
            // Compare the release labels using SemVer 2.0.0 comparision rules.
            var releaseLabels = SpecialVersion.Split('.');
            var otherReleaseLabels = other.SpecialVersion.Split('.');

            var releasev2Result = CompareReleaseLabels(releaseLabels, otherReleaseLabels);
            if (releasev2Result == 0)
            {
                return packageReleaseVersionResult;
            }
            else
            {
                return releasev2Result;
            }
        }

        public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
        {
            if (Object.ReferenceEquals(version1, null))
            {
                return Object.ReferenceEquals(version2, null);
            }
            return version1.Equals(version2);
        }

        public static bool operator !=(SemanticVersion version1, SemanticVersion version2)
        {
            return !(version1 == version2);
        }

        public static bool operator <(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 == null)
            {
                throw new ArgumentNullException("version1");
            }
            return version1.CompareTo(version2) < 0;
        }

        public static bool operator <=(SemanticVersion version1, SemanticVersion version2)
        {
            return (version1 == version2) || (version1 < version2);
        }

        public static bool operator >(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 == null)
            {
                throw new ArgumentNullException("version1");
            }
            return version2 < version1;
        }

        public static bool operator >=(SemanticVersion version1, SemanticVersion version2)
        {
            return (version1 == version2) || (version1 > version2);
        }

        /// <summary>
        /// Returns the original version string without build metadata.
        /// </summary>
        /// <remarks>SemVer 2.0.0 versions using build metadata or multiple release labels will be normalized.
        /// SemVer 1.0.0 versions cannot be normalized in this method for backwards compatibility reasons.</remarks>
        public override string ToString()
        {
            if (IsSemVer2())
            {
                // Normalize semver2 to match NuGet.Versioning
                return ToNormalizedString();
            }
            
            // Remove metadata from the original string if it exists.
            var plusIndex = _originalString.IndexOf('+');

            if (plusIndex > -1)
            {
                var versionMinusMetadata = _originalString.Substring(0, plusIndex);
                if (PackageReleaseVersion !=0)
                {
                    versionMinusMetadata += "_" + PackageReleaseVersion.ToString();
                }

                return versionMinusMetadata;
            }
            
            return _originalString;
        }

        /// <summary>
        /// Returns the normalized string representation of this instance of <see cref="SemanticVersion"/>.
        /// If the instance can be strictly parsed as a <see cref="SemanticVersion"/>, the normalized version
        /// string if of the format {major}.{minor}.{build}[-{special-version}]. If the instance has a non-zero
        /// value for <see cref="System.Version.Revision"/>, the format is {major}.{minor}.{build}.{revision}[-{special-version}].
        /// </summary>
        /// <remarks>Build metadata is not included.</remarks>
        /// <returns>The normalized string representation.</returns>
        public string ToNormalizedString()
        {
            if (_normalizedVersionString == null)
            {
                var builder = new StringBuilder();
                builder
                    .Append(Version.Major)
                    .Append('.')
                    .Append(Version.Minor)
                    .Append('.')
                    .Append(Math.Max(0, Version.Build));

                if (Version.Revision > 0)
                {
                    builder.Append('.')
                           .Append(Version.Revision);
                }

                if (!string.IsNullOrEmpty(SpecialVersion))
                {
                    builder.Append('-')
                           .Append(SpecialVersion);
                } 
                
                // Rob added this
                //if (!string.IsNullOrEmpty(Metadata))
                //{
                //    builder.Append('+')
                //           .Append(Metadata);
                //}

                if (PackageReleaseVersion != 0)
                {
                    builder.Append('_')
                           .Append(PackageReleaseVersion);
                }

                _normalizedVersionString = builder.ToString();
            }

            return _normalizedVersionString;
        }

        /// <summary>
        /// Returns the full normalized string including build metadata.
        /// </summary>
        public string ToFullString()
        {
            var s = ToNormalizedString();

            if (!string.IsNullOrWhiteSpace(Metadata))
            {
                var underscoreIndex = s.IndexOf('_');

                if (underscoreIndex > -1)
                {
                    s = s.Substring(0, underscoreIndex);
                }

                s = string.Format(CultureInfo.InvariantCulture, "{0}+{1}", s, Metadata);

                if (PackageReleaseVersion != 0)
                {
                    s += "_" + PackageReleaseVersion.ToString();
                }
            }

            return s;
        }

        /// <summary>
        /// Returns the original string used to construct the version. This includes metadata.
        /// </summary>
        public string ToOriginalString()
        {
            return _originalString;
        }

        /// <summary>
        /// True if the version contains metadata or multiple release labels.
        /// </summary>
        public bool IsSemVer2()
        {
            return !string.IsNullOrEmpty(Metadata)
                || (!string.IsNullOrEmpty(SpecialVersion) && SpecialVersion.Contains("."));
        }

        public bool Equals(SemanticVersion other)
        {
            return !Object.ReferenceEquals(null, other) &&
                   Version.Equals(other.Version) &&
                   SpecialVersion.Equals(other.SpecialVersion, StringComparison.OrdinalIgnoreCase) &&
                   PackageReleaseVersion.Equals(other.PackageReleaseVersion);

            // Rob: for some reason the devs did not want to compare metadata
            // && Metadata.Equals(other.Metadata, StringComparison.OrdinalIgnoreCase)
        }

        public override bool Equals(object obj)
        {
            SemanticVersion semVer = obj as SemanticVersion;
            return !Object.ReferenceEquals(null, semVer) && Equals(semVer);
        }

        public override int GetHashCode()
        {
            int hashCode = Version.GetHashCode();
            if (SpecialVersion != null)
            {
                hashCode = hashCode * 4567 + SpecialVersion.GetHashCode();
            }

            // Rob: for some reason the devs did not want to compare metadata
            //if (Metadata != null)
            //{
            //    hashCode = hashCode * 7890 + Metadata.GetHashCode();
            //}

            if (PackageReleaseVersion != 0)
            {
                hashCode = hashCode * 123 + PackageReleaseVersion.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Compares sets of release labels.
        /// </summary>
        private static int CompareReleaseLabels(IEnumerable<string> version1, IEnumerable<string> version2)
        {
            var result = 0;

            var a = version1.GetEnumerator();
            var b = version2.GetEnumerator();

            var aExists = a.MoveNext();
            var bExists = b.MoveNext();

            while (aExists || bExists)
            {
                if (!aExists && bExists)
                {
                    return -1;
                }

                if (aExists && !bExists)
                {
                    return 1;
                }

                // compare the labels
                result = CompareRelease(a.Current, b.Current);

                if (result != 0)
                {
                    return result;
                }

                aExists = a.MoveNext();
                bExists = b.MoveNext();
            }

            return result;
        }

        /// <summary>
        /// Release labels are compared as numbers if they are numeric, otherwise they will be compared
        /// as strings.
        /// </summary>
        private static int CompareRelease(string version1, string version2)
        {
            var version1Num = 0;
            var version2Num = 0;
            var result = 0;

            // check if the identifiers are numeric
            var v1IsNumeric = Int32.TryParse(version1, out version1Num);
            var v2IsNumeric = Int32.TryParse(version2, out version2Num);

            // if both are numeric compare them as numbers
            if (v1IsNumeric && v2IsNumeric)
            {
                result = version1Num.CompareTo(version2Num);
            }
            else if (v1IsNumeric || v2IsNumeric)
            {
                // numeric labels come before alpha labels
                if (v1IsNumeric)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            else
            {
                // Ignoring 2.0.0 case sensitive compare. Everything will be compared case insensitively as 2.0.1 specifies.
                result = StringComparer.OrdinalIgnoreCase.Compare(version1, version2);
            }

            return result;
        }
    }
}
