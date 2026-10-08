// <copyright file="MerchantNameTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Localization;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Web.AdminPanel.Pages;

/// <summary>
/// Ensures the merchant grid displays the selected translation instead of serialized data.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MerchantNameTests
{
    /// <summary>
    /// The same merchant follows the selected culture without changing its stored name.
    /// </summary>
    [Test]
    public void MerchantNameUsesCurrentCulture()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            var merchant = new MonsterDefinition { Designation = "Hanzo The Blacksmith||zh-CN=铁匠汉斯||@src=MerchantNames/HanzoTheBlacksmith" };
            var model = new Merchants.MerchantStorageViewModel(merchant);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("zh-CN");
            Assert.That(model.Name, Is.EqualTo("铁匠汉斯"));
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Assert.That(model.Name, Is.EqualTo("Hanzo The Blacksmith"));
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Assert.That(model.Name, Is.EqualTo("Hanzo The Blacksmith"));
            Assert.That(merchant.Designation.Value, Is.EqualTo("Hanzo The Blacksmith||zh-CN=铁匠汉斯||@src=MerchantNames/HanzoTheBlacksmith"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }
}
