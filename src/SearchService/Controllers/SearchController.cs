using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace SearchService.Controllers;

    [ApiController]
   [Route("api/[controller]")]
    public class SearchController:ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> SearchItems([FromQuery] string searchTerm)
        {
             var query = DB.Find<Item>();
             query.Sort(x => x.Ascending(a=>a.Make));
             if(!string.IsNullOrEmpty(searchTerm))
        {
          query.Match(Search.Full , searchTerm).SortByTextScore();  
          
  }
             var results = await query.ExecuteAsync(); 

               return Ok(results);
        }
        
    }
