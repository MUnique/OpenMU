using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Setup.Data;

// Ensure some Assemblies are loaded, which provide plugins
_ = MUnique.OpenMU.GameLogic.Rand.NextInt(1, 2);
_ = DataInitialization.Id;
_ = MUnique.OpenMU.GameServer.ClientVersionResolver.DefaultVersion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services//.AddSingleton<SetupService>()
    .AddSingleton<IMigratableDatabaseContextProvider, PersistenceContextProvider>()
    .AddSingleton(s => (PersistenceContextProvider)s.GetService<IMigratableDatabaseContextProvider>()!)
    .AddSingleton(s => (IPersistenceContextProvider)s.GetService<IMigratableDatabaseContextProvider>()!)
    .AddSingleton(s =>
    {
        var plugInManager = new PlugInManager(null, s.GetService<ILoggerFactory>()!, s);
        plugInManager.DiscoverAndRegisterPlugInsOf<IDataInitializationPlugIn>();
        return plugInManager;
    })
    ;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
