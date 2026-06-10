import { Link } from "@tanstack/react-router";
import { Anchor, List, Text } from "@mantine/core";
import { LegalDocument, LegalSection } from "./legal-document";

/**
 * Draft Terms of Service for the public beta. The text is a good-faith draft
 * and MUST be reviewed by a qualified lawyer before the beta opens to the
 * public.
 */
export function TermsOfService() {
  return (
    <LegalDocument
      title="Terms of Service"
      intro="These terms govern your use of Paddokk. By creating an account or using the service, you agree to them. Please read them together with our Privacy Policy."
    >
      <LegalSection heading="Acceptance of these terms">
        <Text>
          By accessing or using Paddokk you agree to be bound by these Terms of
          Service and our{" "}
          <Anchor component={Link} to="/privacy" inherit>
            Privacy Policy
          </Anchor>
          . If you do not agree, do not use the service.
        </Text>
      </LegalSection>

      <LegalSection heading="Eligibility and accounts">
        <Text>
          You must be old enough to form a binding contract and to give valid
          consent for data processing in your country. You are responsible for
          the activity under your account and for keeping your credentials
          secure. Provide accurate registration information and keep it up to
          date.
        </Text>
      </LegalSection>

      <LegalSection heading="Acceptable use">
        <Text>When using Paddokk you agree not to:</Text>
        <List spacing="xs">
          <List.Item>
            break any applicable law or infringe the rights of others;
          </List.Item>
          <List.Item>
            post content that is unlawful, hateful, harassing, defamatory, or
            sexually explicit;
          </List.Item>
          <List.Item>
            upload malware, attempt to disrupt the service, or probe its
            security without authorisation;
          </List.Item>
          <List.Item>
            scrape, harvest, or bulk-export other users&apos; data, or use the
            service to send spam;
          </List.Item>
          <List.Item>
            impersonate others or misrepresent your affiliation with any person
            or entity.
          </List.Item>
        </List>
      </LegalSection>

      <LegalSection heading="Your content">
        <Text>
          You retain ownership of the content you create on Paddokk. You grant
          us a non-exclusive, worldwide, royalty-free licence to host, store,
          and display that content solely to operate and provide the service.
          You are responsible for your content and confirm you have the rights
          to share it. We may remove content that violates these terms.
        </Text>
      </LegalSection>

      <LegalSection heading="Marketplace and user interactions">
        <Text>
          Paddokk may let users list items or interact directly. We are not a
          party to transactions between users, do not verify listings, and are
          not responsible for the conduct of any user. Exercise normal caution
          when dealing with others.
        </Text>
      </LegalSection>

      <LegalSection heading="Account suspension and termination">
        <Text>
          We may suspend or terminate your access if you breach these terms,
          create risk or legal exposure for us, or for prolonged inactivity. You
          may stop using the service and delete your account at any time from
          your account settings. Provisions that by their nature should survive
          termination will do so.
        </Text>
      </LegalSection>

      <LegalSection heading="Service availability">
        <Text>
          Paddokk is provided during a public beta and is offered &quot;as
          is&quot; and &quot;as available&quot;. Features may change, be
          interrupted, or be discontinued. We do not guarantee that the service
          will be uninterrupted, error-free, or free of data loss.
        </Text>
      </LegalSection>

      <LegalSection heading="Limitation of liability">
        <Text>
          To the fullest extent permitted by law, Paddokk and its operators are
          not liable for any indirect, incidental, or consequential damages, or
          for loss of data, profits, or goodwill, arising from your use of the
          service. Nothing in these terms excludes liability that cannot be
          excluded under mandatory law, including under the laws of Sweden and
          the EU.
        </Text>
      </LegalSection>

      <LegalSection heading="Governing law and disputes">
        <Text>
          These terms are governed by the laws of Sweden, without regard to its
          conflict-of-laws rules, and subject to any mandatory consumer
          protections of your country of residence within the EU. Disputes are
          subject to the jurisdiction of the Swedish courts, unless mandatory
          law provides otherwise.
        </Text>
      </LegalSection>

      <LegalSection heading="Changes to these terms">
        <Text>
          We may update these terms from time to time. We update the &quot;Last
          updated&quot; date below when we do, and material changes may require
          you to acknowledge the updated terms before continuing to use the
          service.
        </Text>
      </LegalSection>

      <LegalSection heading="Contact">
        <Text>
          Questions about these terms? Contact us at{" "}
          <Anchor href="mailto:legal@paddokk.com">legal@paddokk.com</Anchor>.
        </Text>
      </LegalSection>
    </LegalDocument>
  );
}
