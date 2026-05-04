import { createFileRoute, Outlet } from "@tanstack/react-router"

export const Route = createFileRoute("/_app/cars")({
  staticData: { breadcrumb: "Cars" },
  component: () => <Outlet />,
})
