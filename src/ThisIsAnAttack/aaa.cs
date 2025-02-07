
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Diagnostics.CodeAnalysis
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    [System.AttributeUsage(validOn: AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute
    { }
}

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Runtime.CompilerServices
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Specifies that a type has required members or that a member is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute
    { }

    /// <summary>
    /// Indicates that compiler support for a particular feature is required for the location where this attribute is applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            this.FeatureName = featureName;
        }

        /// <summary>
        /// The name of the compiler feature.
        /// </summary>
        public string FeatureName { get; }

        /// <summary>
        /// If true, the compiler can choose to allow access to the location where this attribute is applied if it does not understand <see cref="FeatureName"/>.
        /// </summary>
        public bool IsOptional { get; init; }

        /// <summary>
        /// The <see cref="FeatureName"/> used for the ref structs C# feature.
        /// </summary>
        public const string RefStructs = nameof(RefStructs);

        /// <summary>
        /// The <see cref="FeatureName"/> used for the required members C# feature.
        /// </summary>
        public const string RequiredMembers = nameof(RequiredMembers);
    }

}
