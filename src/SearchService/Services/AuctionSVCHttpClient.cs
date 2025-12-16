using MongoDB.Entities;

namespace SearchService.Services
{
    public class AuctionSVCHttpClient
    {

  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;
         
        public AuctionSVCHttpClient(HttpClient httpClient ,IConfiguration configuration)
        {
   _configuration = configuration;
   _httpClient = httpClient;
            
        }

        public async Task<List<Item>> GetAuctionsAsync()
        {
            var lastUpdated= await DB.Find<Item,string>()
            .Sort(x=> x.Descending(a=> a.UpdatedAt))
            .Project(x=> x.UpdatedAt.ToString("o"))
            .ExecuteFirstAsync();

            var auctionServiceUrl = _configuration.GetValue<string>("AuctionServiceUrl");
            var requestUrl = string.IsNullOrEmpty(lastUpdated) 
                ? $"{auctionServiceUrl}/api/auctions" 
                : $"{auctionServiceUrl}/api/auctions?date={Uri.EscapeDataString(lastUpdated)}";

         return await _httpClient.GetFromJsonAsync<List<Item>>(requestUrl);
           

            
        }   
    }
}