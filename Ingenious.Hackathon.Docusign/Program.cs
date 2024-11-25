using Ingenious.Hackathon.Docusign.Models;
using Ingenious.Hackathon.Docusign.Services;
using Ingenious.Hackathon.Docusign.Sql;
using Ingenious.Hackathon.Docusign.Sql.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var azureAdSection = builder.Configuration.GetSection("AzureAd");

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(azureAdSection)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.CallbackPath = new PathString("/Home/LoginSuccess");
});

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();


builder.Services.AddDbContext<HackathonDbContext, HackathonDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
           sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
           maxRetryCount: 5,        // Max retry attempts
           maxRetryDelay: TimeSpan.FromSeconds(10),  // Max delay between retries
           errorNumbersToAdd: null))  // Optionally specify SQL error numbers to retry on
       );

builder.Services.Configure<DocuSignSettings>(builder.Configuration.GetSection("DocuSign"));
builder.Services.Configure<DocuSignJWT>(builder.Configuration.GetSection("DocuSignJWT"));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DocuSignServices>();
builder.Services.AddScoped<DocuSignTokenService>();

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

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers();

app.Run();
