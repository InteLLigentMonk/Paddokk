import { Outlet, createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_app/users/$username/journeys")({
  staticData: { breadcrumb: "Journeys" },
  component: () => <Outlet />,
});
