import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/_app/me/")({
  loader: () => {
    throw redirect({ to: "/dashboard" });
  },
});
