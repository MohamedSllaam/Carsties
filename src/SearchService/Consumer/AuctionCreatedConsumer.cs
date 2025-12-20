using AutoMapper;
using Contracts;
using MassTransit;

namespace SearchService.Consumer;


 public class AuctionCreatedConsumer : IConsumer<AuctioCreated>
 {
    private readonly IMapper _mapper;
    public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
    public Task Consume(ConsumeContext<AuctioCreated> context)
        { 
            var auctionCreated = context.Message;
            var item = _mapper.Map<Item>(auctionCreated);
            // Logic to index the item in the search service goes here
            return Task.CompletedTask;     
        }
    }
