// <copyright file="FullyQualifyingCSharpHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.DesignTime;

using Microsoft.EntityFrameworkCore.Storage;

#pragma warning disable EF1001 // Internal EF Core API usage. There is no public API to influence how types are referenced in generated code.

/// <summary>
/// A <see cref="Microsoft.EntityFrameworkCore.Design.ICSharpHelper"/> which references the types of OpenMU
/// with their full name in generated code.
/// </summary>
/// <remarks>
/// The entity types of this project have the same names as their base types of the data model, e.g.
/// <see cref="Persistence.EntityFramework.Model.Account"/> and <see cref="DataModel.Entities.Account"/>. The generated code of the
/// compiled models imports both namespaces and references both types, which would be ambiguous with short names.
/// </remarks>
internal class FullyQualifyingCSharpHelper : Microsoft.EntityFrameworkCore.Design.Internal.CSharpHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FullyQualifyingCSharpHelper"/> class.
    /// </summary>
    /// <param name="typeMappingSource">The type mapping source.</param>
    public FullyQualifyingCSharpHelper(ITypeMappingSource typeMappingSource)
        : base(typeMappingSource)
    {
    }

    /// <inheritdoc />
    public override string Reference(Type type, bool? fullName = null)
    {
        return base.Reference(type, fullName ?? ContainsOpenMuType(type));
    }

    private static bool ContainsOpenMuType(Type type)
    {
        if (type.Namespace?.StartsWith("MUnique.OpenMU", StringComparison.Ordinal) is true)
        {
            return true;
        }

        if (type.HasElementType && type.GetElementType() is { } elementType)
        {
            return ContainsOpenMuType(elementType);
        }

        return type.IsGenericType && type.GetGenericArguments().Any(ContainsOpenMuType);
    }
}
