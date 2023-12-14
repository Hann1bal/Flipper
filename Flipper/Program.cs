using System.Diagnostics;
using Flipper;
using Flipper.Hubs;
using Flipper.Models;
using Flipper.Repository;
using Flipper.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins("http://localhost:5175", "http://localhost:5173",
                    "http://109.196.164.39:5000", "https://109.196.164.39:5001", "http://109.196.164.39","http://statsofexile.online","statsofexile.online",
                    "https://109.196.164.39").AllowAnyMethod()
                .AllowAnyHeader().AllowCredentials();
        });
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddDbContextFactory<FlipperContext>
(
    optionsBuilder =>
    {
        optionsBuilder.UseNpgsql(FlipperContext.GetDatabaseConnectionString());
        optionsBuilder.EnableDetailedErrors();
    }
);

builder.Services.AddHttpClient("KnB_API", config =>
{
    config.BaseAddress = new Uri(builder.Configuration.GetSection("KbAuthProperty")["baseUri"]);
    config.Timeout = new TimeSpan(0, 0, 30);
    config.DefaultRequestHeaders.Clear();
});
builder.Services.AddSingleton<ApiRequestSenderService>();
builder.Services.AddScoped<IBaseRepository<Cards>, CardRepository>();
builder.Services.AddScoped<IBaseRepository<Currency>, CurrencyRepository>();
builder.Services.AddScoped<IBaseRepository<Gem>, GemRepository>();
builder.Services.AddScoped<IBaseRepository<Uniq>, UniqRepository>();
builder.Services.AddScoped<IBaseRepository<Account>, AccountRepository>();
builder.Services.AddSingleton<HttpNinjaService>();
builder.Services.AddSingleton<UpdateService>();
builder.Services.AddSingleton<CharacterListDownloaderService>();
builder.Services.AddHostedService<TimerService>();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();

app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.MapControllers();
if (app.Environment.IsDevelopment())
{
    System.Diagnostics.Process.Start(new ProcessStartInfo
    {
        FileName = "http://localhost:5001/index.html",
        UseShellExecute = true
    });
}

app.MapHub<GetDataHub>("/data");
app.Run();