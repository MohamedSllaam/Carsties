using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Add DbContext
builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
   x.UsingRabbitMq((context, cfg) => 
   {
        cfg.ConfigureEndpoints(context);
   });
});
var app = builder.Build();
 

app.UseHttpsRedirection();


app.UseAuthorization();
app.MapControllers(); 
try
{
    DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred during database initialization: {ex.Message}");
}   
app.Run();

 
