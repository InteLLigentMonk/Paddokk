import { createFileRoute } from "@tanstack/react-router";
import { TermsOfService } from "@/components/legal";

export const Route = createFileRoute("/_legal/terms")({
  component: TermsOfService,
});
