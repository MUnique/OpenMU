namespace MUnique.OpenMU.Web.API
{
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using System.Text.Json;

    /// <summary>
    /// Server API controller
    /// </summary>
    [Route("api/")]
    public class ServerController : Controller
    {
        private IDictionary<int, IGameServer> _gameServers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameServers"></param>
        public ServerController(IDictionary<int, IGameServer> gameServers) => _gameServers = gameServers;

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