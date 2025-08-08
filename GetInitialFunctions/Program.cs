using GetInitialFunctions.Services;
using Interv.API.DAL;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddQueueServiceClient("UseDevelopmentStorage=true");
});

builder.Services.AddScoped<ITagDiscoveryApiService, TagDiscoveryApiService>();
builder.Services.AddHttpClient<TagDiscoveryApiService>("tagdiscovery", options =>
{
    options.BaseAddress = new Uri("https://tagdiscovery.com");
});
builder.Services.AddScoped<IInitialsSerivice, InitialsSerivice>();
builder.Services.AddScoped<IQueueService,AzureQueueService>();

builder.Services.AddDbContext<InitialsContext>(builder =>
{
    builder.UseInMemoryDatabase("InitialsDb");
});

builder.ConfigureFunctionsWebApplication();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InitialsContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
