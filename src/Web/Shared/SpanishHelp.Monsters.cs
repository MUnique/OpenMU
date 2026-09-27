// <copyright file="SpanishHelp.Monsters.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

/// <summary>
/// Spanish help of monsters, NPCs, their spawn areas and drops.
/// </summary>
public static partial class SpanishHelp
{
    private static void AddMonsters(Dictionary<string, Entry> e)
    {
        // Monsters and NPCs (Game configuration > Monsters).
        Add(e, "MonsterDefinition.Number", "Número único del monstruo o NPC. El cliente lo usa para mostrar su modelo y su nombre.", "No conviene cambiarlo en monstruos existentes: el jugador vería otro modelo o nombre.");
        Add(e, "MonsterDefinition.Designation", "Nombre del monstruo para el servidor y los registros (logs).", "No se muestra en el juego: el nombre que ve el jugador sale del cliente.");
        Add(e, "MonsterDefinition.ObjectKind", "Tipo de objeto: monstruo que ataca, NPC pacífico, comerciante, puerta, trampa, etc.", "Define si el objeto pelea, si se puede atacar y cómo interactúan los jugadores con él.");
        Add(e, "MonsterDefinition.Attributes", "Estadísticas del monstruo: nivel, vida, daño mínimo y máximo, defensa, tasa de ataque y de defensa, resistencias.", "Es la forma directa de hacerlo más fácil o más difícil. El nivel también influye en la experiencia que da y en qué ítems suelta.");
        Add(e, "MonsterDefinition.AttackSkill", "Habilidad con la que ataca el monstruo (también llamada tipo de ataque).", "Cambia cómo pega: cuerpo a cuerpo, a distancia o en área.");
        Add(e, "MonsterDefinition.AttackRange", "Distancia (en casillas) desde la que ataca sin acercarse más al objetivo.", "Con más alcance, ataca desde más lejos y es más peligroso para quien lo pelea a distancia.");
        Add(e, "MonsterDefinition.AttackDelay", "Tiempo entre un ataque y el siguiente.", "Un valor menor lo hace atacar más rápido y pegar más por segundo.");
        Add(e, "MonsterDefinition.MoveDelay", "Tiempo que tarda en dar cada paso.", "Un valor menor lo hace moverse más rápido y alcanzar antes a los jugadores.");
        Add(e, "MonsterDefinition.MoveRange", "Radio dentro del que se mueve al azar. Hoy el servidor no lo usa.");
        Add(e, "MonsterDefinition.ViewRange", "Distancia a la que el monstruo detecta a los jugadores.", "Con más alcance, empieza a perseguirte desde más lejos.");
        Add(e, "MonsterDefinition.RespawnDelay", "Tiempo que pasa hasta que reaparece un monstruo muerto.", "Bajarlo aumenta cuántos monstruos hay para farmear en un mapa y por lo tanto la experiencia y los drops por hora.");
        Add(e, "MonsterDefinition.NumberOfMaximumItemDrops", "Cantidad máxima de ítems que suelta al morir.", "Subirlo hace que cada monstruo deje más botín. Afecta directamente la economía del servidor.");
        Add(e, "MonsterDefinition.DropItemGroups", "Grupos de drop especiales de este monstruo. Por ejemplo, Kundun puede soltar ítems ancient.", "Sirven para darle botín propio a un monstruo, además de los drops generales.");
        Add(e, "MonsterDefinition.MerchantStore", "Ítems que vende este NPC. Solo aplica a NPC comerciantes.", "Cambia lo que los jugadores pueden comprar. Se edita más cómodo en Merchant stores.");
        Add(e, "MonsterDefinition.NpcWindow", "Ventana que se abre al hablar con el NPC (tienda, baúl, combinaciones, etc.).");
        Add(e, "MonsterDefinition.ItemCraftings", "Combinaciones que ofrece este NPC. Solo aplica a NPC de crafteo, como el Chaos Goblin.");
        Add(e, "MonsterDefinition.Quests", "Misiones que se pueden iniciar hablando con este NPC.");
        Add(e, "MonsterDefinition.Buffs", "Bendiciones (buffs) que este NPC puede otorgar.");
        Add(e, "MonsterDefinition.IntelligenceTypeName", "Comportamiento especial del monstruo o NPC (una implementación de inteligencia). Vacío es el comportamiento normal.");
        Add(e, "MonsterDefinition.Attribute", "Atributo interno del monstruo. Se deja como está.");

        // Where monsters live (Game configuration > Game maps > Monster spawns, or the map editor).
        Add(e, "MonsterSpawnArea.MonsterDefinition", "Qué monstruo aparece en esta zona.");
        Add(e, "MonsterSpawnArea.GameMap", "Mapa donde está la zona.");
        Add(e, "MonsterSpawnArea.Quantity", "Cantidad de monstruos que aparecen en la zona.", "Subirlo llena el lugar de monstruos: más experiencia y drops por hora, pero también más carga para el servidor y más competencia entre jugadores. Si la zona es chica, quedan apilados.");
        Add(e, "MonsterSpawnArea.X1", "Coordenada X de la esquina superior izquierda de la zona.", "Junto con Y1, X2 e Y2 define el rectángulo donde aparecen. Con un rectángulo más grande, los monstruos quedan más repartidos.");
        Add(e, "MonsterSpawnArea.Y1", "Coordenada Y de la esquina superior izquierda de la zona.");
        Add(e, "MonsterSpawnArea.X2", "Coordenada X de la esquina inferior derecha de la zona.");
        Add(e, "MonsterSpawnArea.Y2", "Coordenada Y de la esquina inferior derecha de la zona.");
        Add(e, "MonsterSpawnArea.Direction", "Hacia dónde mira el monstruo al aparecer. Sirve sobre todo para NPC.");
        Add(e, "MonsterSpawnArea.SpawnTrigger", "Cuándo aparecen los monstruos: siempre, o solo durante un evento u oleada.", "Los monstruos de eventos no aparecen fuera del evento aunque la zona exista.");
        Add(e, "MonsterSpawnArea.WaveNumber", "Oleada de un evento a la que pertenece la zona.");
        Add(e, "MonsterSpawnArea.MaximumHealthOverride", "Vida máxima solo para esta zona. Vacío usa la vida normal del monstruo.", "Permite armar jefes más duros sin tocar al monstruo en el resto de los mapas.");

        // Drops (Game configuration > Drop item groups).
        Add(e, "DropItemGroup.Description", "Descripción del grupo de drop, para reconocerlo en el panel.");
        Add(e, "DropItemGroup.Chance", "Probabilidad de que se aplique el grupo cuando muere un monstruo, de 0.0 a 1.0 (0.1 es un 10%).", "Es la palanca principal del botín: subirla hace que este grupo caiga más seguido. Los cambios se sienten enseguida en la economía.");
        Add(e, "DropItemGroup.ItemType", "Tipo de botín del grupo: Zen, ítems al azar, joyas, ítems excellent, ancient, etc.");
        Add(e, "DropItemGroup.ItemLevel", "Nivel de mejora con el que caen los ítems del grupo.", "Un nivel más alto hace los ítems más valiosos.");
        Add(e, "DropItemGroup.MinimumMonsterLevel", "Nivel mínimo del monstruo para que se aplique el grupo. Vacío significa que no hay mínimo.");
        Add(e, "DropItemGroup.MaximumMonsterLevel", "Nivel máximo del monstruo para que se aplique el grupo. Vacío significa que no hay máximo.");
        Add(e, "DropItemGroup.Monster", "Monstruo específico al que aplica el grupo. Vacío significa que vale para todos.");
        Add(e, "DropItemGroup.PossibleItems", "Ítems que este grupo puede soltar. Si está vacío, se elige entre los ítems que cumplen las condiciones del grupo.", "Definirlo permite armar premios cerrados, como una caja con ítems puntuales.");
    }
}
