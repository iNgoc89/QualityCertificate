using Blazored.LocalStorage;
using HoangThach.AccountShared.Data;
using HoangThach.AccountShared.Models.Entities;
using HoangThach.AccountShared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using QualityCertificate.Components;
using QualityCertificate.Datas.Entity;
using QualityCertificate.Datas.Models;
using QualityCertificate.Services;
using System.Text;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

var ssoConnectionString = builder.Configuration.GetConnectionString("SSODbContext");
var wsmConnectionString = builder.Configuration.GetConnectionString("WSMDbContext");

builder.Services.AddDbContext<WSMDbContext>(options =>
       options.UseSqlServer(wsmConnectionString));

#region SSO
builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(ssoConnectionString));

var sp = builder.Services.BuildServiceProvider();
var scope = sp.CreateScope();
var authDB = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

var secretConfig = authDB.SecretKeys?.FirstOrDefault(f => f.Key == "SecretKey" && f.IsActive);

string secretKey = secretConfig?.Value ?? "yourSecretKeyWithAtLeast32Characters1234567890";

builder.Services.AddSingleton<System.Text.Encodings.Web.UrlEncoder>(UrlEncoder.Default);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthenticationStateProvider, HoangThach.AccountShared.Services.AuthenticationService>();

builder.Services.AddScoped<HoangThach.AccountShared.Services.AuthenticationService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient();
#endregion

builder.Services.AddScoped<IBulkCementCQ, BulkCementCQService>();
builder.Services.AddSingleton<RabbitMQClient_Topic>();

builder.Services.AddScoped<QRService>();
builder.Services.AddScoped<WordOpenXmlService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

BackgroundTask.getInstance().Init(app, builder.Configuration);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
