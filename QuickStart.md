# Quick Start OpenMU / Guía Rápida OpenMU

*Read this QuickStart in [English](#english) or [Español](#espanol).*

<a id="english"></a>
## English

General requirements:

* Free TCP ports:
  * 80 (admin panel)
  * 55901 - 55906 (game servers)
  * 44405 - 44406 (connect servers)
    * 44405: default connection port for the original client
    * 44406: connection port especially for the [open source client](https://github.com/sven-n/MuMain)
  * 55980 (chat server)

* A game client (check [our Discord](https://discord.gg/2u5Agkd) FAQs)
* Knowledge or way to start the game client, so that it connects to the server. Our Launcher will do that.

  * Launcher binaries: [MUnique.OpenMU.ClientLauncher v0.9.6.zip](https://github.com/MUnique/OpenMU/releases/download/v0.9.0/MUnique.OpenMU.ClientLauncher_0.9.6.zip)
    * It requires the [.NET 10 runtime](https://dotnet.microsoft.com/download/dotnet/10.0) or higher
  * If your server and client runs on your local host, use any IP of 127.x.x.x, except 127.0.0.1, because this one is blocked by the client. For example, you could use 127.127.127.127

This guide describes two ways of starting the server. Use Docker, if you just
want to play around. If you want to develop or debug the server, choose the
manual way.

As you can see on the connect server ports, the server is initialized for two different clients by default.
They can connect to the same game servers through different ports. However, if you connect to the wrong port,
it may currently still work all correctly, you'll just get warnings in the logs. However, as soon as
we change encryption keys or methods, this will change.

## Docker

Please take a look at the deploy-folder of this project. There you'll find a more
detailed guide about how to set up this project.

### Environment Variables

It's possible to define additional environment variables to influence the
postgres database connection strings.

| Name | Description         |
|------|---------------------|
| DB_HOST | The hostname of the database. If the local configuration file is still configured to use 'localhost', the value of this variable replaces it |
| DB_ADMIN_USER | The user name of the postgres admin account. If the local configuration file is still configured to use 'postgres' for the user name of the admin (first entry in the ConnectionSettings.xml), the value of this variable replaces it. |
| DB_ADMIN_PW | The user name of the postgres admin account. If the local configuration file is still configured to use 'admin' for the user password of the admin (first entry in the ConnectionSettings.xml), the value of this variable replaces it. |

## Manually

Use this way, if you want to develop or debug for OpenMU.

Requirements:

* Windows OS, 10 or higher

  * It runs under Linux and MacOS, too. However, this guide describes it for
    windows.

  * PostgreSQL installed

  * Visual Studio 2026 installed, with workloads for ASP.NET Web development
    and .NET Desktop development. Please keep it up-to-date to prevent any issues.
  
  * Visual Stuido Extension "Web Compiler 2022+", if you plan to edit SCSS files
    for the admin panel.
    * https://marketplace.visualstudio.com/items?itemName=Failwyn.WebCompiler64

  * [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
    (it should be included in Visual Studio 2026)
    * `winget install Microsoft.DotNet.SDK.10`

  * [NodeJS 16+](https://nodejs.org) installed
    * `winget install OpenJS.NodeJS.LTS`

  * This repository cloned

If you have that, you'll need to do:

* Open the solution of OpenMU with Visual Studio

* Right click the solution and 'Restore NuGet Packages'

* Edit OpenMU\Persistence\EntityFramework\ConnectionSettings.xml, so that the
  connection strings are correct - however only the user/password of the first
  and second connection string need to be correct. The server will try to create
  the other roles specified by the settings.

* Build the solution

* Start MUnique.OpenMU.Startup

  * If required, it will create the database schemas, the required roles and
    gives permissions to this roles.

  * Optional: You can reinitialize the database by adding a ```-reinit``` parameter.

* When the Admin Panel is initialized, go to <http://localhost/>. Then you
  should see three gameservers, the chat server and two connect servers. Start
  the connect servers and at least one gameserver.

* If you update to a newer state of the master-branch, it could be possible
    that you have to update the database and configuration.
    You can find updates in the admin panel.

* Then you can connect to the server through the game client.

## Helpful (optional) steps

* __Auto Start__: If you don't want to start each server listener after starting
  the process, you can either activate "Auto Start"

  * in the admin panel at ```Configuration -> System```,

  * or with the start parameter ```-autostart```.

* __IP Resolving__: If you encounter disconnects after selecting a server, it's most
  likely a wrong setting for the IP resolver. You can change it easily over the
  admin panel at ```Configuration -> System``` as well.
  You may also change the setting by providing start parameters or environment
  parameters, however I just recommend this for experienced users. Look at this
  [Readme](src/Startup/Readme.md) for more information.

* __Changing the game version__: If you want to player another version than
  season 6, you may initialize the database with another game version.
  You can do this over the admin panel as well, at the ```Setup``` page.

## Test Accounts

To test some features of the server, test accounts are created automatically
when the database is initialized.

These are the user names:

* test0 - test9: General test accounts, level 1 to 90, in 10 level steps

Season 6 only:
* test300: General test account with level 300
* test400: General test account with level 400, master characters
* testgm: Test account of a game master
* testgm2: Test account of a game master with summoner and rage fighter characters
* testunlock: Test account without characters, but unlocked character classes
* quest1: Test account for the level 150 quests
* quest2: Test account for the level 220 quests
* quest3: Test account for the level 400 quests
* ancient: Test account with ancient item sets, level 330 characters
* socket: Test account with socket item sets, level 380 characters

The __passwords__ of these accounts are the __same as the user name__.

<a id="espanol"></a>
## Español

Requisitos generales:

* Puertos TCP libres:
  * 80 (admin panel)
  * 55901 - 55906 (game servers)
  * 44405 - 44406 (connect servers)
    * 44405: default connection port for the original client
    * 44406: connection port especially for the [open source client](https://github.com/sven-n/MuMain)
  * 55980 (chat server)

* Un game client (revisa las FAQs de [nuestro Discord](https://discord.gg/2u5Agkd))
* Conocimiento o forma de iniciar el game client para que se conecte al server. Nuestro Launcher lo hará.

  * Binarios del Launcher: [MUnique.OpenMU.ClientLauncher v0.9.6.zip](https://github.com/MUnique/OpenMU/releases/download/v0.9.0/MUnique.OpenMU.ClientLauncher_0.9.6.zip)
    * Requiere el [.NET 9 runtime](https://dotnet.microsoft.com/download/dotnet/9.0)
  * Si tu server y client corren en tu host local, usa cualquier IP de 127.x.x.x, excepto 127.0.0.1, porque el client la bloquea. Por ejemplo, podrías usar 127.127.127.127

Esta guía describe dos maneras de iniciar el server. Usa Docker si solo quieres probar. Si quieres desarrollar o depurar el server, elige la forma manual.

Como puedes ver en los puertos del connect server, el server se inicializa para dos clients diferentes por defecto.
Pueden conectarse a los mismos game servers a través de distintos puertos. Sin embargo, si te conectas al puerto incorrecto,
quizás actualmente todavía funcione correctamente y solo obtendrás advertencias en los logs. Sin embargo, tan pronto como
cambiemos las claves o métodos de encriptación, esto cambiará.

## Docker

Por favor, revisa la carpeta deploy de este proyecto. Allí encontrarás una guía más detallada sobre cómo configurar este proyecto.

### Variables de entorno

Es posible definir variables de entorno adicionales para influir en las cadenas de conexión de la base de datos postgres.

| Nombre | Descripción |
|-------|-------------|
| DB_HOST | El hostname de la base de datos. Si el archivo de configuración local aún está configurado para usar 'localhost', el valor de esta variable lo reemplaza |
| DB_ADMIN_USER | El nombre de usuario de la cuenta admin de postgres. Si el archivo de configuración local aún está configurado para usar 'postgres' como nombre de usuario del admin (primer entrada en ConnectionSettings.xml), el valor de esta variable lo reemplaza. |
| DB_ADMIN_PW | La contraseña de la cuenta admin de postgres. Si el archivo de configuración local aún está configurado para usar 'admin' para la contraseña del admin (primer entrada en ConnectionSettings.xml), el valor de esta variable lo reemplaza. |

## Manualmente

Usa este método si quieres desarrollar o depurar OpenMU.

Requisitos:

* Windows OS, 10 o superior

  * También se ejecuta en Linux y MacOS. Sin embargo, esta guía lo describe para Windows.

  * PostgreSQL instalado

  * Visual Studio 2022 (17.12+) instalado, con workloads para ASP.NET Web development y .NET Desktop development. Manténlo actualizado para evitar problemas.

  * Extensión de Visual Studio "Web Compiler 2022+" si planeas editar archivos SCSS para el admin panel.
    * https://marketplace.visualstudio.com/items?itemName=Failwyn.WebCompiler64

  * [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0) (debería estar incluido en Visual Studio 17.12+)
    * `winget install Microsoft.DotNet.SDK.9`

  * [NodeJS 16+](https://nodejs.org) instalado
    * `winget install OpenJS.NodeJS.LTS`

  * Este repositorio clonado

Si tienes eso, tendrás que:

* Abrir la solución de OpenMU con Visual Studio

* Click derecho en la solución y 'Restore NuGet Packages'

* Editar OpenMU\Persistence\EntityFramework\ConnectionSettings.xml, para que las cadenas de conexión sean correctas; sin embargo, solo el usuario/contraseña de la primera y segunda cadena necesitan ser correctos. El server intentará crear los otros roles especificados por la configuración.

* Build la solución

* Iniciar MUnique.OpenMU.Startup

  * Si es necesario, creará los esquemas de base de datos, los roles requeridos y dará permisos a estos roles.

  * Opcional: puedes reinicializar la base de datos agregando un parámetro ```-reinit```.

* Cuando el Admin Panel esté inicializado, ve a <http://localhost/>. Allí deberías ver tres game servers, el chat server y dos connect servers. Inicia los connect servers y al menos un game server.

* Si actualizas a un estado más reciente de la rama master, podría ser necesario actualizar la base de datos y la configuración. Puedes encontrar actualizaciones en el admin panel.

* Luego puedes conectarte al server a través del game client.

## Pasos útiles (opcionales)

* __Auto Start__: Si no quieres iniciar cada server listener después de iniciar el proceso, puedes activar "Auto Start"

  * en el admin panel en ```Configuration -> System```,

  * o con el parámetro de inicio ```-autostart```.

* __Resolución de IP__: Si encuentras desconexiones después de seleccionar un server, lo más probable es una configuración incorrecta para el IP resolver. Puedes cambiarlo fácilmente desde el admin panel en ```Configuration -> System```. También puedes cambiar el ajuste proporcionando start parameters o environment parameters; sin embargo, solo lo recomiendo para usuarios experimentados. Mira este [Readme](src/Startup/Readme.md) para más información.

* __Cambiar la versión del juego__: Si quieres jugar otra versión distinta de la season 6, puedes inicializar la base de datos con otra versión del juego. Puedes hacerlo también desde el admin panel en la página ```Setup```.

## Cuentas de prueba

Para probar algunas features del server, se crean cuentas de prueba automáticamente cuando se inicializa la base de datos.

Estos son los user names:

* test0 - test9: General test accounts, level 1 a 90, en pasos de 10 niveles

Solo Season 6:
* test300: General test account con level 300
* test400: General test account con level 400, master characters
* testgm: Test account de un game master
* testgm2: Test account de un game master con personajes summoner y rage fighter
* testunlock: Test account sin characters, pero classes desbloqueadas
* quest1: Test account para las level 150 quests
* quest2: Test account para las level 220 quests
* quest3: Test account para las level 400 quests
* ancient: Test account con ancient item sets, level 330 characters
* socket: Test account con socket item sets, level 380 characters

Los __passwords__ de estas cuentas son __los mismos que el user name__.
