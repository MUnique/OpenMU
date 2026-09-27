// <copyright file="SpanishHelp.Items.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

/// <summary>
/// Spanish help of items: the item editor (merchant stores, vaults, ...) and item definitions.
/// </summary>
public static partial class SpanishHelp
{
    private static void AddItems(Dictionary<string, Entry> e)
    {
        // The item editor: one concrete item (in a merchant store, an inventory, ...).
        Add(e, "ItemViewModel.ItemSlot", "Número de casilla donde se ubica el ítem. Se cuenta de izquierda a derecha y de arriba hacia abajo, empezando en 0.", "Un ítem ocupa tantas casillas como su ancho por su alto. Evitá superponer ítems en una tienda o en un inventario.");
        Add(e, "ItemViewModel.Definition", "Qué ítem es (una espada, una poción, una joya...). Se elige de la lista de definiciones de ítems.", "Cambiarlo puede dejar sin sentido el nivel, las opciones y la cantidad ya cargados.");
        Add(e, "ItemViewModel.Level", "Nivel de mejora del ítem (+0, +1... hasta +15). Solo aparece si el ítem se puede mejorar.", "Sube el daño o la defensa del ítem y también sus requisitos. En una tienda de comerciante se vende ya con ese nivel.");
        Add(e, "ItemViewModel.Amount", "Cantidad de unidades de la pila (pociones, joyas, flechas...). El máximo es la Durability de la definición del ítem.", "Para vender pilas más grandes primero hay que subir la Durability del ítem en Game configuration > Items. El jugador recibe la pila completa al comprar y el precio sube con la cantidad.");
        Add(e, "ItemViewModel.Durability", "Durabilidad actual del ítem equipable.", "Baja con el uso y se repara con los NPC. El máximo depende del ítem y de su nivel.");
        Add(e, "ItemViewModel.PetExperience", "Experiencia acumulada de una mascota entrenable (Dark Raven, Dark Horse).", "Determina el nivel de la mascota.");
        Add(e, "ItemViewModel.HasSkill", "Indica si el ítem otorga su habilidad (por ejemplo la de un arma) mientras está equipado.", "Sin esto el jugador no puede usar esa habilidad con el arma.");
        Add(e, "ItemViewModel.HasLuck", "Agrega la opción Luck (Suerte) al ítem.", "Es una ventaja de combate y de mejora. Cuidado si se ofrece gratis o en una tienda.");
        Add(e, "ItemViewModel.AncientSet", "Set Ancient al que pertenece el ítem.", "Da bonificaciones extra cuando el jugador junta varias piezas del mismo set.");

        // A concrete item (data model).
        Add(e, "Item.Definition", "Qué ítem es. Define su nombre, tamaño, requisitos y poder base.");
        Add(e, "Item.Durability", "Durabilidad actual. En ítems apilables es la cantidad de unidades de la pila.");
        Add(e, "Item.HasSkill", "Indica si el ítem otorga su habilidad mientras está equipado.");
        Add(e, "Item.ItemOptions", "Opciones del ítem (excellent, luck, skill, harmony, sockets...).", "Cada opción cambia los atributos del ítem y su valor para los jugadores.");
        Add(e, "Item.ItemSetGroups", "Set Ancient aplicado al ítem.");
        Add(e, "Item.ItemSlot", "Casilla del inventario, el baúl o la tienda donde está el ítem.");
        Add(e, "Item.Level", "Nivel de mejora del ítem (+0 a +15).", "Sube su daño o defensa y sus requisitos.");
        Add(e, "Item.PetExperience", "Experiencia de la mascota. Solo aplica a mascotas entrenables.");
        Add(e, "Item.SocketCount", "Cantidad de sockets del ítem.", "Limita cuántas opciones de socket puede llevar.");
        Add(e, "Item.StorePrice", "Precio que el propio jugador fijó para su tienda personal.");

        // Item definitions (Game configuration > Items).
        Add(e, "ItemDefinition.Name", "Nombre del ítem para el servidor y el panel.", "El nombre que ve el jugador en el juego sale de los archivos del cliente: cambiarlo acá no lo modifica en pantalla.");
        Add(e, "ItemDefinition.Group", "Categoría del ítem (0 a 15): 0 espadas, 1 hachas, 2 mazas, 3 lanzas, 4 arcos y ballestas, 5 báculos, 6 escudos, 7 cascos, 8 armaduras, 9 pantalones, 10 guantes, 11 botas, 12 alas y orbes, 13 mascotas, anillos y collares, 14 consumibles y joyas, 15 pergaminos.", "Junto con Number identifica al ítem. El cliente usa el mismo par, así que no conviene cambiarlo en ítems existentes.");
        Add(e, "ItemDefinition.Number", "Número del ítem dentro de su grupo. Junto con Group lo identifica de forma única.", "Si se cambia, servidor y cliente dejan de coincidir y el jugador vería otro ítem.");
        Add(e, "ItemDefinition.Durability", "Durabilidad máxima del ítem a nivel +0. En los ítems apilables (pociones, joyas, flechas) es el tamaño máximo de la pila.", "En ítems apilables define cuántas unidades caben en una casilla en todo el juego: inventario, baúl, comercio y tiendas. Subirla no cambia lo que los jugadores ya tienen. En armas y armaduras define cuánto duran antes de romperse. Conviene probar en el cliente después de cambiarla.");
        Add(e, "ItemDefinition.Value", "Valor base del ítem en Zen.", "Influye en el precio de compra y de venta a los NPC. En pilas, el precio crece con la cantidad.");
        Add(e, "ItemDefinition.DropLevel", "Nivel de drop: nivel mínimo del monstruo que puede soltar este ítem.", "Bajarlo hace que aparezca en monstruos más débiles; subirlo, que solo caiga de los más fuertes.");
        Add(e, "ItemDefinition.MaximumDropLevel", "Nivel máximo de monstruo que puede soltar el ítem. Vacío significa sin límite.", "Sirve para que un ítem deje de caer en monstruos muy fuertes.");
        Add(e, "ItemDefinition.DropsFromMonsters", "Indica si los monstruos pueden soltar el ítem al morir.", "Desactivarlo lo saca de los drops comunes. Los grupos de drop especiales pueden seguir incluyéndolo.");
        Add(e, "ItemDefinition.Width", "Cantidad de casillas de ancho que ocupa en el inventario.", "Debe coincidir con el cliente; si no, el ítem se superpone o se ve mal.");
        Add(e, "ItemDefinition.Height", "Cantidad de casillas de alto que ocupa en el inventario.", "Debe coincidir con el cliente; si no, el ítem se superpone o se ve mal.");
        Add(e, "ItemDefinition.ItemSlot", "Ranura del personaje donde se equipa (mano derecha, casco, alas...).", "Si no coincide con el cliente, el ítem no se puede equipar.");
        Add(e, "ItemDefinition.Skill", "Habilidad que da el ítem al equiparlo, o que se aprende al consumirlo (pergaminos y orbes).");
        Add(e, "ItemDefinition.IsAmmunition", "Marca al ítem como munición (flechas y virotes), que se consume del arma equipada.");
        Add(e, "ItemDefinition.IsBoundToCharacter", "El ítem queda atado al personaje: no se puede comerciar, guardar en el baúl, poner en la tienda personal ni ser recogido por otros.", "Útil para premios o ítems de evento que no deben circular entre jugadores.");
        Add(e, "ItemDefinition.IsQuestItem", "Ítem de misión: solo lo puede recoger del suelo quien tenga una misión activa que lo requiera.");
        Add(e, "ItemDefinition.MaximumItemLevel", "Nivel máximo de mejora del ítem (normalmente 15). Cero significa que no se puede mejorar.");
        Add(e, "ItemDefinition.MaximumSockets", "Cantidad máxima de sockets que puede tener una unidad de este ítem.");
        Add(e, "ItemDefinition.PossibleItemOptions", "Opciones que este ítem puede tener (excellent, luck, skill, harmony...).", "Define qué opciones pueden salir en los drops y aplicarse con combinaciones.");
        Add(e, "ItemDefinition.PossibleItemSetGroups", "Sets Ancient a los que puede pertenecer este ítem.");
        Add(e, "ItemDefinition.QualifiedCharacters", "Clases de personaje que pueden equipar el ítem.", "Si se quita una clase, esa clase ya no podrá usarlo.");
        Add(e, "ItemDefinition.Requirements", "Requisitos para equiparlo: nivel, fuerza, agilidad, vitalidad, energía, etc.", "Bajarlos permite equiparlo antes; subirlos lo vuelve más exclusivo.");
        Add(e, "ItemDefinition.BasePowerUpAttributes", "Atributos base del ítem, por ejemplo el daño mínimo y máximo de un arma o la defensa de una armadura.", "Cambia directamente el poder del ítem para todos los jugadores.");
        Add(e, "ItemDefinition.ConsumeEffect", "Efecto que se aplica al consumir el ítem (por ejemplo curar vida o dar un buff).");
        Add(e, "ItemDefinition.DropItems", "Grupos de drop (uno por nivel de ítem) que se usan cuando un jugador suelta este ítem.");
        Add(e, "ItemDefinition.PetExperienceFormula", "Fórmula de la experiencia necesaria por nivel de mascota. Solo aplica a mascotas entrenables.");
        Add(e, "ItemDefinition.StorageLimitPerCharacter", "Máximo de unidades de este ítem que puede llevar un personaje. Cero significa sin límite.", "Sirve para ítems únicos, como llaves o entradas de evento.");
    }
}
