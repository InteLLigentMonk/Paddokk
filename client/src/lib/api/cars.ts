import { createServerFn } from "@tanstack/react-start";
import {
  CarsBrowseStatsQueryParams,
  CarsGetCarParams,
  CarsSearchCarsQueryParams,
} from "@/generated/api-zod/cars/cars.zod";
import {
  carsBrowseStats,
  carsGetCar,
  carsSearchCars,
} from "@/generated/api/cars/cars";

// CarSearchSort matches backend enum ordinal: 0=Relevance, 1=Newest, 2=MostLiked, 3=MostJourneys
export const CAR_SEARCH_SORT = {
  Relevance: 0,
  Newest: 1,
  MostLiked: 2,
  MostJourneys: 3,
} as const;

export type CarSortKey = keyof typeof CAR_SEARCH_SORT;

export const getPublicCarFn = createServerFn({ method: "GET" })
  .inputValidator(CarsGetCarParams)
  .handler(async ({ data: { carId } }) => await carsGetCar(Number(carId)));

export const searchCarsFn = createServerFn({ method: "GET" })
  .inputValidator(CarsSearchCarsQueryParams)
  .handler(async ({ data }) => await carsSearchCars(data));

export const getCarsBrowseStatsFn = createServerFn({ method: "GET" })
  .inputValidator(CarsBrowseStatsQueryParams)
  .handler(async ({ data }) => await carsBrowseStats(data));
