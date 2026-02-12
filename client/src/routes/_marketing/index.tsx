import { createFileRoute } from "@tanstack/react-router";
import {
  CtaSection,
  FeaturesSection,
  HeroSection,
  PricingSection,
  RoadmapSection,
} from "@/components/marketing";

export const Route = createFileRoute("/_marketing/")({
  component: LandingPage,
});

function LandingPage() {
  return (
    <>
      <HeroSection />
      <FeaturesSection />
      <PricingSection />
      <RoadmapSection />
      <CtaSection />
    </>
  );
}
