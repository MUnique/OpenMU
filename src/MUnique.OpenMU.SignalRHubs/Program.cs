var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ServerStateHub>("/signalr/hubs/serverState");
});

app.Run();
