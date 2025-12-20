using Contracts;
using MassTransit;

namespace AuctionService
{
 public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctioCreated>>
 {
  public async Task Consume(ConsumeContext<Fault<AuctioCreated>> context)
  {
   Console.WriteLine($"AuctionCreatedFaultConsumer: Handling fault for Auction ID: {context.Message.Message.Id}");
    
    var exception = context.Message.Exceptions.First();

    if(exception.ExceptionType == "System.ArgumentException")
    {
        context.Message.Message.Model = "FooBar";
        await context.Publish(context.Message.Message);
        // Additional handling logic for ArgumentException
    }
    else
    {
        Console.WriteLine("Not an argument exception - update error dashboad somewhere");
        // General handling logic for other exceptions
    }
  }
 }
}