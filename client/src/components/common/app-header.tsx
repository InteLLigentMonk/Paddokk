import {
  ActionIcon,
  Avatar,
  Box,
  Group,
  Kbd,
  Menu,
  Title,
  UnstyledButton,
  rem,
} from "@mantine/core";
import { spotlight } from "@mantine/spotlight";
import { useEffect, useState } from "react";
import { Link, getRouteApi } from "@tanstack/react-router";
import { useSuspenseQuery } from "@tanstack/react-query";
import { LogOut, Search, Settings, User } from "lucide-react";
import { ColorSchemeToggle } from "./color-scheme-toggle";
import { useAuth } from "@/hooks/use-auth";
import { currentUserQueryOptions } from "@/lib/api/users.queries";
import { userProfileUrl } from "@/lib/urls";

const appRouteApi = getRouteApi("/_app");

const iconProps = { size: 18, strokeWidth: 1.5 } as const;

function HeaderLogo() {
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
      <Title order={1} fz={{ base: "1rem", sm: "1.25rem" }}>
        Paddokk
      </Title>
    </Group>
  );
}

function HeaderSearch() {
  const [isMac, setIsMac] = useState(false);

  useEffect(() => {
    setIsMac(/Mac|iPhone|iPad|iPod/.test(navigator.userAgent));
  }, []);

  return (
    <Box style={{ flex: 1, maxWidth: rem(500), marginInline: "auto" }}>
      <UnstyledButton
        onClick={() => spotlight.open()}
        style={{
          width: "100%",
          height: rem(36),
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
            fontSize: rem(14),
            color: "var(--mantine-color-dimmed)",
            textAlign: "left",
          }}
        >
          Search...
        </Box>
        <Group gap={4}>
          <Kbd size="sm">{isMac ? "⌘" : "Ctrl"}</Kbd>+<Kbd size="sm">K</Kbd>
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

function UserMenu({ user, onLogout, isLoggingOut }: UserMenuProps) {
  const { data: currentUser } = useSuspenseQuery(currentUserQueryOptions());

  return (
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
          component={Link}
          to={userProfileUrl(currentUser.username)}
          leftSection={<User {...iconProps} />}
        >
          Profile
        </Menu.Item>

        <Menu.Item
          component={Link}
          to="/me/settings"
          leftSection={<Settings {...iconProps} />}
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
  );
}

export function AppHeader() {
  const { auth } = appRouteApi.useRouteContext();
  const { logout, isLoggingOut } = useAuth();

  if (!auth.user) return null;

  return (
    <Box
      component="header"
      p="sm"
      style={{
        position: "sticky",
        top: 0,
        zIndex: 101,
        borderBottom: "1px solid var(--mantine-color-default-border)",
        backgroundColor: "var(--mantine-color-body)",
      }}
    >
      <Group justify="space-between" wrap="nowrap">
        <HeaderLogo />

        <Box visibleFrom="md" style={{ flex: 1 }}>
          <HeaderSearch />
        </Box>

        <Group gap="sm" wrap="nowrap">
          <Box hiddenFrom="md">
            <ActionIcon
              variant="default"
              size="lg"
              aria-label="Search"
              onClick={() => spotlight.open()}
            >
              <Search {...iconProps} />
            </ActionIcon>
          </Box>

          <ColorSchemeToggle />

          <UserMenu
            user={auth.user}
            onLogout={logout}
            isLoggingOut={isLoggingOut}
          />
        </Group>
      </Group>
    </Box>
  );
}
