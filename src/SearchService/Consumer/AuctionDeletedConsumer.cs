using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService.Consumer;


public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
        private readonly IMapper _mapper;
    public AuctionDeletedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
 public async Task Consume(ConsumeContext<AuctionDeleted> context)
 {
      Console.WriteLine("--> Consuming AuctionDeleted event");
            var result = await DB.DeleteAsync<Item>(context.Message.Id);
                
            // Logic to index the item in the search service goes here
      if(!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionDeletedConsumer), "Failed to delete item in search index.");
        }
 }
}

 