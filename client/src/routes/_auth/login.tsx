import { createFileRoute } from "@tanstack/react-router";
import { LoginForm } from "@/components/auth";

export const Route = createFileRoute("/_auth/login")({
  component: LoginPage,
});

function LoginPage() {
  return <LoginForm mode="page" />;
}
