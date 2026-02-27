import {
  ActionIcon,
  Avatar,
  Box,
  Container,
  Group,
  Kbd,
  Menu,
  UnstyledButton,
  rem,
} from "@mantine/core";
import { spotlight } from "@mantine/spotlight";
import { useMediaQuery } from "@mantine/hooks";
import { LogOut, Search, Settings, User } from "lucide-react";
import { useAuth } from "@/hooks/use-auth";
import { ColorSchemeToggle } from "./color-scheme-toggle";

const iconProps = { size: 18, strokeWidth: 1.5 } as const;


function HeaderLogo({ showCarDropdown }: { showCarDropdown: boolean }) {
  return (
    <Group gap="md" wrap="nowrap">
      <Box
        component="img"
        src="/Logo.svg"
        alt="Paddokk"
        style={{
          height: rem(40),
          width: rem(40),
          cursor: "pointer",
        }}
      />
      {showCarDropdown && (
        <Box
          style={{
            fontSize: rem(14),
            color: "var(--mantine-color-dimmed)",
          }}
        >
          {/* TODO: Implement car dropdown */}
          My Car
        </Box>
      )}
    </Group>
  );
}

function HeaderSearch({ isLargeScreen }: { isLargeScreen: boolean }) {
  const isMac =
    typeof navigator !== "undefined" && /Mac|iPhone|iPad|iPod/.test(navigator.userAgent);

  return (
    <Box style={{ flex: 1, maxWidth: rem(500), marginInline: "auto" }}>
      <UnstyledButton
        onClick={() => spotlight.open()}
        style={{
          width: "100%",
          height: isLargeScreen ? rem(36) : rem(30),
          paddingLeft: rem(12),
          paddingRight: rem(12),
          borderRadius: rem(1000),
          border: "1px solid var(--mantine-color-default-border)",
          backgroundColor:
            "light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-6))",
          display: "flex",
          alignItems: "center",
          gap: rem(8),
          cursor: "pointer",
          transition: "border-color 100ms ease",
          // }}
          // onMouseEnter={(e) => {
          //   e.currentTarget.style.borderColor = "var(--mantine-color-gray-4)";
          // }}
          // onMouseLeave={(e) => {
          //   e.currentTarget.style.borderColor =
          //     "var(--mantine-color-default-border)";
        }}
      >
        <Search
          {...iconProps}
          style={{ color: "var(--mantine-color-dimmed)" }}
        />
        <Box
          component="span"
          style={{
            flex: 1,
            fontSize: isLargeScreen ? rem(14) : rem(13),
            color: "var(--mantine-color-dimmed)",
            textAlign: "left",
          }}
        >
          Search...
        </Box>
        <Group gap={4}>
          <Kbd size={isLargeScreen ? "sm" : "xs"}>{isMac ? "⌘" : "Ctrl"}</Kbd>+
          <Kbd size={isLargeScreen ? "sm" : "xs"}>K</Kbd>
        </Group>
      </UnstyledButton>
    </Box>
  );
}

interface UserMenuProps {
  user: { name: string; email: string };
  onLogout: () => void;
  isLoggingOut: boolean;
}

function UserMenu({
  user,
  onLogout,
  isLoggingOut,
}: UserMenuProps) {
  return (
    <>
      <Menu position="bottom-end" offset={8} withArrow>
        <Menu.Target>
          <Avatar
            size="md"
            radius="xl"
            style={{ cursor: "pointer" }}
            alt={user.name}
          >
            {user.name.charAt(0).toUpperCase()}
          </Avatar>
        </Menu.Target>

        <Menu.Dropdown>
          <Menu.Label>
            <Box>
              <Box fw={500} fz="sm">
                {user.name}
              </Box>
              <Box fz="xs" c="dimmed">
                {user.email}
              </Box>
            </Box>
          </Menu.Label>

          <Menu.Divider />

          <Menu.Item
            leftSection={<User {...iconProps} />}
            onClick={() => {
              // TODO: Navigate to profile
            }}
          >
            Profile
          </Menu.Item>

          <Menu.Item
            leftSection={<Settings {...iconProps} />}
            onClick={() => {
              // TODO: Navigate to settings
            }}
          >
            Settings
          </Menu.Item>

          <Menu.Divider />

          <Menu.Item
            leftSection={<LogOut {...iconProps} />}
            color="red"
            onClick={onLogout}
            disabled={isLoggingOut}
          >
            Sign out
          </Menu.Item>
        </Menu.Dropdown>
      </Menu>
    </>
  );
}

export function AppHeader() {
  const { user, logout, isLoggingOut } = useAuth();
  const isLargeScreen = useMediaQuery("(min-width: 62em)");

  if (!user) return null;

  return (
    <Box
      component="header"
      style={{
        borderBottom: "1px solid var(--mantine-color-default-border)",
        backgroundColor: "var(--mantine-color-body)",
      }}
    >
      <Container size="xl" py="sm">
        <Group justify="space-between" wrap="nowrap">
          <HeaderLogo showCarDropdown={isLargeScreen} />

          {isLargeScreen && <HeaderSearch isLargeScreen={isLargeScreen} />}

          <Group gap="sm" wrap="nowrap">
            {!isLargeScreen && (
              <ActionIcon
                variant="default"
                size="lg"
                aria-label="Search"
                onClick={() => spotlight.open()}
              >
                <Search {...iconProps} />
              </ActionIcon>
            )}

            <ColorSchemeToggle />

            <UserMenu
              user={user}
              onLogout={logout}
              isLoggingOut={isLoggingOut}
            />
          </Group>
        </Group>
      </Container>
    </Box>
  );
}
