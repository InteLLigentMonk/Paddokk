import { Outlet, createFileRoute } from "@tanstack/react-router"

export const Route = createFileRoute("/_app/cars")({
  staticData: { breadcrumb: "Cars" },
  component: () => <Outlet />,
})
