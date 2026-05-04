import { createFileRoute, Outlet } from "@tanstack/react-router"

export const Route = createFileRoute("/_app/journeys")({
  staticData: { breadcrumb: "Journeys" },
  component: () => <Outlet />,
})
