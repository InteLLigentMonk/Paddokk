import {
  HeadContent,
  Outlet,
  Scripts,
  createRootRouteWithContext,
} from "@tanstack/react-router";
import { TanStackRouterDevtoolsPanel } from "@tanstack/react-router-devtools";
import { TanStackDevtools } from "@tanstack/react-devtools";
import { ColorSchemeScript, mantineHtmlProps } from "@mantine/core";
import TanStackQueryDevtools from "../integrations/tanstack-query/devtools";

import appCss from "../styles.css?url";

import {
  Provider as MantineProvider,
  NotificationsContainer,
} from "../integrations/mantine";

import { authSessionQueryOptions } from "../lib/api/auth.queries";
import {
  ConsentProvider,
  
  readConsentRecord
} from "../lib/consent";
import { ConsentBanner } from "../components/consent";
import type {ConsentRecord} from "../lib/consent";
import type { QueryClient } from "@tanstack/react-query";

interface MyRouterContext {
  queryClient: QueryClient;
  auth: {
    isAuthenticated: boolean;
    user: {
      id: string;
      name: string;
      email: string;
      image?: string;
    } | null;
  };
  consent: ConsentRecord | null;
}

export const Route = createRootRouteWithContext<MyRouterContext>()({
  beforeLoad: async ({ context: { queryClient } }) => {
    const session = await queryClient.ensureQueryData(
      authSessionQueryOptions(),
    );

    return {
      auth: {
        isAuthenticated: !!session.user,
        user: session.user ?? null,
      },
      consent: await readConsentRecord(),
    };
  },

  head: () => ({
    meta: [
      {
        charSet: "utf-8",
      },
      {
        name: "viewport",
        content: "width=device-width, initial-scale=1",
      },
      {
        title: "Paddokk - The Social Platform for Car Enthusiasts",
      },
      {
        name: "description",
        content:
          "Document your car journeys, share your builds, and connect with fellow car enthusiasts. Paddokk bridges the gap between forums and modern social platforms.",
      },
    ],
    links: [
      {
        rel: "stylesheet",
        href: appCss,
      },
    ],
  }),

  shellComponent: RootDocument,
  component: RootProviders,
});

function RootDocument({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" {...mantineHtmlProps}>
      <head>
        <ColorSchemeScript defaultColorScheme="auto" />
        <HeadContent />
      </head>
      <body>
        {children}
        <TanStackDevtools
          config={{
            position: "bottom-right",
          }}
          plugins={[
            {
              name: "Tanstack Router",
              render: <TanStackRouterDevtoolsPanel />,
            },
            TanStackQueryDevtools,
          ]}
        />
        <Scripts />
      </body>
    </html>
  );
}

function RootProviders() {
  const { consent } = Route.useRouteContext();

  return (
    <MantineProvider>
      <NotificationsContainer />
      <ConsentProvider initialRecord={consent}>
        <Outlet />
        <ConsentBanner />
      </ConsentProvider>
    </MantineProvider>
  );
}
