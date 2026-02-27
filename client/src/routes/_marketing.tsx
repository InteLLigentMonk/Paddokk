import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";
import { MarketingFooter, MarketingHeader } from "@/components/marketing";
import { AuthModal } from "@/components/auth";

export const Route = createFileRoute("/_marketing")({
  beforeLoad: ({ context }) => {
    // Redirect authenticated users to the app
    if (context.auth.isAuthenticated) {
      throw redirect({ to: "/dashboard" });
    }
  },
  component: MarketingLayout,
});

function MarketingLayout() {
  return (
    <div className="flex min-h-screen flex-col">
      <MarketingHeader />
      <main className="flex-1">
        <Outlet />
      </main>
      <MarketingFooter />
      <AuthModal />
    </div>
  );
}
