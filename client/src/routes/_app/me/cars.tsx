import { createFileRoute, Outlet } from "@tanstack/react-router";

export const Route = createFileRoute("/_app/me/cars")({
  component: () => <Outlet />,
});
