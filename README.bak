# OpenMU Project / Proyecto OpenMU

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/d0f57e29e7524dadb677561389256d8b)](https://www.codacy.com/gh/MUnique/OpenMU/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=MUnique/OpenMU&amp;utm_campaign=Badge_Grade)
[![Gitter chat](https://badges.gitter.im/OpenMU-Project/gitter.svg)](https://gitter.im/OpenMU-Project/Lobby)
[![Discord chat](https://img.shields.io/discord/669595902750490698?logo=discord)](https://discord.gg/2u5Agkd)

*Read this README in [English](#english) or [Español](#espanol).*

<a id="english"></a>
## English

| Platform       |Build Status          |
|----------------|----------------------|
| Windows        | ![Windows Build Status](https://dev.azure.com/MUnique/OpenMU/_apis/build/status/MUnique.OpenMU?branchName=master) |
| Linux (Docker) | [![Docker Build Status](https://dev.azure.com/MUnique/OpenMU/_apis/build/status/MUnique.OpenMU%20Docker?branchName=master)](https://hub.docker.com/r/munique/openmu)  |

| NuGet Packages |   |
|----------------|---|
| MUnique.OpenMU.Network | [![NuGet Badge](https://img.shields.io/nuget/v/MUnique.OpenMU.Network)](https://www.nuget.org/packages/MUnique.OpenMU.Network/) |
| MUnique.OpenMU.Network.Packets | [![NuGet Badge](https://img.shields.io/nuget/v/MUnique.OpenMU.Network.Packets)](https://www.nuget.org/packages/MUnique.OpenMU.Network.Packets/) |

This project aims to create an easy to use, extendable and customizable server
for a MMORPG called "MU Online".
The server supports multiple versions of the game, but the main focus is
version of Season 6 Episode 3 using the ENG (english) protocol. Additionally,
the long-term focus is on the [open source client](https://github.com/sven-n/MuMain)
which supports a slightly extended network protocol.
However, parts of the software can also be suitable for the development of
other games, even for other kind of games.

The code is a complete rewrite from scratch - it's not based on pre-existing
projects, and it's also explicitly not based on decompiled server sources or
their countless derivates.

There also exists a [blog](https://munique.net) which may contain some valuable
information about this development.

## Fork changelog

This fork diverges from the original OpenMU project and introduces:

- Bilingual documentation in English and Spanish.
- LAN presets for Season 6 with deployment overlays for LAN, DNS and npm.
- `proxynet` integration replacing nginx and fixing port configuration.
- Additional Spanish translations and admin panel language fixes.
- New crafting recipes plus fixes for craftings, skills and grid issues.
- Updated White Wizard event data and related fixes.

### Elf Summon Plug-in

This fork includes a configurable plug-in to change Elf summons (skills 30..36) and scale their stats by Energy without restarting the server.

Code location: `src/GameLogic/PlugIns/ElfSummonsAll.cs`.

What it provides
- Replace the summoned monster per skill (30..36) or keep the default mapping.
- Dynamic scaling by Energy applied to the base stats of the chosen monster (HP, base damage Phys/Wiz/Curse, DefenseBase):
  `scale = 1 + floor(TotalEnergy / EnergyPerStep) * PercentPerStep`.
- Buff/regeneration skills also include your own summon (and party members� summons) when the target mode is self/party.
- Apply configuration changes at runtime; just unsummon and summon again.

How to enable
- In the Admin Panel: Plugins ? filter by "Summon configuration".
- You will see 7 entries: "Elf Summon cfg � (30..36)". Activate the ones you need.
- Edit the "Custom Configuration" of each. Available fields:
  - `MonsterNumber` (int): 0 = use the server default mapping; >0 = monster number to summon.
  - `EnergyPerStep` (int): 0 to disable; otherwise size of each Energy step (e.g. 1000).
  - `PercentPerStep` (float): added per step (e.g. 0.05 = +5%).

Important notes (for using this plug-in in another repo)
- Monster stat cache adjustment (required so scaling applies to summons):
  - In `src/GameLogic/Attributes/MonsterAttributeHolder.cs`, don�t cache by `MonsterDefinition` (equals by Id). Summoned clones share Id; read attributes per-instance instead. Included in this fork.
- Prevent damage to your own summon with area skills (recommended):
  - In `src/GameLogic/PlayerActions/Skills/AreaSkillAttackAction.cs` and `src/GameLogic/PlayerActions/Skills/AreaSkillHitAction.cs`, exclude `Monster { SummonedBy == player }` from targets. Included in this fork.
- Configuration hot-reload: On each summon creation, the plug-in fetches the latest CustomConfiguration from the database (no cache). No restart required; just re-summon.
- Pet HUD (Fenrir/Raven bar): Elf summons don�t use the item-pet system, so the stock client doesn�t show that bar. Name/owner display is supported. Pet HUD would require client changes.

Examples
- +5% per 1000 Energy using default monster: `{"MonsterNumber": 0, "EnergyPerStep": 1000, "PercentPerStep": 0.05}`.## Current project state

This project is currently under development without any release.
You can try the current state by using the available docker image, also
mentioned in the [quick start guide](QuickStart.md).

## Licensing

This project is released under the MIT license (see LICENSE file).

## Used technologies

The project is mainly written in C# and targets .NET 9.0.

The servers admin panel is hosted on an embedded ASP.NET Core webserver (Kestrel)
and implemented as Blazor Server App.

At the moment the persistence layer uses the [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore)
and [PostgreSQL](https://www.postgresql.org) as database. Additionally, it's
also possible to start it in a non-persistent in-memory mode.

The project supports distributed hosting based on Dapr. Alternatively, it can be
hosted in one process as well.

## Deployment

We provide Docker images and docker-compose files for easy deployment.
Please take a look at the deploy-folder of this project.

## Contributions

Contributions are welcome if they meet the following criteria:

* Language is english.

* Code should be StyleCop compliant - this project uses the [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)
  for VS2022 so you should see issues directly as warnings.

* Coding style (naming, etc.) and quality should fit to the current state.

* No code copied/converted from the well-known decompiled source of the
    original server.

If you want to contribute, please create a new issue for the feature or bug (if
the issue doesn't exist yet) so we can see who is working on something and can
discuss possible solutions. If it's a small thing, you can also just send a
pull request without adding an issue.

Apart of that, contributions from non-developers are welcome as well. You can
test the server, submit issues or suggestions, packet descriptions or
documentations about the concepts and mechanics of the game itself. Please use
markdown files/syntax for this purpose.

If you have questions about that, don't hesitate to ask in our [discord channel](https://discord.gg/2u5Agkd)
or by submitting an issue.

## How to contribute code

If you want to contribute code, please do the following steps:

1. fork this project from the original MUnique OpenMU Project.
2. create a feature branch from the master branch
3. commit your changes to your feature branch
4. submit a pull request to the original master branch
5. lean back, wait for the code review and merge :)

## How to use

Please have a look at the [quick start guide](QuickStart.md).

## Gameplay differences to the original server

This project doesn't have the goal to copy the original MU Online server
behavior to 100 %. This is not entirely possible, because the original server
is written in another programming language and has a completely different
architecture.
With some points we make our life easier in this project, with other points we
try to improve the gameplay.

### Calculations

The calculations of attribute values (like character damage decrement etc.) are
done with 32 bit float numbers and without rounding off, like the original
server does at some places.
E.g. distributed stat points always have effect, while in the original server
effects might get rounded down. For example, when 4 points of strength gives 1
base damage, the original server doesn't calculate a fraction of 1 damage for
3 points, while OpenMU calculates 0.75 damage. This damage
has then an effect in further calculations.

### Countdown when changing character or sub-server

The original server uses a five second countdown when a player wants to change
his character or the sub-server. Maybe this was done for some performance
reasons, as the original server would then save the character/account data.
We think that's really annoying and see no real value in that, so we don't use
a countdown.

<a id="espanol"></a>
## Español

| Plataforma       | Estado de compilación          |
|-----------------|-------------------------------|
| Windows        | ![Windows Build Status](https://dev.azure.com/MUnique/OpenMU/_apis/build/status/MUnique.OpenMU?branchName=master) |
| Linux (Docker) | [![Docker Build Status](https://dev.azure.com/MUnique/OpenMU/_apis/build/status/MUnique.OpenMU%20Docker?branchName=master)](https://hub.docker.com/r/munique/openmu)  |

| Paquetes NuGet |   |
|----------------|---|
| MUnique.OpenMU.Network | [![NuGet Badge](https://img.shields.io/nuget/v/MUnique.OpenMU.Network)](https://www.nuget.org/packages/MUnique.OpenMU.Network/) |
| MUnique.OpenMU.Network.Packets | [![NuGet Badge](https://img.shields.io/nuget/v/MUnique.OpenMU.Network.Packets)](https://www.nuget.org/packages/MUnique.OpenMU.Network.Packets/) |

Este proyecto tiene como objetivo crear un servidor fácil de usar, ampliable y personalizable para un MMORPG llamado "MU Online".
El servidor admite múltiples versiones del juego, pero el enfoque principal es la versión de la Season 6 Episode 3 utilizando el protocolo ENG (inglés). Además, el enfoque a largo plazo está en el [cliente de código abierto](https://github.com/sven-n/MuMain), que soporta un protocolo de red ligeramente ampliado.
Sin embargo, partes del software también pueden ser adecuadas para el desarrollo de otros juegos, incluso de otro tipo.

El código es una reescritura completa desde cero; no se basa en proyectos preexistentes ni en fuentes de servidor descompiladas ni en sus innumerables derivados.

También existe un [blog](https://munique.net) que puede contener información valiosa sobre este desarrollo.

## Cambios del fork

Este fork se desvía del proyecto original OpenMU e introduce:

- Documentación bilingüe en inglés y español.
- Presets LAN para Season 6 con overlays de despliegue para LAN, DNS y npm.
- Integración de `proxynet` reemplazando nginx y corrigiendo la configuración de puertos.
- Traducciones adicionales al español y correcciones de idioma del panel de administración.
- Nuevas recetas de crafteo y correcciones para crafteo, habilidades y problemas de cuadrícula.
- Datos actualizados del evento White Wizard y correcciones relacionadas.

### Plugin de invocaciones de Elfa

Este fork incluye un plugin configurable para cambiar las invocaciones de la Elfa (skills 30..36) y escalar sus stats en base a la Energ�a, sin reiniciar el servidor.

Ubicacion del codigo: `src/GameLogic/PlugIns/ElfSummonsAll.cs`.

Que permite
- Reemplazar el monstruo invocado por cada skill (30..36) o mantener el mapeo por defecto.
- Escalado por Energia aplicado a los stats base del monstruo elegido (HP, da�o base Fis/Wiz/Curse, DefenseBase):
  `scale = 1 + floor(TotalEnergy / EnergyPerStep) * PercentPerStep`.
- Los skills de Buff/Regeneration incluyen al summon propio (y los del party) cuando el target es self/party.
- Cambios de configuracion en caliente; basta con desinvocar y volver a invocar.

Como habilitarlo
- En el Panel de Administracion: Plugins -> filtrar por "Summon configuration".
- Vas a ver 7 entradas: "Elf Summon cfg ... (30..36)". Activa las que quieras usar.
- Edita la "Custom Configuration" de cada una. Campos disponibles:
  - `MonsterNumber` (int): 0 = usa el mapeo por defecto del servidor; >0 = numero de monstruo a invocar.
  - `EnergyPerStep` (int): 0 para desactivar; si no, tama�o de cada paso de Energia (p.ej. 1000).
  - `PercentPerStep` (float): incremento por paso (p.ej. 0.05 = +5%).

Notas importantes (si queres usar solo el plugin en otro repo)
- Ajuste de cache de stats de monstruos (requerido para que el escalado aplique):
  - En `src/GameLogic/Attributes/MonsterAttributeHolder.cs`, evita cachear por `MonsterDefinition` (igual por Id). Los clones del summon comparten Id; leer por instancia. Incluido en este fork.
- Evitar da�o al propio summon con skills en area (recomendado):
  - En `src/GameLogic/PlayerActions/Skills/AreaSkillAttackAction.cs` y `src/GameLogic/PlayerActions/Skills/AreaSkillHitAction.cs`, excluir `Monster { SummonedBy == player }` de los targets. Incluido en este fork.
- Hot-reload: En cada creacion del summon, el plugin lee la CustomConfiguration mas reciente desde la base de datos (sin cache). No hace falta reiniciar; desinvoca y volve a invocar.
- HUD de "pet": Las invocaciones de elfa no usan el sistema de mascotas por item, por lo que el cliente no muestra esa barra.

Ejemplos de uso
- +5% por cada 1000 de Energia usando el mob por defecto: `{"MonsterNumber": 0, "EnergyPerStep": 1000, "PercentPerStep": 0.05}`.## Estado actual del proyecto

Este proyecto se encuentra actualmente en desarrollo sin ningún lanzamiento.
Puedes probar el estado actual utilizando la imagen de docker disponible, mencionada también en la [guía rápida](QuickStart.md).

## Licencia

Este proyecto se publica bajo la licencia MIT (ver archivo LICENSE).

## Tecnologías utilizadas

El proyecto está escrito principalmente en C# y apunta a .NET 9.0.

El panel de administración del servidor se aloja en un servidor web ASP.NET Core embebido (Kestrel) y se implementa como una aplicación Blazor Server.

En este momento la capa de persistencia utiliza [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) y [PostgreSQL](https://www.postgresql.org) como base de datos. Además, es posible iniciarlo en un modo no persistente en memoria.

El proyecto soporta alojamiento distribuido basado en Dapr. Alternativamente, también puede alojarse en un solo proceso.

## Despliegue

Proporcionamos imágenes de Docker y archivos docker-compose para un despliegue sencillo.
Por favor, echa un vistazo a la carpeta deploy de este proyecto.

## Contribuciones

Las contribuciones son bienvenidas si cumplen los siguientes criterios:

* El idioma es inglés.
* El código debe cumplir con StyleCop; este proyecto usa [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/) para VS2022, por lo que deberías ver los problemas directamente como advertencias.
* El estilo de codificación (nombres, etc.) y la calidad deben ajustarse al estado actual.
* No debe incluir código copiado/convertido de la conocida fuente descompilada del servidor original.

Si deseas contribuir, crea un nuevo issue para la característica o el error (si el issue aún no existe) para que podamos ver quién está trabajando en algo y discutir posibles soluciones.
Si es algo pequeño, también puedes enviar un pull request sin añadir un issue.

Además, las contribuciones de personas que no son desarrolladoras también son bienvenidas.
Puedes probar el servidor, enviar issues o sugerencias, descripciones de paquetes o documentaciones sobre los conceptos y mecánicas del juego.
Por favor, utiliza archivos/sintaxis markdown para este propósito.

Si tienes preguntas al respecto, no dudes en preguntar en nuestro [canal de Discord](https://discord.gg/2u5Agkd) o creando un issue.

## Cómo contribuir con código

Si deseas contribuir con código, sigue los siguientes pasos:

1. Haz un fork de este proyecto desde el proyecto original MUnique OpenMU.
2. Crea una rama de características a partir de la rama master.
3. Haz commit de tus cambios en tu rama.
4. Envía un pull request a la rama master original.
5. Relájate, espera la revisión de código y la fusión :)

## Cómo usarlo

Por favor, echa un vistazo a la [guía rápida](QuickStart.md).

## Diferencias de jugabilidad con el servidor original

Este proyecto no tiene como objetivo copiar al 100 % el comportamiento del servidor original de MU Online.
Esto no es completamente posible, porque el servidor original está escrito en otro lenguaje de programación y tiene una arquitectura completamente diferente.
En algunos aspectos nos facilitamos la vida en este proyecto y en otros tratamos de mejorar la jugabilidad.

### Cálculos

Los cálculos de los valores de atributos (como decremento del daño del personaje, etc.) se realizan con números de coma flotante de 32 bits y sin redondeo, a diferencia del servidor original en algunos lugares.
Por ejemplo, los puntos de estadísticas distribuidos siempre tienen efecto, mientras que en el servidor original los efectos pueden redondearse hacia abajo.
Si 4 puntos de fuerza otorgan 1 de daño base, el servidor original no calcula una fracción de daño para 3 puntos, mientras que OpenMU calcula 0.75 de daño.
Este daño tiene efecto en cálculos posteriores.

### Cuenta regresiva al cambiar de personaje o sub-servidor

El servidor original utiliza una cuenta regresiva de cinco segundos cuando un jugador quiere cambiar de personaje o de sub-servidor.
Quizás esto se hizo por razones de rendimiento, ya que el servidor original guardaba los datos del personaje/cuenta.
Creemos que eso es muy molesto y no vemos un valor real en ello, así que no usamos cuenta regresiva.

