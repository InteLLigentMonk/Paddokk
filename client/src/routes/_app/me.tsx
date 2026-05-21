import { Outlet, createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_app/me")({
  staticData: { breadcrumb: "Me" },
  component: () => <Outlet />,
});
