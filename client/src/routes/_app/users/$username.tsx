import { Outlet, createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_app/users/$username")({
  loader: ({ params }) => ({ username: params.username }),
  staticData: {
    breadcrumb: (loaderData) => {
      const data = loaderData as { username?: string } | undefined;
      return data?.username ? `@${data.username}` : "User";
    },
  },
  component: () => <Outlet />,
});
