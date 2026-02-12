import { Box, Container } from '@mantine/core'

interface SectionWrapperProps {
  children: React.ReactNode
  id?: string
  variant?: 'default' | 'muted' | 'primary'
  py?: string
}

export function SectionWrapper({
  children,
  id,
  variant = 'default',
  py = 'calc(var(--mantine-spacing-xl) * 3)',
}: SectionWrapperProps) {
  const bgMap = {
    default: undefined,
    muted: 'var(--mantine-color-gray-0)',
    primary: 'var(--mantine-color-myColor-0)',
  } as const

  return (
    <Box
      component="section"
      id={id}
      py={py}
      style={{ backgroundColor: bgMap[variant] }}
    >
      <Container size="lg">{children}</Container>
    </Box>
  )
}
