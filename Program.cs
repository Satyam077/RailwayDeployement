using Microsoft.Extensions.Options;
using RailwayDeployement.Service;
using RailwayDeployement;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddHealthChecks();

builder.Services.Configure<OmnisendConfig>(builder.Configuration.GetSection("Omnisend"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<OmnisendConfig>>().Value);

builder.Services.AddHttpClient<OmnisendService>();
builder.Services.AddScoped<OmnisendService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var corsPolicyName = "AllowAngularDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:64421")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors(corsPolicyName);
//app.UseSwagger();
//app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
