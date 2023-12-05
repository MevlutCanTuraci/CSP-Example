#region Imports
using System.Security.Cryptography;

#endregion


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.Use(async (context, next) =>
{
    var rng = RandomNumberGenerator.Create();
    var nonceBytes = new byte[32];
    rng.GetBytes(nonceBytes);
    var nonce = Convert.ToBase64String(nonceBytes);
    context.Items.Add("ScriptNonce", nonce);

    context.Response.Headers.Add("Content-Security-Policy",
    new[] { string.Format("script-src 'self' https://ajax.cloudflare.com https://gstatic.com 'nonce-{0}'", nonce) });

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();