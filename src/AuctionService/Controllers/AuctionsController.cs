using System.Linq;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

 [ApiController]
 [Route("api/[controller]")]
 public class AuctionsController: ControllerBase
 {
  private readonly IMapper _mapper;
 private readonly AuctionDbContext _auctionDbContext;

 public AuctionsController(IMapper mapper,AuctionDbContext auctionDbContext )
     {  
         _mapper = mapper;
         _auctionDbContext = auctionDbContext;
     }

    [HttpGet]
    public async Task<IActionResult> GetAuctions(string date)
    {

        var query = _auctionDbContext.Auctions.OrderBy(x=> x.Item.Make).AsQueryable();

     if (!string.IsNullOrWhiteSpace(date) && !DateTime.TryParse(date, out var dt))
        return BadRequest("Invalid date format. Use yyyy-MM-dd.");
     
        if(!string.IsNullOrEmpty(date))
        {
            query =query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime())>= 0);

        }


        var auctions = await _auctionDbContext.Auctions
        .Include(a => a.Item)
        .OrderBy(a => a.Item.Make)
        .ToListAsync();
       

        // var auctionDtos = _mapper.Map<List<AuctionDto>>(auctions);

        return  await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync()
            is List<AuctionDto> auctionDtos
            ? Ok(auctionDtos)
            : StatusCode(500, "An error occurred while retrieving auctions.");
    } 

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuctionById(Guid id)
    {
        var auction = await _auctionDbContext.Auctions
        .Include(a => a.Item)
        .FirstOrDefaultAsync(a => a.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        var auctionDto = _mapper.Map<AuctionDto>(auction);
        return Ok(auctionDto);
    }
    
   [HttpPost]
    public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto createAuctionDto)
    {
        var auction = _mapper.Map<Auction>(createAuctionDto);
       // auction.Id = Guid.NewGuid();
       // auction.Item.Id = Guid.NewGuid();
        auction.Seller="TestSeller"; // Placeholder, replace with actual seller info

        await _auctionDbContext.Auctions.AddAsync(auction);
        var result=  await _auctionDbContext.SaveChangesAsync();
        if(result<=0)
        {
            return StatusCode(500, "An error occurred while creating the auction.");
        }
        var auctionDto = _mapper.Map<AuctionDto>(auction);
        return CreatedAtAction(nameof(GetAuctionById),
         new { id = auction.Id }, auctionDto);
    } 
     
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuction(Guid id, [FromBody] UpdateAuctionDto updateAuctionDto)
    {
        var existingAuction = await _auctionDbContext.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (existingAuction == null)
        {
            return NotFound();
        }

        // Update auction and item properties
        existingAuction.Item.Make = updateAuctionDto.Make ??  existingAuction.Item.Make ;
        existingAuction.Item.Model = updateAuctionDto.Model ?? existingAuction.Item.Model ;
        existingAuction.Item.Year = updateAuctionDto.Year ?? existingAuction.Item.Year ;  
        existingAuction.Item.Color = updateAuctionDto.Color ?? existingAuction.Item.Color ;
        existingAuction.Item.Mileage = updateAuctionDto.Mileage ?? existingAuction.Item.Mileage ;


        var result = await _auctionDbContext.SaveChangesAsync();
        if (result <= 0)
        {
            return StatusCode(500, "An error occurred while updating the auction.");
        }


         return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuction(Guid id)
    {
        var existingAuction = await _auctionDbContext.Auctions 
            .FindAsync(id);

        if (existingAuction == null)
        {
            return NotFound();
        }

        _auctionDbContext.Auctions.Remove(existingAuction);
        var result = await _auctionDbContext.SaveChangesAsync();
        if (result <= 0)
        {
            return StatusCode(500, "An error occurred while deleting the auction.");
        }

        return NoContent();
    }
 }
