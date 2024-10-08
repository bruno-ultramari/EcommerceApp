using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using EcommerceApp.Models;
using EcommerceApp.Pages;
using EcommerceApp.Controllers;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Carregar segredos do Secret Manager no ambiente de desenvolvimento
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Identity (usuários e roles)
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Outras opções de configuração de identidade, se necessário
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<SignInManager<User>>();

// Configurar SMTP usando segredos do Secret Manager
builder.Services.AddTransient<SmtpClient>(provider =>
{
    var config = builder.Configuration.GetSection("Smtp");

    // Use o operador ?? para fornecer valores padrão caso as configurações sejam nulas
    var host = config["Host"] ?? throw new InvalidOperationException("SMTP Host não configurado.");
    var port = config["Port"] ?? throw new InvalidOperationException("SMTP Port não configurado.");
    var userName = config["UserName"] ?? throw new InvalidOperationException("SMTP UserName não configurado.");
    var password = config["Password"] ?? throw new InvalidOperationException("SMTP Password não configurado.");

    return new SmtpClient
    {
        Host = host,
        Port = int.Parse(port),
        EnableSsl = bool.Parse(config["EnableSSL"] ?? "false"), // Considera um valor padrão se for nulo
        Credentials = new System.Net.NetworkCredential(userName, password)
    };
});


// Adicionar serviço de autorização
builder.Services.AddAuthorization();

// Adicionar suporte para controllers e views (MVC)
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
