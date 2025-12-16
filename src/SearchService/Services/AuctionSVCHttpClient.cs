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
    }
}