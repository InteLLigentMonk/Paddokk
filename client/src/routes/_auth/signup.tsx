import { createFileRoute } from "@tanstack/react-router";
import { SignupForm } from "@/components/auth";

export const Route = createFileRoute("/_auth/signup")({
  component: SignupPage,
});

function SignupPage() {
  return <SignupForm mode="page" />;
}
