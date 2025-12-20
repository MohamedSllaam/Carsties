using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumer;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
        private readonly IMapper _mapper;
    public AuctionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
 public async Task Consume(ConsumeContext<AuctionUpdated> context)
 {
   var auctionUpdated = context.Message;

            var item = _mapper.Map<Item>(auctionUpdated);

            var result = await DB.Update<Item>()
                .Match(x =>  x.ID == context.Message.Id)
                 .ModifyOnly(x=> new
                 {
                        x.Make,
                        x.Model,
                        x.Year,
                        x.Color,
                        x.Mileage
                    
                 } , item)
                .ExecuteAsync(); 
            // Logic to index the item in the search service goes here
            if(!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionUpdatedConsumer), "Failed to update item in search index.");
        }
 }
}

 