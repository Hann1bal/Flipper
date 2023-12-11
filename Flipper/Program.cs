using System.Diagnostics;
using Flipper;
using Flipper.Hubs;
using Flipper.Models;
using Flipper.Repository;
using Flipper.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder => { corsPolicyBuilder.WithOrigins("http://localhost:5175", "http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials(); });
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddDbContextFactory<FlipperContext>
(
    optionsBuilder => { optionsBuilder.EnableDetailedErrors(false); }
);

builder.Services.AddHttpClient("KnB_API", config =>
{
    config.BaseAddress = new Uri(builder.Configuration.GetSection("KbAuthProperty")["baseUri"]);
    config.Timeout = new TimeSpan(0, 0, 30);
    config.DefaultRequestHeaders.Clear();
});
builder.Services.AddSingleton<ApiRequestSenderService>();
builder.Services.AddSingleton<IBaseRepository<Cards>, CardRepository>();
builder.Services.AddSingleton<IBaseRepository<Currency>, CurrencyRepository>();
builder.Services.AddSingleton<IBaseRepository<Gem>, GemRepository>();
builder.Services.AddSingleton<IBaseRepository<Uniq>, UniqRepository>();
builder.Services.AddSingleton<HttpNinjaService>();

builder.Services.AddSingleton<UpdateService>();

builder.Services.AddHostedService<TimerService>();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.MapControllers();
System.Diagnostics.Process.Start(new ProcessStartInfo
{
    FileName = "http://localhost:5027/index.html",
    UseShellExecute = true
});
app.MapHub<GetDataHub>("/data");
app.Run("http://localhost:5027");