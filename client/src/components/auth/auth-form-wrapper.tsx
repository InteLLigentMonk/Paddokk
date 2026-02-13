import { Stack, Text, Title } from "@mantine/core";

interface AuthFormWrapperProps {
  title: string;
  subtitle?: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
}

/**
 * Wrapper component for auth forms
 * Provides consistent branding and layout
 */
export function AuthFormWrapper({
  title,
  subtitle,
  children,
  footer,
}: AuthFormWrapperProps) {
  return (
    <Stack gap="xl">
      {/* Header */}
      <Stack gap="xs" ta="center">
        <Text fw={700} fz="xl" c="myColor">
          Paddokk
        </Text>
        <Title order={2} size="h3" fw={600}>
          {title}
        </Title>
        {subtitle && (
          <Text size="sm" c="dimmed">
            {subtitle}
          </Text>
        )}
      </Stack>

      {/* Form Content */}
      {children}

      {/* Footer */}
      {footer && (
        <Text size="sm" ta="center" c="dimmed">
          {footer}
        </Text>
      )}
    </Stack>
  );
}
