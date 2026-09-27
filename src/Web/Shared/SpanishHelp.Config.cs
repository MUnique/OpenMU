// <copyright file="SpanishHelp.Config.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

/// <summary>
/// Spanish help of the general game configuration and of the maps.
/// </summary>
public static partial class SpanishHelp
{
    private static void AddGameConfiguration(Dictionary<string, Entry> e)
    {
        // Game configuration > General.
        Add(e, "GameConfiguration.ExperienceRate", "Multiplicador de experiencia del juego para el nivel normal.", "Es el rate del servidor: con 25, cada monstruo da 25 veces su experiencia base. Se multiplica además por el rate de cada servidor de juego y por el multiplicador del mapa. El Happy Hour se suma encima.");
        Add(e, "GameConfiguration.MasterExperienceRate", "Multiplicador de experiencia para el Master Level.", "Cambia qué tan rápido se sube el Master Level una vez alcanzado el nivel máximo.");
        Add(e, "GameConfiguration.ExperienceFormula", "Fórmula de la experiencia total necesaria para llegar a cada nivel. La variable es level.", "Cambiarla vuelve más lento o más rápido subir de nivel, y afecta a todos los jugadores, incluso a los que ya tienen experiencia. Conviene tocar el rate antes que esta fórmula.");
        Add(e, "GameConfiguration.MasterExperienceFormula", "Fórmula de la experiencia necesaria para cada Master Level. La variable es level.", "Igual que la fórmula normal, pero para el Master Level.");
        Add(e, "GameConfiguration.MaximumLevel", "Nivel máximo que se puede alcanzar.", "En el nivel máximo, las clases Master empiezan a ganar experiencia de Master Level.");
        Add(e, "GameConfiguration.MaximumMasterLevel", "Master Level máximo que se puede alcanzar.", "Define hasta dónde progresa un personaje Master. Se ve también en la web.");
        Add(e, "GameConfiguration.MinimumMonsterLevelForMasterExperience", "Nivel mínimo del monstruo para dar experiencia de Master Level.", "Evita farmear Master Level en monstruos muy débiles.");
        Add(e, "GameConfiguration.PreventExperienceOverflow", "Si está activo, cuando la experiencia ganada supera lo necesario para el nivel siguiente, solo se gana lo justo y el sobrante se descarta. Si no, el sobrante se aplica a los niveles siguientes (comportamiento normal).", "Con las rates altas, activarlo evita que un solo monstruo haga subir varios niveles de golpe.");
        Add(e, "GameConfiguration.MaximumCharactersPerAccount", "Cantidad máxima de personajes por cuenta.", "Se muestra en la web. Bajarlo no borra personajes ya creados.");
        Add(e, "GameConfiguration.MaximumPartySize", "Cantidad máxima de integrantes de una party.", "Afecta cómo se reparte la experiencia en grupo y la fuerza de los eventos en party.");
        Add(e, "GameConfiguration.MaximumInventoryMoney", "Máximo de Zen que puede llevar un personaje en el inventario.");
        Add(e, "GameConfiguration.MaximumVaultMoney", "Máximo de Zen que se puede guardar en el baúl.");
        Add(e, "GameConfiguration.ShouldDropMoney", "Si está activo, los monstruos sueltan el Zen en el suelo. Si no, se suma directamente al personaje.");
        Add(e, "GameConfiguration.ClampMoneyOnPickup", "Si está activo, al recoger Zen se llena hasta el máximo permitido en vez de rechazar el pozo completo.");
        Add(e, "GameConfiguration.ItemDropDuration", "Tiempo que un ítem permanece en el suelo antes de desaparecer.", "Más tiempo da más margen a los jugadores para recogerlo.");
        Add(e, "GameConfiguration.InfoRange", "Distancia a la que un jugador ve a otros objetos del juego.", "Más rango es más carga para el servidor y para la red.");
        Add(e, "GameConfiguration.RecoveryInterval", "Cada cuánto se recuperan vida, maná y AG.", "Un intervalo menor hace que se recuperen más rápido.");
        Add(e, "GameConfiguration.DamagePerOneItemDurability", "Daño recibido acumulado que hace falta para gastar 1 punto de durabilidad de un ítem defensivo.", "Un valor mayor hace que las armaduras duren más.");
        Add(e, "GameConfiguration.DamagePerOnePetDurability", "Daño recibido acumulado que hace falta para gastar 1 punto de durabilidad de una mascota.");
        Add(e, "GameConfiguration.HitsPerOneItemDurability", "Cantidad de golpes que hacen falta para gastar 1 punto de durabilidad de un arma.", "Un valor mayor hace que las armas duren más.");
        Add(e, "GameConfiguration.AreaSkillHitsPlayer", "Si está activo, las habilidades de área también golpean a otros jugadores.", "Afecta el PvP en zonas donde hay combate entre jugadores.");
        Add(e, "GameConfiguration.MaximumItemOptionLevelDrop", "Nivel máximo de opción de ítem que puede salir en los drops.", "Bajarlo achica el poder de los ítems que caen.");
        Add(e, "GameConfiguration.ExcellentItemDropLevelDelta", "Diferencia de nivel que hace falta entre el monstruo y el ítem para que este entre en los drops excellent.", "Un valor mayor exige monstruos más fuertes para soltar ítems excellent de cada nivel.");
        Add(e, "GameConfiguration.LetterSendPrice", "Precio en Zen de enviar una carta.");
        Add(e, "GameConfiguration.MaximumLetters", "Máximo de cartas que puede tener un jugador en su bandeja.");
        Add(e, "GameConfiguration.MaximumPasswordLength", "Largo máximo de la contraseña.");
        Add(e, "GameConfiguration.CharacterNameRegex", "Expresión regular que deben cumplir los nombres de personaje.", "Sirve para bloquear caracteres raros o nombres inválidos al crear personajes.");
        Add(e, "GameConfiguration.Monsters", "Lista de todos los monstruos y NPC.");
        Add(e, "GameConfiguration.Items", "Lista de todas las definiciones de ítems.");
        Add(e, "GameConfiguration.Maps", "Lista de todos los mapas.");
        Add(e, "GameConfiguration.Skills", "Lista de todas las habilidades.");
        Add(e, "GameConfiguration.CharacterClasses", "Lista de las clases de personaje.");
        Add(e, "GameConfiguration.DropItemGroups", "Grupos de drop que se pueden asignar a mapas y personajes.");
        Add(e, "GameConfiguration.PlugInConfigurations", "Configuración de los plugins (comandos, eventos, Happy Hour, etc.). Se edita mejor desde la pantalla Plugins.");

        // Founder Experience Bonus plugin (Plugins page).
        Add(e, "FounderBonusConfiguration.CutoffDate", "Fecha y hora (en UTC) hasta la cual una cuenta cuenta como Fundadora.", "Tiene que coincidir con la apertura de la beta que muestra la web (variable BETA_OPENS_AT), convertida a UTC. Por ejemplo, 12/10 10:00 hora Argentina es 12/10 13:00 UTC.");
        Add(e, "FounderBonusConfiguration.BonusMultiplier", "Multiplicador de experiencia extra para las cuentas Fundadoras. 1.05 es +5%.", "Se aplica sobre la experiencia normal y la de Master Level, encima de las demás bonificaciones. Ponerlo en 1.00 deja el plugin activo sin dar ningún extra.");
    }

