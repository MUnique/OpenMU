// <copyright file="CashShopPackagePersistenceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Tests compatibility of the existing package name column with localized names.
/// </summary>
[TestFixture]
public class CashShopPackagePersistenceTests
{
    /// <summary>
    /// The compiled model reads old plain names and preserves translations and source metadata on a round trip.
    /// </summary>
    [Test]
    public void NamesUseTheExistingTextColumn()
    {
        var entityType = EntityDataContext.CompleteModel.GetEntityTypes().Single(type => type.ClrType.BaseType == typeof(CashShopPackage));
        var property = entityType.FindProperty(nameof(CashShopPackage.Name))!;
        Assert.That(property.GetColumnType(), Is.EqualTo("text"));
        var converter = property.GetTypeMapping().Converter!;
        var oldName = (LocalizedString)converter.ConvertFromProvider("SD Boost Aura")!;
        Assert.That(oldName.ValueInNeutralLanguage, Is.EqualTo("SD Boost Aura"));
        var chinese = CultureInfo.GetCultureInfo("zh-CN");
        var name = oldName.WithTranslation(chinese, "最大SD提升光环").WithSourceKey("CashShopPackageNames/SDBoostAura").WithSourceStamp();
        var serialized = converter.ConvertToProvider(name);
        Assert.That(serialized, Is.EqualTo(name.Value));
        var restored = (LocalizedString)converter.ConvertFromProvider(serialized)!;
        Assert.That(restored.Value, Is.EqualTo(name.Value));
        Assert.That(restored.GetTranslation(chinese), Is.EqualTo("最大SD提升光环"));
        Assert.That(restored.SourceKey, Is.EqualTo(name.SourceKey));
    }
}
