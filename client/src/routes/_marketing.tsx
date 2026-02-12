import { createFileRoute, Outlet } from '@tanstack/react-router'
import { MarketingHeader, MarketingFooter } from '@/components/marketing'

export const Route = createFileRoute('/_marketing')({
  component: MarketingLayout,
})

function MarketingLayout() {
  return (
    <div className="flex min-h-screen flex-col">
      <MarketingHeader />
      <main className="flex-1">
        <Outlet />
      </main>
      <MarketingFooter />
    </div>
  )
}
