import { createRouter } from "@tanstack/react-router";
import { setupRouterSsrQueryIntegration } from "@tanstack/react-router-ssr-query";
import * as TanstackQuery from "./integrations/tanstack-query/root-provider";

// Import the generated route tree
import { routeTree } from "./routeTree.gen";

declare module "@tanstack/react-router" {
  interface StaticDataRouteOption {
    breadcrumb?: string | ((loaderData: unknown) => string);
  }
}

// Create a new router instance
export const getRouter = () => {
  const rqContext = TanstackQuery.getContext();

  const router = createRouter({
    routeTree,
    context: {
      ...rqContext,
      auth: {
        isAuthenticated: false,
        user: null,
      },
    },

    defaultPreload: "intent",
    defaultPreloadStaleTime: 5 * 60 * 1000,
    defaultStaleTime: 30 * 1000,
  });

  setupRouterSsrQueryIntegration({
    router,
    queryClient: rqContext.queryClient,
  });

  return router;
};
