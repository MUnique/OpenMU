// <copyright file="GameConfigurationLoaderGeneratorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
using System.Runtime.CompilerServices;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.DesignTime;

/// <summary>
/// Tests for the generated code of the <c>GameConfigurationLoader</c>.
/// </summary>
[TestFixture]
public class GameConfigurationLoaderGeneratorTest
{
    /// <summary>
    /// The name of the environment variable which updates the generated code when it's set to <c>1</c>.
    /// </summary>
    private const string UpdateVariable = "OPENMU_UPDATE_GENERATED_CODE";

    /// <summary>
    /// Tests that the generated code of the loader matches the current model of the <see cref="EntityDataContext"/>.
    /// </summary>
    [Test]
    public void GeneratedCodeIsUpToDate()
    {
        var generated = GameConfigurationLoaderGenerator.Generate(EntityDataContext.CompleteModel);
        var path = GetGeneratedFilePath();
        if (Environment.GetEnvironmentVariable(UpdateVariable) == "1")
        {
            File.WriteAllText(path, generated);
            Assert.Pass($"The generated code was updated: {path}");
        }

        Assert.That(
            File.ReadAllText(path).ReplaceLineEndings("\n"),
            Is.EqualTo(generated),
            $"The generated code of the GameConfigurationLoader is outdated. Run this test with the environment variable {UpdateVariable}=1 to update it, see src/Persistence/EntityFramework/Loading/Readme.md.");
    }

    private static string GetGeneratedFilePath([CallerFilePath] string testFilePath = "")
    {
        return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(testFilePath)!, "..", "..", "src", "Persistence", "EntityFramework", "Loading", "GameConfigurationLoader.Generated.cs"));
    }
}
