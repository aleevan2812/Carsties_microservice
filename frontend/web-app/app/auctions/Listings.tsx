// 'use client'

import React from "react";
import { json } from "stream/consumers";
import AuctionCard from "./AuctionCard";

async function getData() {
  const res = await fetch("http://localhost:6001/search");

  // console.log(res);
  if (!res.ok) throw new Error("Failed to fetch data");

  return res.json();
}

export default async function Listings() {
  const data = await getData();
  return (
    <div>
      {data &&
        data.results.map((auction: any) => (
          <AuctionCard auction={auction} key={auction.id} />
        ))}
    </div>
  );
}
