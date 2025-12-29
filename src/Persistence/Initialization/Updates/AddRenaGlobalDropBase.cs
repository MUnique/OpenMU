// Adds a global drop group for Rena on Season 1 maps

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;

public abstract class AddRenaGlobalDropBase : UpdatePlugInBase
{
    protected const short RenaGroup = 14;
    protected const short RenaNumber = 21;
    protected const short RenaGlobalDropId = 32010;

    protected void EnsureRenaExists(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Items.Any(i => i.Group == RenaGroup && i.Number == RenaNumber))
        {
            return;
        }

        var itemDefinition = context.CreateNew<ItemDefinition>();
        itemDefinition.Name = "Rena";
        itemDefinition.Number = RenaNumber;
        itemDefinition.Group = (byte)RenaGroup;
        itemDefinition.DropsFromMonsters = false;
        itemDefinition.Durability = 255; // allow stacking (255 pieces)
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        itemDefinition.SetGuid(itemDefinition.Group, itemDefinition.Number);
        gameConfiguration.Items.Add(itemDefinition);
    }

    protected DropItemGroup EnsureRenaDropGroup(IContext context, GameConfiguration gameConfiguration)
    {
        var id = GuidHelper.CreateGuid<DropItemGroup>(RenaGlobalDropId);
        if (gameConfiguration.DropItemGroups.FirstOrDefault(g => g.GetId() == id) is { } existing)
        {
            return existing;
        }

        var rena = gameConfiguration.Items.First(i => i.Group == RenaGroup && i.Number == RenaNumber);
        var drop = context.CreateNew<DropItemGroup>();
        drop.SetGuid(RenaGlobalDropId);
        drop.Description = "Rena Global Drop (S1 maps)";
        drop.Chance = 0.002; // 0.2 %
        drop.PossibleItems.Add(rena);
        gameConfiguration.DropItemGroups.Add(drop);
        return drop;
    }

    protected void AttachToSeason1Maps(GameConfiguration gameConfiguration, DropItemGroup drop)
    {
        string[] s1Maps =
        {
            "Lorencia",
            "Noria",
            "Devias",
            "Dungeon",
            "Lost Tower",
            "Atlans",
            "Arena",
            "Exile",
        };

        foreach (var map in gameConfiguration.Maps.Where(m => s1Maps.Contains(m.Name)))
        {
            if (!map.DropItemGroups.Contains(drop))
            {
                map.DropItemGroups.Add(drop);
            }
        }
    }
}
