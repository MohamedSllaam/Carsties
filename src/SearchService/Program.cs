
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumer;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);
 

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSVCHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
   
    x.UsingRabbitMq((context, cfg) => 
    {
            cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.UseAuthorization();
app.MapControllers(); 
// Initialize MongoDB.Entities DB
app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await app.InitializeAsync(); 
        Console.WriteLine("Database initialized successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
    }
});

app.Run();
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
=>
     HttpPolicyExtensions.
        HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
