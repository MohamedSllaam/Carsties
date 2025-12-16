
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);
 

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSVCHttpClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.UseAuthorization();
app.MapControllers(); 
// Initialize MongoDB.Entities DB
try
{
await app.InitializeAsync(); 
Console.WriteLine("Database initialized successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error initializing database: {ex.Message}");
     
}
app.Run();
