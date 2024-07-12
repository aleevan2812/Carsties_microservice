"use server";

import { PagedResult, Auction } from "@/public/types";

export async function getData(
  pageNumber: number,
  pageSize: number
): Promise<PagedResult<Auction>> {
  const res = await fetch(
    `http://localhost:6001/search?pageSize=${pageSize}&pageNumber=${pageNumber}`
  );

  // console.log(res);
  if (!res.ok) throw new Error("Failed to fetch data");

  return res.json();
}
