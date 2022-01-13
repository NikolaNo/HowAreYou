
using BlazorSentimentAnalysis.Server.ML.DataModels;
using HowAreYou.Areas.Identity;
using HowAreYou.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Caching.Memory;
using HowAreYou.Controllers;
using System.Net;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMemoryCache();
builder.Services.AddPredictionEnginePool<SampleObservation, SamplePrediction>()
.FromFile(builder.Configuration["MLModel:MLModelFilePath"]);
builder.Services.AddSingleton<SentimentController>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.Use(async (context, next) =>
{
    if (context.Request.Path.ToString().EndsWith(""))
    { 
        MemoryCache Cache = (MemoryCache)context.RequestServices.GetRequiredService<IMemoryCache>();
        var key = context.Connection.RemoteIpAddress;
        Cache.TryGetValue(key, out int Count);
        if (Count > 20)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsync("We dont do that here");
        }
        else {
            var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
            Cache.Set(key, Count + 1, options);
        }
    }
    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
