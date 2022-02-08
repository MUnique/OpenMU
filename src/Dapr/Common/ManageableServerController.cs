using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.Dapr.Common;

[ApiController]
[Route("")]
public class ManageableServerController
{
    private readonly IManageableServer _manageableServer;

    public ManageableServerController(IManageableServer manageableServer)
    {
        _manageableServer = manageableServer;
    }

    [HttpPost(nameof(IManageableServer.Shutdown))]
    public void Shutdown()
    {
        this._manageableServer.Shutdown();
    }

    [HttpPost(nameof(IManageableServer.Start))]
    public void Start()
    {
        this._manageableServer.Start();
    }
}