
using System.Text.Json;
using DnsClient.Protocol;
using MongoDB.Driver;
using MongoDB.Entities;

namespace SearchService.Data
{
    public static class DbInitializer
    {
         public static async Task InitializeAsync(this WebApplicationBuilder builder)
          {
              await DB.InitAsync(
    "SearchDb",
    MongoClientSettings.FromConnectionString(
        builder.Configuration.GetConnectionString("MongoDbConnection")
    )
);

            // Create text index on Item collection
            await DB.Index<Item>()
                .Key(i => i.Make, KeyType.Text)
                .Key(i => i.Model, KeyType.Text)
                .Key(i => i.Color, KeyType.Text)
                .CreateAsync();
        
           var count  = await DB.CountAsync<Item>();
           if (count == 0)
            {
                Console.WriteLine("Seeding initial data into Item collection...");
                var filePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "Data",
                    "auctions.json"
                );

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Seed file not found.");
                    return;
                }

                var itemData= await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);
                
                if (items != null && items.Count > 0)
                {
                    await DB.SaveAsync(items);
                    Console.WriteLine($"Seeded {items.Count} items into Item collection.");
                }
                else
                {
                    Console.WriteLine("No items found to seed.");
                }
            }


          }     
    }

}