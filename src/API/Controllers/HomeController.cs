namespace MUnique.OpenMU.API.Controllers
{
    using System.Linq;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// HomeController
    /// </summary>
    public class HomeController : Controller
    {
        private IDictionary<int, IGameServer> _gameServers;
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="gameServers"></param>
        public HomeController(IDictionary<int, IGameServer> gameServers) => _gameServers = gameServers;

        [HttpGet]
        public IActionResult Index()
        {
            var item = new
            {
                action = "Home/Index",
                msg = "Your index response"
            };
            return Ok(JsonSerializer.Serialize(item));
        }

        /// <summary>
        /// SendGlobalMessage
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpGet]
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
        /// ServerState
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ServerState()
        {
            var server = (GameServer)this._gameServers.First().Value;
            if (server is not null)
            {
                var item = new
                {
                    state = server.ServerState.ToString(),
                    players = server.Context.PlayerCount,
                };
                return Ok(JsonSerializer.Serialize(item));
            }

            return Ok("Server not ready");
        }
    }
}