
using System.Text.Json;
using DnsClient.Protocol;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Services;

namespace SearchService.Data
{
    public static class DbInitializer
    {
         public static async Task InitializeAsync(this WebApplication app)
          {
              await DB.InitAsync(
    "SearchDb",
    MongoClientSettings.FromConnectionString(
        app.Configuration.GetConnectionString("MongoDbConnection")
    )
);

            // Create text index on Item collection
            await DB.Index<Item>()
                .Key(i => i.Make, KeyType.Text)
                .Key(i => i.Model, KeyType.Text)
                .Key(i => i.Color, KeyType.Text)
                .CreateAsync();
           
           var count  = await DB.CountAsync<Item>();

           using var scope = app.Services.CreateScope();
              var auctionClient = scope.ServiceProvider.GetRequiredService<AuctionSVCHttpClient>();

              var items= await auctionClient.GetAuctionsAsync();
                if(count == 0)
                {
                    await DB.SaveAsync(items);
                }
                 
        
          }
          }}