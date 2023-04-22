namespace MUnique.OpenMU.API.Controllers
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// HomeController
    /// </summary>
    public class HomeController : Controller
    {
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
        public async Task<IActionResult> SendGlobalMessage([FromQuery(Name = "msg")] string msg)
        {
            var server = GameServer.Instance;
            if (server is not null)
            {
                await server.SendGlobalMessageAsync(msg, MessageType.GoldenCenter);
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
            var server = GameServer.Instance;
            if (server is not null)
            {
                var item = new
                {
                    state = server.ServerState.ToString()
                };
                return Ok(JsonSerializer.Serialize(item));
            }

            return Ok("Server not ready");
        }
    }
}