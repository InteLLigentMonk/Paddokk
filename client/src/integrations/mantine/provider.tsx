import { MantineProvider } from '@mantine/core'
import { theme } from './config'

export function Provider({ children }: { children: React.ReactNode }) {
  return <MantineProvider theme={theme}>{children}</MantineProvider>
}
