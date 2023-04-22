namespace MUnique.OpenMU.API.Controllers
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.GameServer;

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

        [HttpGet]
        public IActionResult Test(string id)
        {
            var item = new
            {
                id,
                action = "Home/About",
                msg = "Your about response"
            };
            return Ok(JsonSerializer.Serialize(item));
        }

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