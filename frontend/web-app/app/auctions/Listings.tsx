// 'use client'

import React from "react";
import { json } from "stream/consumers";
import AuctionCard from "./AuctionCard";
import { Auction, PagedResult } from "@/public/types";

async function getData(): Promise<PagedResult<Auction>> {
  const res = await fetch("http://localhost:6001/search?pageSize=10");

  // console.log(res);
  if (!res.ok) throw new Error("Failed to fetch data");

  return res.json();
}

export default async function Listings() {
  const data = await getData();
  return (
    <div className="grid grid-cols-4 gap-6">
      {data &&
        data.results.map((auction: any) => (
          <AuctionCard auction={auction} key={auction.id} />
        ))}
    </div>
  );
}
