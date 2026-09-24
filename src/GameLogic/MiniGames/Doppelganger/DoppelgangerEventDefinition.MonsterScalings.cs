// <copyright file="DoppelgangerEventDefinition.MonsterScalings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

/// <content>
/// The default multipliers for the monsters, taken from the original season 6 server data.
/// </content>
public partial class DoppelgangerEventDefinition
{
    /// <summary>
    /// Creates the multipliers of the monsters of the original season 6 event. Each entry applies
    /// up to a player level (including the master level) and contains the multipliers for the
    /// level, health, damage and defense for one to five players.
    /// Higher player levels use the last entry.
    /// </summary>
    private static IList<DoppelgangerMonsterScaling> CreateDefaultMonsterScalings()
    {
        return
        [
            CreateScaling(10, [1f, 1f, 1f, 1f, 1f], [1f, 2f, 3f, 3.8f, 4.8f], [1f, 2f, 3f, 3.7f, 4.7f], [1f, 2f, 3f, 3.7f, 4.5f]),
            CreateScaling(20, [1.2f, 1.2f, 1.2f, 1.2f, 1.2f], [2f, 2.2f, 3.2f, 4f, 5f], [1.15f, 2.05f, 3.05f, 3.75f, 4.75f], [1.35f, 2.15f, 3.15f, 3.85f, 4.65f]),
            CreateScaling(30, [1.6f, 1.6f, 1.6f, 1.6f, 1.6f], [2.7f, 2.4f, 3.4f, 4.2f, 5.2f], [1.2f, 2.1f, 3.1f, 3.8f, 4.8f], [1.5f, 2.3f, 3.3f, 4f, 4.8f]),
            CreateScaling(40, [1.9f, 1.9f, 1.9f, 1.9f, 1.9f], [3.4f, 2.6f, 3.6f, 4.4f, 5.4f], [1.23f, 2.15f, 3.15f, 3.85f, 4.85f], [1.65f, 2.45f, 3.45f, 4.15f, 4.95f]),
            CreateScaling(50, [2.2f, 2.2f, 2.2f, 2.2f, 2.2f], [4.1f, 2.8f, 3.8f, 4.6f, 5.6f], [1.43f, 2.18f, 3.18f, 3.88f, 4.88f], [1.8f, 2.6f, 3.6f, 4.3f, 5.1f]),
            CreateScaling(60, [2.5f, 2.5f, 2.5f, 2.5f, 2.5f], [4.8f, 3.5f, 4.5f, 5.3f, 6.3f], [1.63f, 2.38f, 3.38f, 4.08f, 5.08f], [1.95f, 2.75f, 3.75f, 4.45f, 5.25f]),
            CreateScaling(70, [2.7f, 2.7f, 2.7f, 2.7f, 2.7f], [5.5f, 4f, 5f, 5.8f, 6.8f], [1.83f, 2.58f, 3.58f, 4.28f, 5.28f], [2.1f, 2.9f, 3.9f, 4.6f, 5.4f]),
            CreateScaling(80, [3f, 3f, 3f, 3f, 3f], [6.2f, 4.5f, 5.5f, 6.3f, 7.3f], [2.1f, 2.78f, 3.78f, 4.48f, 5.48f], [2.25f, 3.05f, 4.05f, 4.75f, 5.55f]),
            CreateScaling(90, [3.3f, 3.3f, 3.3f, 3.3f, 3.3f], [6.9f, 5f, 6f, 6.8f, 7.8f], [2.31f, 2.98f, 3.98f, 4.68f, 5.68f], [2.4f, 3.2f, 4.2f, 4.9f, 5.8f]),
            CreateScaling(100, [3.6f, 3.6f, 3.6f, 3.6f, 3.6f], [7.6f, 6f, 7f, 7.8f, 8.8f], [2.51f, 3.28f, 4.28f, 4.98f, 5.98f], [2.55f, 3.35f, 4.35f, 5.05f, 6.85f]),
            CreateScaling(110, [3.65f, 3.65f, 3.65f, 3.65f, 3.65f], [8.3f, 6.5f, 7.5f, 8.3f, 9.3f], [2.71f, 3.48f, 4.48f, 5.18f, 6.18f], [2.7f, 3.5f, 4.5f, 5.2f, 6f]),
            CreateScaling(120, [3.69f, 3.69f, 3.69f, 3.69f, 3.69f], [9f, 6.7f, 7.7f, 8.5f, 9.5f], [2.91f, 3.68f, 4.68f, 5.38f, 6.38f], [2.85f, 3.65f, 4.65f, 5.35f, 6.15f]),
            CreateScaling(130, [3.73f, 3.73f, 3.73f, 3.73f, 3.73f], [9.7f, 6.9f, 7.9f, 8.7f, 9.7f], [3.11f, 3.88f, 4.88f, 5.58f, 6.58f], [3f, 3.8f, 4.8f, 5.5f, 6.3f]),
            CreateScaling(140, [3.77f, 3.77f, 3.77f, 3.77f, 3.77f], [10.4f, 7.1f, 8.1f, 8.9f, 9.9f], [3.31f, 4.08f, 5.08f, 5.78f, 6.78f], [3.15f, 3.95f, 4.95f, 5.65f, 6.45f]),
            CreateScaling(150, [3.81f, 3.81f, 3.81f, 3.81f, 3.81f], [11.1f, 7.3f, 8.3f, 9.1f, 10.1f], [3.51f, 4.28f, 5.28f, 5.98f, 6.98f], [3.3f, 4.1f, 5.1f, 5.8f, 6.6f]),
            CreateScaling(160, [3.85f, 3.85f, 3.85f, 3.85f, 3.85f], [11.8f, 8f, 9f, 9.8f, 10.8f], [3.71f, 4.48f, 5.48f, 6.18f, 7.18f], [3.45f, 4.25f, 5.25f, 5.95f, 6.75f]),
            CreateScaling(170, [3.89f, 3.89f, 3.89f, 3.89f, 3.89f], [12.5f, 9f, 10f, 10.8f, 11.8f], [3.81f, 4.68f, 5.68f, 6.38f, 7.38f], [4.6f, 4.4f, 5.4f, 6.1f, 6.9f]),
            CreateScaling(180, [3.93f, 3.93f, 3.93f, 3.93f, 3.93f], [13.2f, 11f, 12f, 11.8f, 12.8f], [3.91f, 6f, 6.4f, 6.5f, 6.9f], [4.65f, 5.3f, 6.3f, 6.7f, 7f]),
            CreateScaling(190, [3.97f, 3.97f, 3.97f, 3.97f, 3.97f], [13.5f, 12f, 13f, 12.8f, 13.8f], [4.08f, 6.4f, 6.8f, 6.7f, 7.18f], [4.7f, 5.8f, 6.4f, 6.7f, 7.3f]),
            CreateScaling(200, [4.01f, 4.01f, 4.01f, 4.01f, 4.01f], [15f, 15f, 15f, 14.8f, 14.8f], [4.28f, 6.8f, 7f, 6.9f, 7.28f], [4.85f, 6.4f, 7f, 7.1f, 7.5f]),
            CreateScaling(210, [4.05f, 4.05f, 4.05f, 4.05f, 4.05f], [16f, 17f, 18f, 18.8f, 18.8f], [4.48f, 7f, 7.2f, 7.1f, 7.38f], [5f, 7f, 7.5f, 7.5f, 7.8f]),
            CreateScaling(220, [5f, 4.09f, 4.09f, 4.09f, 4.09f], [17.8f, 18f, 19f, 19.8f, 20.6f], [4.68f, 7.01f, 6.8f, 6.58f, 7.58f], [6.4f, 7.2f, 7.7f, 7.7f, 7.9f]),
            CreateScaling(230, [5.05f, 4.13f, 4.13f, 4.13f, 4.13f], [18.8f, 19f, 20f, 20.8f, 21.6f], [4.88f, 7.02f, 7f, 6.78f, 7.78f], [6.6f, 7.4f, 7.9f, 7.9f, 8.1f]),
            CreateScaling(240, [5.1f, 4.17f, 4.17f, 4.17f, 4.17f], [19.8f, 20f, 21f, 21.8f, 22.6f], [5.08f, 7.03f, 7.1f, 6.98f, 7.98f], [6.8f, 7.6f, 8.1f, 8.1f, 8.3f]),
            CreateScaling(250, [5.15f, 4.21f, 4.21f, 4.21f, 4.21f], [20.8f, 21f, 22f, 22.8f, 23.6f], [5.28f, 7.04f, 7.28f, 7.18f, 8.18f], [7.2f, 7.8f, 8.3f, 8.3f, 8.5f]),
            CreateScaling(260, [5.2f, 4.25f, 4.25f, 4.25f, 4.25f], [21.8f, 22f, 23f, 23.8f, 24.6f], [5.48f, 7.05f, 7.48f, 7.38f, 8.38f], [7.6f, 8f, 8.5f, 8.5f, 8.7f]),
            CreateScaling(270, [5.25f, 4.29f, 4.29f, 4.29f, 4.29f], [22.8f, 23f, 24f, 24.8f, 25.6f], [5.68f, 7.06f, 7.68f, 7.58f, 8.58f], [8f, 8.2f, 8.7f, 8.7f, 8.9f]),
            CreateScaling(280, [5.3f, 4.33f, 4.33f, 4.33f, 4.33f], [23.8f, 24f, 25f, 25.8f, 26.6f], [5.88f, 7.07f, 7.88f, 7.78f, 8.78f], [8.2f, 8.4f, 8.9f, 8.9f, 9.1f]),
            CreateScaling(290, [5.35f, 4.37f, 4.37f, 4.37f, 4.37f], [25.8f, 25f, 26f, 26.8f, 27.6f], [6.18f, 7.08f, 8.08f, 7.98f, 8.98f], [8.6f, 8.6f, 9.1f, 9.1f, 9.3f]),
            CreateScaling(300, [5.4f, 4.41f, 4.41f, 4.41f, 4.41f], [26.2f, 26f, 27f, 27.8f, 28.6f], [6.28f, 7.28f, 8.28f, 8.18f, 9.18f], [8.7f, 8.8f, 9.3f, 9.3f, 9.5f]),
            CreateScaling(310, [5.45f, 4.45f, 4.45f, 4.45f, 4.45f], [26.8f, 27f, 28f, 28.8f, 29.6f], [6.48f, 7.48f, 8.48f, 8.38f, 9.38f], [8.8f, 9f, 9.5f, 9.5f, 9.7f]),
            CreateScaling(320, [5.5f, 4.49f, 4.49f, 4.49f, 4.49f], [27.8f, 28f, 29f, 29.8f, 30.6f], [6.68f, 7.68f, 8.68f, 8.58f, 9.58f], [9f, 9.2f, 9.7f, 9.7f, 9.9f]),
            CreateScaling(330, [5.55f, 4.53f, 4.53f, 4.53f, 4.53f], [29.8f, 29.5f, 30f, 30.8f, 31.6f], [6.98f, 7.88f, 8.88f, 8.78f, 9.78f], [9.4f, 9.4f, 9.9f, 9.9f, 10.1f]),
            CreateScaling(340, [5.6f, 4.57f, 4.57f, 4.57f, 4.57f], [30.8f, 30.5f, 31f, 31.8f, 32.6f], [7.18f, 8.08f, 9.08f, 9.05f, 10.05f], [9.6f, 9.6f, 10.1f, 10.1f, 10.3f]),
            CreateScaling(350, [5.65f, 4.61f, 4.61f, 4.61f, 4.61f], [31.8f, 31.5f, 32f, 32.8f, 33.6f], [7.38f, 8.35f, 9.35f, 9.18f, 10.18f], [9.8f, 9.8f, 10.3f, 10.3f, 10.5f]),
            CreateScaling(360, [5.7f, 4.65f, 4.65f, 4.65f, 4.65f], [32.8f, 32.5f, 33f, 33.8f, 34.6f], [7.58f, 8.48f, 9.48f, 9.38f, 10.38f], [10f, 10f, 10.5f, 10.5f, 10.7f]),
            CreateScaling(370, [5.75f, 4.69f, 4.69f, 4.69f, 4.69f], [33.8f, 33.5f, 34f, 34.8f, 35.6f], [7.78f, 8.68f, 9.68f, 9.58f, 10.58f], [10.2f, 10.2f, 10.7f, 10.7f, 10.9f]),
            CreateScaling(380, [5.8f, 4.73f, 4.73f, 4.73f, 4.73f], [34.8f, 34.5f, 35f, 35.8f, 36.6f], [7.98f, 8.88f, 9.88f, 9.78f, 10.78f], [10.4f, 10.4f, 10.9f, 10.9f, 11.1f]),
            CreateScaling(390, [5.85f, 4.77f, 4.77f, 4.77f, 4.77f], [35.8f, 35.5f, 36f, 36.8f, 37.6f], [8.18f, 9.08f, 10.08f, 9.98f, 10.98f], [10.6f, 10.6f, 11.1f, 11.1f, 11.3f]),
            CreateScaling(400, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [38f, 37f, 38f, 38.8f, 39.8f], [8.48f, 9.28f, 10.28f, 10.18f, 11.18f], [11.6f, 11.5f, 11.4f, 11.4f, 11.5f]),
            CreateScaling(410, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [39f, 41f, 42f, 42.8f, 43.8f], [8.58f, 9.68f, 10.78f, 10.38f, 11.38f], [12f, 12.5f, 12.8f, 12.5f, 12.3f]),
            CreateScaling(420, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [42f, 50f, 52f, 61.8f, 66.8f], [8.68f, 9.88f, 10.78f, 10.58f, 11.58f], [12.3f, 13.5f, 15f, 15.7f, 17.5f]),
            CreateScaling(430, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [47f, 52f, 57f, 66.8f, 71.8f], [8.88f, 9.98f, 10.88f, 10.78f, 11.78f], [12.6f, 14f, 15.5f, 16.2f, 18f]),
            CreateScaling(440, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [53f, 58f, 63f, 72.8f, 77.8f], [9.08f, 10.08f, 11.08f, 10.98f, 11.98f], [12.9f, 14.3f, 16.3f, 17f, 18.8f]),
            CreateScaling(450, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [59f, 64f, 69f, 78.8f, 83.8f], [9.38f, 10.28f, 11.28f, 11.18f, 12.18f], [13.1f, 15.1f, 17.1f, 17.8f, 19.6f]),
            CreateScaling(460, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [69f, 70f, 75f, 84.8f, 89.8f], [9.98f, 11f, 12f, 12.38f, 13.38f], [14f, 15.9f, 17.9f, 18.6f, 20.4f]),
            CreateScaling(470, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [76f, 76f, 81f, 90.8f, 95.8f], [10.28f, 12f, 13f, 13.1f, 13.7f], [14.7f, 16.7f, 18.7f, 19.4f, 21.2f]),
            CreateScaling(480, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [82f, 82f, 87f, 96.8f, 100.8f], [10.58f, 12.5f, 13.5f, 13.5f, 14f], [15.2f, 17.5f, 19.5f, 20.2f, 22f]),
            CreateScaling(490, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [88f, 88f, 93f, 101.8f, 106.8f], [10.88f, 13f, 13.8f, 13.7f, 14.3f], [16f, 18.3f, 20.3f, 21f, 22.8f]),
            CreateScaling(500, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [93f, 94f, 99f, 107.8f, 112.8f], [11.18f, 13.4f, 14.1f, 14f, 14.6f], [16.7f, 19.1f, 21.1f, 21.8f, 23.6f]),
            CreateScaling(510, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [95f, 100f, 105f, 113.8f, 118.8f], [11.38f, 13.7f, 14.4f, 14.3f, 14.9f], [17.4f, 19.9f, 21.9f, 22.6f, 24.4f]),
            CreateScaling(520, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [101f, 106f, 111f, 119.8f, 124.8f], [11.48f, 14f, 14.7f, 14.6f, 15.2f], [17.7f, 20.7f, 22.7f, 23.4f, 25.2f]),
            CreateScaling(530, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [107f, 112f, 117f, 125.8f, 130.8f], [11.78f, 14.3f, 15f, 14.9f, 15.5f], [18.5f, 21.5f, 23.5f, 24.2f, 26f]),
            CreateScaling(540, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [113f, 118f, 123f, 131.8f, 136.8f], [12.08f, 14.6f, 15.3f, 15.2f, 15.8f], [19.3f, 22.3f, 24.3f, 25f, 26.8f]),
            CreateScaling(550, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [119f, 124f, 129f, 137.8f, 142.8f], [12.38f, 14.9f, 15.6f, 15.5f, 16.1f], [20.1f, 23.1f, 25.1f, 25.8f, 27.6f]),
            CreateScaling(560, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [125f, 130f, 135f, 143.8f, 148.8f], [12.68f, 15.2f, 15.9f, 15.8f, 16.4f], [20.9f, 23.9f, 25.9f, 26.6f, 28.4f]),
            CreateScaling(570, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [131f, 136f, 141f, 149.8f, 154.8f], [12.98f, 15.5f, 16.2f, 16.1f, 16.7f], [21.7f, 24.7f, 26.7f, 27.4f, 29.2f]),
            CreateScaling(580, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [137f, 142f, 147f, 155.8f, 160.8f], [13.28f, 15.8f, 16.5f, 16.4f, 17f], [22.5f, 25.5f, 27.5f, 28.2f, 30f]),
            CreateScaling(590, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [143f, 148f, 153f, 161.8f, 166.8f], [13.58f, 16.1f, 16.8f, 16.7f, 17.3f], [23.3f, 26.3f, 28.3f, 29f, 30.8f]),
            CreateScaling(600, [6.79f, 6.79f, 6.79f, 6.79f, 6.79f], [149f, 157f, 162f, 167.8f, 172.8f], [13.88f, 16.6f, 17.2f, 17f, 17.6f], [24.1f, 28.1f, 29.1f, 29.8f, 31.6f]),
        ];
    }

    private static DoppelgangerMonsterScaling CreateScaling(int maximumPlayerLevel, float[] level, float[] health, float[] damage, float[] defense)
    {
        return new DoppelgangerMonsterScaling
        {
            MaximumPlayerLevel = maximumPlayerLevel,
            LevelMultipliers = level,
            HealthMultipliers = health,
            DamageMultipliers = damage,
            DefenseMultipliers = defense,
        };
    }
}
