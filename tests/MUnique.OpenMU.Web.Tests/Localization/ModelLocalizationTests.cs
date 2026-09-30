// <copyright file="ModelLocalizationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Localization;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Verifies localization of runtime model types and enum selections.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ModelLocalizationTests
{
    /// <summary>
    /// Tests that persistence-derived types resolve captions from their base models.
    /// </summary>
    [Test]
    public void DerivedModelsUseBaseResources()
    {
        var culture = CultureInfo.GetCultureInfo("zh-CN");
        Assert.That(MUnique.OpenMU.DataModel.Properties.Resources.ResourceManager.GetString("SystemConfiguration_IpResolver_Name", culture), Is.EqualTo("IP 地址解析方式"));
        Assert.That(typeof(DerivedAccount).GetTypeCaption(culture), Is.EqualTo("游戏账号"));
        Assert.That(typeof(DerivedAccount).GetPropertyCaption(nameof(Account.LoginName), culture), Is.EqualTo("登录名"));
        Assert.That(typeof(DerivedAccount).GetPropertyCaption(nameof(Account.PasswordHash), culture), Is.EqualTo("密码哈希值"));
        Assert.That(typeof(DerivedAccount).GetPropertyCaption(nameof(Account.SecurityCode), culture), Is.EqualTo("安全码"));
        Assert.That(typeof(DerivedAccount).GetPropertyCaption(nameof(Account.IsBot), culture), Is.EqualTo("机器人账号"));
        Assert.That(typeof(DerivedAccount).GetPropertyCaption(nameof(Account.LoginName), CultureInfo.GetCultureInfo("en")), Is.EqualTo("Login Name"));
    }

    /// <summary>
    /// Tests that field labels honor explicit captions and localize runtime models.
    /// </summary>
    /// <param name="label">The optional explicit caption.</param>
    /// <param name="expectedCaption">The expected rendered caption.</param>
    [TestCase(null, "登录名")]
    [TestCase("", "登录名")]
    [TestCase("自定义标签", "自定义标签")]
    public void FieldLabelUsesChineseCaption(string? label, string expectedCaption)
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            using var context = new BunitContext();
            var account = new DerivedAccount();
            var component = context.Render<FieldLabel<string>>(parameters => parameters
                .Add(field => field.ValueExpression, () => account.LoginName)
                .Add(field => field.Text, label));
            Assert.That(component.Find("label").TextContent.Trim(), Is.EqualTo(expectedCaption));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    /// <summary>
    /// Tests that translated enum labels preserve the original binding values.
    /// </summary>
    [Test]
    public void EnumLabelsAreLocalizedWithoutChangingValues()
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            using var context = new BunitContext();
            context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
            var account = new Account { State = AccountState.Normal };
            var component = context.Render<EnumSelect<AccountState>>(parameters => parameters
                .Add(field => field.Value, account.State)
                .Add(field => field.ValueExpression, () => account.State)
                .Add(field => field.ValueChanged, value => account.State = value));
            Assert.That(component.Find("option[value='Normal']").TextContent, Is.EqualTo("正常"));
            Assert.That(component.Find("option[value='Banned']").TextContent, Is.EqualTo("封禁"));
            component.Find("select").Change("Banned");
            Assert.That(account.State, Is.EqualTo(AccountState.Banned));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    /// <summary>
    /// Verifies resource-backed field metadata in the connection-server creation form.
    /// </summary>
    /// <param name="culture">The UI culture.</param>
    /// <param name="serverId">The expected server identifier caption.</param>
    /// <param name="description">The expected description caption.</param>
    /// <param name="client">The expected client caption.</param>
    /// <param name="port">The expected port caption.</param>
    [TestCase("zh-CN", "服务器 ID", "说明", "客户端", "监听端口")]
    [TestCase("en", "Server ID", "Description", "Client", "Listening Port")]
    public void ConnectServerCaptionsFollowCulture(string culture, string serverId, string description, string client, string port)
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            Assert.That(new MUnique.OpenMU.DataModel.Configuration.SystemConfiguration().ToString(), Is.EqualTo(culture == "zh-CN" ? "系统配置" : "System Configuration"));
            Assert.That(typeof(MUnique.OpenMU.Network.IpResolverType).GetField("Custom")!.GetCustomAttribute<DisplayAttribute>()!.GetName(), Is.EqualTo(culture == "zh-CN" ? "自定义" : "Custom"));
            var type = typeof(CreateConnectServerConfig.ConnectServerViewModel);
            var expected = new Dictionary<string, string>
            {
                ["ServerId"] = serverId,
                ["Description"] = description,
                ["Client"] = client,
                ["NetworkPort"] = port,
            };
            foreach (var (property, caption) in expected)
            {
                Assert.That(type.GetProperty(property)!.GetCustomAttribute<DisplayAttribute>()?.GetName(), Is.EqualTo(caption));
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    /// <summary>
    /// Verifies weekday captions follow the UI language while preserving enum values.
    /// </summary>
    /// <param name="culture">The UI culture.</param>
    /// <param name="caption">The expected Sunday caption.</param>
    [TestCase("zh-CN", "星期日")]
    [TestCase("en", "Sunday")]
    [TestCase("de", "Sonntag")]
    public void WeekdayLabelsFollowCulture(string culture, string caption)
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            using var context = new BunitContext();
            context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
            var schedule = new MUnique.OpenMU.DataModel.Configuration.CastleSiegeStateScheduleEntry();
            var component = context.Render<EnumSelect<DayOfWeek>>(parameters => parameters
                .Add(field => field.Value, schedule.DayOfWeek)
                .Add(field => field.ValueExpression, () => schedule.DayOfWeek)
                .Add(field => field.ValueChanged, value => schedule.DayOfWeek = value));
            Assert.That(component.Find("option[value='Sunday']").TextContent, Is.EqualTo(caption));
            component.Find("select").Change("Monday");
            Assert.That(schedule.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    /// <summary>
    /// Verifies that generated model summaries follow the selected UI language.
    /// </summary>
    /// <param name="culture">The UI language.</param>
    /// <param name="scheduleText">The expected schedule text.</param>
    /// <param name="upgradeText">The expected upgrade text.</param>
    [TestCase("zh-CN", "结束：星期六 22:05", "等级 2：属性值 300，升级所需守护宝石 3 颗、金币 1000")]
    [TestCase("en", "End on Saturday at 22:05", "Level 2: Value=300, Jewels=3, Zen=1000")]
    public void ModelSummariesFollowCulture(string culture, string scheduleText, string upgradeText)
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            var schedule = new MUnique.OpenMU.DataModel.Configuration.CastleSiegeStateScheduleEntry
            {
                State = MUnique.OpenMU.DataModel.Configuration.CastleSiegeState.End,
                DayOfWeek = DayOfWeek.Saturday,
                Hour = 22,
                Minute = 5,
            };
            var upgrade = new MUnique.OpenMU.DataModel.Configuration.CastleSiegeUpgradeDefinition
            {
                Level = 2,
                Value = 300,
                RequiredJewelOfGuardianCount = 3,
                RequiredZen = 1000,
            };
            Assert.That(schedule.ToString(), Is.EqualTo(scheduleText));
            Assert.That(upgrade.ToString(), Is.EqualTo(upgradeText));
            Assert.That(new ItemStorage { Money = 100 }.ToString(), Is.EqualTo(culture == "zh-CN" ? "0 件物品，100 金币" : "0 Items, 100 Money"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    /// <summary>
    /// Verifies that resources provide Chinese weekdays when platform date names are English.
    /// </summary>
    [Test]
    public void WeekdayResourcesOverridePlatformDateNames()
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            var culture = (CultureInfo)CultureInfo.GetCultureInfo("zh-CN").Clone();
            culture.DateTimeFormat = (DateTimeFormatInfo)CultureInfo.InvariantCulture.DateTimeFormat.Clone();
            CultureInfo.CurrentUICulture = culture;
            Assert.That(CultureHelper.GetDayName(DayOfWeek.Saturday), Is.EqualTo("星期六"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    private sealed class DerivedAccount : Account
    {
    }
}
