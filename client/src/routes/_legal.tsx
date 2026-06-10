import { Outlet, createFileRoute } from "@tanstack/react-router";
import { MarketingFooter, MarketingHeader } from "@/components/marketing";
import { AuthModal } from "@/components/auth";

/**
 * Layout for public legal pages (Privacy Policy, Terms of Service). Unlike
 * `_marketing` and `_app`, it deliberately performs no auth redirect: these
 * pages must be reachable by both signed-in and anonymous visitors. It reuses
 * the marketing header/footer so the pages render with the global site chrome.
 */
export const Route = createFileRoute("/_legal")({
  component: LegalLayout,
});

function LegalLayout() {
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
