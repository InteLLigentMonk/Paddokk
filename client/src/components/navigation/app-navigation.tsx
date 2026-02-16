import { useMediaQuery, useDisclosure } from '@mantine/hooks'
import { DesktopSidebar } from './desktop-sidebar'
import { MobileBottomBar } from './mobile-bottom-bar'
import { MobileMoreDrawer } from './mobile-more-drawer'

export function AppNavigation() {
  const isDesktop = useMediaQuery('(min-width: 62em)')
  const [moreDrawerOpened, { toggle, close }] = useDisclosure()

  if (isDesktop) {
    return <DesktopSidebar />
  }

  return (
    <>
      <MobileBottomBar onMoreClick={toggle} />
      <MobileMoreDrawer opened={moreDrawerOpened} onClose={close} />
    </>
  )
}
