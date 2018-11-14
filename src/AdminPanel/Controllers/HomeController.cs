// <copyright file="HomeController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The home controller which just returns the home view of the admin panel web app.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("admin")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Returns the home view.
        /// </summary>
        /// <returns>The home view.</returns>
        [HttpGet]
        public IActionResult Index() => this.View("Index");
    }
}
