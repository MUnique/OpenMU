﻿# OpenMU Project / Proyecto OpenMU

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/d0f57e29e7524dadb677561389256d8b)](https://www.codacy.com/gh/MUnique/OpenMU/dashboard?utm_source=github.com&utm_medium=referral&utm_content=MUnique/OpenMU&utm_campaign=Badge_Grade)
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
- proxynet integration replacing nginx and fixing port configuration.
- Additional Spanish translations and admin panel language fixes.
- New crafting recipes plus fixes for craftings, skills and grid issues.
- White Wizard monster + default invasion drop groups.
- Golden Archer essentials (Rena token, rewards, packed jewels) preconfigured.
- Rena global drop on Season 1 maps for 0.75 / 0.95d / Season 6.

### Recent updates (2025‑09)

- Docker images hardened and easier to debug:
  - `src/Startup/Dockerfile` now accepts `ASPNET_IMAGE`/`SDK_IMAGE` build args, installs ICU (`icu-libs`, `icu-data-full`) and sets `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false` to enable proper cultures on Alpine.
  - Uses `ARG APP_UID` (default `1000`) to avoid failures when no user id is provided.
  - Builds with `-v m` for detailed logs and copies `strings.*.json` to `/app/Localization` during publish.
- Localization refactor:
  - New robust `LocalizationService` (core) with safe culture fallback to Invariant when ICU is unavailable.
  - Single DI registration (singleton) shared by server and Admin Panel.
  - Admin components explicitly inject the core service to avoid type ambiguity.
  - Language selector changes the server language at runtime (affects in‑game messages which use localization APIs).
- Admin Panel reliability:
  - Fixed blank page caused by ambiguous DI type for `LocalizationService`.
  - Added simple log tail endpoint: `GET /api/logs/tail?take=200`.
- Build fixes:
  - Removed duplicate assembly attributes in `src/Localization` by setting `<GenerateAssemblyInfo>false</...>`.
  - Fixed Linux path casing and project reference for Admin Panel localization resources.
  - Minor C# fix in `LetterSendAction` to avoid variable shadowing under `-p:ci=true`.

### Elf Summon Plug-in

This fork includes a configurable plug-in to change Elf summons (skills 30..36) and scale their stats by Energy without restarting the server.

Code location: src/GameLogic/PlugIns/ElfSummonsAll.cs.

What it provides
- Replace the summoned monster per skill (30..36) or keep the default mapping.
- Dynamic scaling by Energy applied to the base stats of the chosen monster (HP, base damage Phys/Wiz/Curse, DefenseBase):
  scale = 1 + floor(TotalEnergy / EnergyPerStep) * PercentPerStep.
- Buff/regeneration skills also include your own summon (and party members' summons) when the target mode is self/party.
- Apply configuration changes at runtime; just unsummon and summon again.

How to enable
- In the Admin Panel: Plugins → filter by "Summon configuration".
- You will see 7 entries: "Elf Summon cfg ... (30..36)". Activate the ones you need.
- Edit the "Custom Configuration" of each. Available fields:
  - MonsterNumber (int): 0 = use the server default mapping; >0 = monster number to summon.
  - EnergyPerStep (int): 0 to disable; otherwise size of each Energy step (e.g. 1000).
  - PercentPerStep (float): added per step (e.g. 0.05 = +5%).

Important notes (for using this plug-in in another repo)
- Monster stat cache adjustment (required so scaling applies to summons):
  - In src/GameLogic/Attributes/MonsterAttributeHolder.cs, don't cache by MonsterDefinition (equals by Id). Summoned clones share Id; read attributes per-instance instead. Included in this fork.
- Prevent damage to your own summon with area skills (recommended):
  - In src/GameLogic/PlayerActions/Skills/AreaSkillAttackAction.cs and src/GameLogic/PlayerActions/Skills/AreaSkillHitAction.cs, exclude Monster { SummonedBy == player } from targets. Included in this fork.
- Configuration hot-reload: On each summon creation, the plug-in fetches the latest CustomConfiguration from the database (no cache). No restart required; just re-summon.
- Pet HUD (Fenrir/Raven bar): Elf summons don't use the item-pet system, so the stock client doesn't show that bar. Name/owner display is supported. Pet HUD would require client changes.

Examples
- +5% per 1000 Energy using default monster: {"MonsterNumber": 0, "EnergyPerStep": 1000, "PercentPerStep": 0.05}.

### Rena & Golden Archer

What’s included
- Rena token (group 14, number 21) added if missing.
- Drop groups: "Golden Archer Rewards" and "Golden Archer Packed Jewels" with common jewels.
- Golden Archer NPC plug-in preconfigured and enabled (uses Rena as token; Box of Luck/Heaven and reward groups).

Global Rena Drops on S1 maps
- Optional updates per version: "Add Rena Global Drop (0.75)", "(0.95d)", "(Season 6)".
- Adds drop group "Rena Global Drop (S1 maps)" to Lorencia, Noria, Devias, Dungeon, Lost Tower, Atlans, Arena, Exile.
- Default chance: 0.2% per kill (editable in Admin Panel by editing the drop group Chance).

How to apply/update
- Admin Panel → Updates: select
  - "Golden Archer: Rena + Reward Group"
  - "Add Rena Global Drop (…version…)"
  and click Apply. They are non-mandatory and safe to re-run.

Notes
- To use the Golden Archer, ensure the NPC exists in your game setup; the plug-in is already enabled and uses the configured drop groups.

### White Wizard & Invasions

What’s included
- Adds White Wizard monster (id 135) if missing, and default drop groups for support mobs and boss.
- Designed for Season 6 data; safe to apply over existing configs.

How to apply/update
- Admin Panel → Updates: select "White Wizard Monster and Invasion Drops" and Apply.

## Current project state

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

### Deploy from a remote fork (compose overlay)

- Overlays in `deploy/all-in-one` allow building the image directly from your fork using a remote git context.
- Required env var: `OPENMU_FORK_CONTEXT` in the form `https://github.com/<user>/OpenMU-<fork>.git#<branch>:src`.
- Example commands (server):
  - `export OPENMU_FORK_CONTEXT="https://github.com/EmanuelCatania/OpenMU-S2.git#master:src"`
  - `docker compose -f docker-compose.no-nginx.yml -f docker-compose.override.yml -f docker-compose.public-dns.yml -f docker-compose.npm-net.yml -f docker-compose.from-fork.yml build --no-cache --progress=plain --build-arg APP_UID=1000 openmu-startup`
  - `docker compose -f docker-compose.no-nginx.yml -f docker-compose.public-dns.yml -f docker-compose.npm-net.yml -f docker-compose.from-fork.yml up -d --no-deps --force-recreate openmu-startup`

Notes
- `src/Startup/Dockerfile` installs ICU (`icu-libs`, `icu-data-full`) and sets `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false` to enable real cultures on Alpine.
- If you are behind a proxy (NPM/Cloudflare), enable WebSockets for `/_blazor` and avoid orange cloud (DNS only) for the host.

## Localization

- Core service: `src/Localization/LocalizationService.cs` reads `strings.*.json` from a configurable `ResourceDirectory` (defaults to `/app/Localization` in Docker).
- Admin Panel components inject the core service and react to language changes (`LanguageChanged` event).
- Changing the language from the UI updates the singleton service used by the server as well; server messages which call `GetLocalizedMessage(...)` reflect the new language immediately.
- On restart the language falls back to the configuration in `src/Startup/appsettings.json` (`Localization:DefaultLanguage`, `Localization:CurrentLanguage`).
- When ICU is not available, cultures gracefully fall back to `InvariantCulture` so the server keeps running.

## Troubleshooting

- Build: `base name (${SDK_IMAGE}) should not be blank`
  - Declare `ARG SDK_IMAGE=...` before the first `FROM`. Already fixed in `src/Startup/Dockerfile`.
- Build: duplicate assembly attributes (CS0579) in Localization
  - Set `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` in `src/Localization/MUnique.OpenMU.Localization.csproj`. Already applied.
- Runtime: crash with `CultureNotFoundException` in globalization‑invariant mode
  - The Dockerfile installs ICU and sets `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false`. Alternatively, the service now falls back to `InvariantCulture`.
- Admin Panel: blank page and console error `Cannot provide a value for property 'Localization' ...`
  - Caused by ambiguous DI type; components explicitly inject the core service (`MUnique.OpenMU.Localization.LocalizationService`). Fixed in `src/Web/AdminPanel/Localization/LocalizedComponentBase.cs` and `src/Web/AdminPanel/Localization/LocalizedLayoutComponentBase.cs`.
- Admin Panel: 404 for `/_content/...` or SignalR disconnects
  - Ensure your proxy forwards static assets and enables WebSockets for `/_blazor`. Try direct access to the host/port to isolate proxy issues.
- Logs
  - Tail recent logs over HTTP: `GET /api/logs/tail?take=200`.
  - Or use `docker logs -n 200 openmu-startup`.

### Deploy from a remote fork (compose overlay)

- Overlays in `deploy/all-in-one` allow building the image directly from your fork using a remote git context.
- Required env var: `OPENMU_FORK_CONTEXT` in the form `https://github.com/<user>/OpenMU-<fork>.git#<branch>:src`.
- Example commands (server):
  - `export OPENMU_FORK_CONTEXT="https://github.com/EmanuelCatania/OpenMU-S2.git#master:src"`
  - `docker compose -f docker-compose.no-nginx.yml -f docker-compose.override.yml -f docker-compose.public-dns.yml -f docker-compose.npm-net.yml -f docker-compose.from-fork.yml build --no-cache --progress=plain --build-arg APP_UID=1000 openmu-startup`
  - `docker compose -f docker-compose.no-nginx.yml -f docker-compose.public-dns.yml -f docker-compose.npm-net.yml -f docker-compose.from-fork.yml up -d --no-deps --force-recreate openmu-startup`

Notes
- `src/Startup/Dockerfile` installs ICU (`icu-libs`, `icu-data-full`) and sets `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false` to enable real cultures on Alpine.
- If you are behind a proxy (NPM/Cloudflare), enable WebSockets for `/_blazor` and avoid orange cloud (DNS only) for the host.

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
- Integración de proxynet reemplazando nginx y corrigiendo la configuración de puertos.
- Traducciones adicionales al español y correcciones de idioma del panel de administración.
- Nuevas recetas de crafteo y correcciones para crafteo, habilidades y problemas de cuadrícula.
- Mago Blanco (White Wizard) + grupos de drop por defecto para la invasión.
- Golden Archer listo (token Rena, recompensas y joyas empaquetadas) preconfigurado.
- Drop global de Rena en mapas de Season 1 para 0.75 / 0.95d / Season 6.

### Plugin de invocaciones de Elfa

Este fork incluye un plugin configurable para cambiar las invocaciones de la Elfa (skills 30..36) y escalar sus stats en base a la Energía, sin reiniciar el servidor.

Ubicación del código: src/GameLogic/PlugIns/ElfSummonsAll.cs.

Qué permite
- Reemplazar el monstruo invocado por cada skill (30..36) o mantener el mapeo por defecto.
- Escalado por Energía aplicado a los stats base del monstruo elegido (HP, daño base Fis/Wiz/Curse, DefenseBase):
  scale = 1 + floor(TotalEnergy / EnergyPerStep) * PercentPerStep.
- Los skills de Buff/Regeneración incluyen al summon propio (y los del party) cuando el target es self/party.
- Cambios de configuración en caliente; basta con desinvocar y volver a invocar.

Cómo habilitarlo
- En el Panel de Administración: Plugins → filtrar por "Summon configuration".
- Verás 7 entradas: "Elf Summon cfg ... (30..36)". Activa las que quieras usar.
- Edita la "Custom Configuration" de cada una. Campos disponibles:
  - MonsterNumber (int): 0 = usa el mapeo por defecto del servidor; >0 = número de monstruo a invocar.
  - EnergyPerStep (int): 0 para desactivar; si no, tamaño de cada paso de Energía (p.ej. 1000).
  - PercentPerStep (float): incremento por paso (p.ej. 0.05 = +5%).

Notas importantes (si quieres usar solo el plugin en otro repo)
- Ajuste de caché de stats de monstruos (requerido para que el escalado aplique):
  - En src/GameLogic/Attributes/MonsterAttributeHolder.cs, evita cachear por MonsterDefinition (igual por Id). Los clones del summon comparten Id; lee por instancia. Incluido en este fork.
- Evitar daño al propio summon con skills en área (recomendado):
  - En src/GameLogic/PlayerActions/Skills/AreaSkillAttackAction.cs y src/GameLogic/PlayerActions/Skills/AreaSkillHitAction.cs, excluir Monster { SummonedBy == player } de los targets. Incluido en este fork.
- Hot-reload: En cada creación del summon, el plugin lee la CustomConfiguration más reciente desde la base de datos (sin caché). No hace falta reiniciar; desinvoca y vuelve a invocar.
- HUD de "pet": Las invocaciones de elfa no usan el sistema de mascotas por ítem, por lo que el cliente no muestra esa barra.

Ejemplos de uso
- +5% por cada 1000 de Energía usando el mob por defecto: {"MonsterNumber": 0, "EnergyPerStep": 1000, "PercentPerStep": 0.05}.

### Rena y Golden Archer

Qué incluye
- Token Rena (grupo 14, número 21) si falta.
- Grupos de drop: "Golden Archer Rewards" y "Golden Archer Packed Jewels" con joyas comunes.
- Plug-in del NPC Golden Archer preconfigurado y habilitado (usa Rena como ficha; Box of Luck/Heaven y grupos de recompensa).

Drop global de Rena en mapas S1
- Updates opcionales por versión: "Add Rena Global Drop (0.75)", "(0.95d)", "(Season 6)".
- Agrega el grupo "Rena Global Drop (S1 maps)" a Lorencia, Noria, Devias, Dungeon, Lost Tower, Atlans, Arena, Exile.
- Chance por defecto: 0.2% por mob (editable en el Admin Panel editando el Chance del grupo).

Cómo aplicarlo/actualizar
- Admin Panel → Updates: seleccionar
  - "Golden Archer: Rena + Reward Group"
  - "Add Rena Global Drop (…versión…)"
  y aplicar. No son obligatorios y son seguros de re-ejecutar.

Notas
- Para usar el Golden Archer, asegurate de tener el NPC en el mapa/escenario; el plug-in ya está habilitado y usa los grupos configurados.

### White Wizard e Invasiones

Qué incluye
- Agrega el monstruo White Wizard (id 135) si falta y grupos de drops por defecto para mobs de soporte y el boss.
- Diseñado para datos de Season 6; es seguro aplicarlo sobre configuraciones existentes.

Cómo aplicarlo/actualizar
- Admin Panel → Updates: seleccionar "White Wizard Monster and Invasion Drops" y aplicar.

## Estado actual del proyecto

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
