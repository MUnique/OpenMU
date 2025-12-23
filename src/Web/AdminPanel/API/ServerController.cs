namespace MUnique.OpenMU.Web.API
{
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Localization;
    using System.Text.Json;
    using System;
    using System.Linq;

    /// <summary>
    /// Server API controller
    /// </summary>
    [Route("api/")]
    public class ServerController : Controller
    {
        private readonly IDictionary<int, IGameServer> _gameServers;
        private readonly LocalizationService _localization;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameServers"></param>
        /// <param name="localization">The localization service.</param>
        public ServerController(IDictionary<int, IGameServer> gameServers, LocalizationService localization)
        {
            this._gameServers = gameServers;
            this._localization = localization;
        }

        /// <summary>
        /// Gets the current server localization settings.
        /// </summary>
        /// <returns>The current language and available languages.</returns>
        [HttpGet]
        [Route("localization")]
        public IActionResult GetLocalization()
        {
            var payload = new
            {
                current = this._localization.CurrentLanguage,
                available = this._localization.AvailableLanguages,
            };

            return this.Ok(payload);
        }

        /// <summary>
        /// Changes the language used by the game servers.
        /// </summary>
        /// <param name="language">The target language code.</param>
        /// <returns>The updated localization state.</returns>
        [HttpPost]
        [Route("localization/{language}")]
        public IActionResult SetLocalization(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return this.BadRequest("Language must be provided.");
            }

            if (!this._localization.AvailableLanguages.Contains(language, StringComparer.OrdinalIgnoreCase))
            {
                return this.BadRequest($"Language '{language}' is not supported.");
            }

            this._localization.SetLanguage(language);
            return this.Ok(new { current = this._localization.CurrentLanguage });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [Route("send/{id=0}")]
        public async Task<IActionResult> SendGlobalMessage(int id, [FromQuery(Name = "msg")] string msg)
        {
            var server = (GameServer)_gameServers.Values.ElementAt(id);
            if (server is not null)
            {
                await server.Context.SendGlobalNotificationAsync(msg).ConfigureAwait(false);
                return Ok("Done");
            }
            return Ok("Server not ready");
        }

        /// <summary>
        /// Gets a flag, if the specified account is currently online.
        /// </summary>
        /// <param name="accountName">Name of the account.</param>
        /// <returns>True, when online.</returns>
        [HttpGet]
        [Route("is-online/{accountName=0}")]
        public async Task<bool> GetIsOnlineAsync(string accountName)
        {
            var isOnline = false;

            foreach (var server in this._gameServers.Values.OfType<GameServer>())
            {
                var players = await server.Context.GetPlayersAsync().ConfigureAwait(false);
                if (players.Any(p => p.Account?.LoginName == accountName))
                {
                    isOnline = true;
                    break;
                }
            }

            return isOnline;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("status")]
        public IActionResult ServerState()
        {
            int sum = 0;
            var list = new List<string>();
            _gameServers.Values.ForEach(async item =>
            {
                var server = item as GameServer;
                if(server is not null)
                {
                    await server.Context.ForEachPlayerAsync(player =>
                    {
                        list.Add(player.GetName());
                        return Task.CompletedTask;
                    }).ConfigureAwait(false);
                    sum = sum + server.Context.PlayerCount;
                }
            });

            var item = new
            {
                state = "Online",
                players = sum,
                playersList = list
            };

            return Ok(JsonSerializer.Serialize(item));
        }
    }
}