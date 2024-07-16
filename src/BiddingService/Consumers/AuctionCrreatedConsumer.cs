﻿using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Consumers;

public class AuctionCrreatedConsumer : IConsumer<AuctionCreated>
{
	public async Task Consume(ConsumeContext<AuctionCreated> context)
	{
		var auction = new Auction
		{
			ID = context.Message.Id.ToString(),
			Seller = context.Message.Seller.ToString(),
			AuctionEnd = context.Message.AuctionEnd,
			ReservePrice = context.Message.ReservePrice,
		};

		await auction.SaveAsync();
	}
}