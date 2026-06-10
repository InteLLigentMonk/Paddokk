import { createFileRoute } from "@tanstack/react-router";
import { PrivacyPolicy } from "@/components/legal";

export const Route = createFileRoute("/_legal/privacy")({
  component: PrivacyPolicy,
});
