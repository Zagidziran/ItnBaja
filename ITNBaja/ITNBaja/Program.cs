using ITNBaja.Client.Pages;
using ITNBaja.Components;
using ITNBaja.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

// Add HttpClient service
builder.Services.AddHttpClient();

// Add API controllers
builder.Services.AddControllers();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add authentication service
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<TokenService>();

// Add memory cache for token storage
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

// Add session middleware
app.UseSession();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(ITNBaja.Client._Imports).Assembly);

app.Run();
