import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_marketing/')({
  component: LandingPage,
})

function LandingPage() {
  return (
    <div>
      {/* Sections will be added in Phase 2 */}
      <p>Landing page content coming soon</p>
    </div>
  )
}
