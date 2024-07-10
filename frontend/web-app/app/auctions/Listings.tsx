// 'use client'

import React from "react";
import { json } from "stream/consumers";

async function getData() {
  const res = await fetch("http://localhost:6001/search");

  // console.log(res);
  if (!res.ok) throw new Error("Failed to fetch data");

  return res.json();
}

export default async function Listings() {
  const data = await getData();
  return <div>{JSON.stringify(data, null, 2)}</div>;
}