    private static void AddMaps(Dictionary<string, Entry> e)
    {
        Add(e, "GameMapDefinition.Name", "Nombre del mapa para el servidor y el panel.", "El nombre que ve el jugador sale del cliente.");
        Add(e, "GameMapDefinition.Number", "Número del mapa. El cliente lo usa para cargar el terreno y los objetos.", "No se debe cambiar en mapas existentes.");
        Add(e, "GameMapDefinition.ExpMultiplier", "Multiplicador de experiencia que se aplica solo en este mapa.", "Se multiplica con el rate del servidor. Un valor mayor a 1 hace del mapa un lugar de farmeo más rápido. Sirve para guiar a los jugadores hacia ciertos mapas.");
        Add(e, "GameMapDefinition.MonsterSpawns", "Zonas donde aparecen monstruos en este mapa.", "Cambian cuántos monstruos hay y dónde. Se ven y editan más fácil en el Map editor.");
        Add(e, "GameMapDefinition.MapRequirements", "Requisitos para entrar al mapa: nivel, Zen, clase, etc.", "Bajarlos abre el mapa a más jugadores.");
        Add(e, "GameMapDefinition.EnterGates", "Puertas de entrada: los puntos desde los que el jugador pasa a otros mapas.");
        Add(e, "GameMapDefinition.ExitGates", "Puntos de aparición en el mapa, a los que llegan los jugadores al entrar o al reaparecer.");
        Add(e, "GameMapDefinition.SafezoneMap", "Mapa al que se lleva a un jugador cuando muere.", "Define dónde reaparece el jugador tras morir.");
        Add(e, "GameMapDefinition.DropItemGroups", "Grupos de drop propios del mapa. Por ejemplo, Land of Trials suelta ítems ancient y Kanturu suelta gemas.", "Sirven para que un mapa tenga un botín distinto al general.");
        Add(e, "GameMapDefinition.BattleZone", "Zona de batalla del mapa, normalmente solo definida para Arena.");
        Add(e, "GameMapDefinition.CharacterPowerUpDefinitions", "Bonificaciones que reciben los personajes mientras están en este mapa.", "Permite crear mapas con efectos propios, como más daño o más defensa.");
        Add(e, "GameMapDefinition.TerrainData", "Datos del terreno (caminable, zona segura, etc.). No se edita a mano.");
        Add(e, "GameMapDefinition.Discriminator", "Permite distinguir varias definiciones del mismo mapa. Se deja como está.");
    }
}
