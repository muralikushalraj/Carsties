using AuctionService.Data;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer(AuctionDbContext auctionDbContext) : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext _auctionDbContext = auctionDbContext;

        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming bid placed");

            var auction = await _auctionDbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

            if (auction.CurrentHighBid == null
                || context.Message.BidStatus.Contains("Accepted")
                && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _auctionDbContext.SaveChangesAsync();
            }
        }
    }
}
