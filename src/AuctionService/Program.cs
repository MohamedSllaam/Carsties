using AuctionService;
using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
   x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
   {
       o.UsePostgres();
       o.QueryDelay = TimeSpan.FromSeconds(10);
       //o.DisableInboxCleanupService = false;
       //o.MessageLifetime = TimeSpan.FromDays(1);
       o.UseBusOutbox();
   });
    
    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
     

   x.UsingRabbitMq((context, cfg) => 
   {
        cfg.ConfigureEndpoints(context);
   });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(option => {
       option.Authority = builder.Configuration["IdentityServiceUrl"];
       option.RequireHttpsMetadata = false;
       option.TokenValidationParameters.ValidateAudience = false;
       option.TokenValidationParameters.NameClaimType = "username";
   });

var app = builder.Build();
 

app.UseHttpsRedirection();

app.UseAuthentication();
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

 
