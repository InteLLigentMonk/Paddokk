import { Outlet, createFileRoute } from "@tanstack/react-router"

export const Route = createFileRoute("/_app/journeys")({
  staticData: { breadcrumb: "Journeys" },
  component: () => <Outlet />,
})
