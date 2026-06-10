import { Link } from "@tanstack/react-router";
import { Anchor, List, Text } from "@mantine/core";
import { LegalDocument, LegalSection } from "./legal-document";

/**
 * Draft Privacy Policy for the public beta. The text is a good-faith draft and
 * MUST be reviewed by a qualified lawyer before the beta opens to the public.
 */
export function PrivacyPolicy() {
  return (
    <LegalDocument
      title="Privacy Policy"
      intro="This policy explains what personal data Paddokk collects, why we collect it, how long we keep it, and the rights you have over it. Paddokk is operated from Sweden and processes personal data in accordance with the EU General Data Protection Regulation (GDPR)."
    >
      <LegalSection heading="Who we are">
        <Text>
          Paddokk (&quot;we&quot;, &quot;us&quot;) is the data controller for
          the personal data described in this policy. For any privacy questions,
          or to exercise the rights described below, contact us using the
          details in the Contact section.
        </Text>
      </LegalSection>

      <LegalSection heading="Personal data we collect">
        <Text>We collect the following categories of personal data:</Text>
        <List spacing="xs">
          <List.Item>
            <strong>Account data</strong> &mdash; the email address, username,
            and password hash you provide when you register, and any display
            name or avatar you add.
          </List.Item>
          <List.Item>
            <strong>Authentication data</strong> &mdash; when you sign in with
            Google or Facebook, we receive your name, email address, and a
            provider account identifier from that service.
          </List.Item>
          <List.Item>
            <strong>Content you create</strong> &mdash; cars, journeys, posts,
            comments, inventory items, marketplace listings, and any images or
            text you upload.
          </List.Item>
          <List.Item>
            <strong>Usage and technical data</strong> &mdash; IP address,
            browser type, device information, and interactions with the site,
            collected to operate and secure the service.
          </List.Item>
          <List.Item>
            <strong>Cookies and consent</strong> &mdash; a record of your cookie
            choices. You can review or change these at any time with the
            &quot;Manage cookies&quot; control in the site footer.
          </List.Item>
        </List>
      </LegalSection>

      <LegalSection heading="Legal basis for processing">
        <Text>We rely on the following legal bases under GDPR Article 6:</Text>
        <List spacing="xs">
          <List.Item>
            <strong>Contract</strong> &mdash; to create and operate your account
            and provide the features you use.
          </List.Item>
          <List.Item>
            <strong>Consent</strong> &mdash; for non-essential cookies and any
            optional communications. You may withdraw consent at any time.
          </List.Item>
          <List.Item>
            <strong>Legitimate interests</strong> &mdash; to keep the service
            secure, prevent abuse, and improve the platform, balanced against
            your rights.
          </List.Item>
          <List.Item>
            <strong>Legal obligation</strong> &mdash; where we must retain or
            disclose data to comply with applicable law.
          </List.Item>
        </List>
      </LegalSection>

      <LegalSection heading="How long we keep your data">
        <Text>
          We keep account data and the content you create for as long as your
          account is active. When you delete your account, we erase your
          profile, avatar, and associated personal data without undue delay,
          except where we are required to retain certain records to meet legal
          obligations or to resolve disputes. Backups are rotated on a limited
          schedule and purged in due course. Server logs containing technical
          data are retained only for a short period for security and
          troubleshooting.
        </Text>
      </LegalSection>

      <LegalSection heading="Your rights as a data subject">
        <Text>
          Under GDPR you have the following rights, which you can exercise free
          of charge by contacting us:
        </Text>
        <List spacing="xs">
          <List.Item>
            <strong>Access</strong> &mdash; obtain a copy of the personal data
            we hold about you.
          </List.Item>
          <List.Item>
            <strong>Rectification</strong> &mdash; correct inaccurate or
            incomplete data, much of which you can edit directly in your account
            settings.
          </List.Item>
          <List.Item>
            <strong>Erasure</strong> &mdash; delete your account and personal
            data (&quot;right to be forgotten&quot;).
          </List.Item>
          <List.Item>
            <strong>Portability</strong> &mdash; receive your data in a
            structured, machine-readable format.
          </List.Item>
          <List.Item>
            <strong>Objection and restriction</strong> &mdash; object to, or ask
            us to restrict, processing based on legitimate interests.
          </List.Item>
        </List>
        <Text>
          You also have the right to lodge a complaint with your supervisory
          authority. In Sweden this is the Swedish Authority for Privacy
          Protection (Integritetsskyddsmyndigheten, IMY).
        </Text>
      </LegalSection>

      <LegalSection heading="Sharing and international transfers">
        <Text>
          We share personal data only with service providers that help us run
          Paddokk &mdash; such as hosting, storage, and authentication providers
          &mdash; acting as our processors under contract. Where data is
          transferred outside the EU/EEA, we rely on appropriate safeguards such
          as the European Commission&apos;s Standard Contractual Clauses. We do
          not sell your personal data.
        </Text>
      </LegalSection>

      <LegalSection heading="Security">
        <Text>
          We use industry-standard measures to protect personal data, including
          encryption in transit, hashed passwords, and access controls. No
          method of transmission or storage is completely secure, but we work to
          protect your data and to notify you and the relevant authority of any
          breach as required by law.
        </Text>
      </LegalSection>

      <LegalSection heading="Children">
        <Text>
          Paddokk is not directed at children under the age of digital consent
          in their country of residence. We do not knowingly collect personal
          data from such children. If you believe a child has provided us with
          personal data, please contact us so we can remove it.
        </Text>
      </LegalSection>

      <LegalSection heading="Changes to this policy">
        <Text>
          We may update this policy from time to time. When we do, we update the
          &quot;Last updated&quot; date below and, where the change is material,
          we re-request your cookie consent on your next visit.
        </Text>
      </LegalSection>

      <LegalSection heading="Contact">
        <Text>
          For privacy questions or to exercise your rights, contact our data
          protection contact at{" "}
          <Anchor href="mailto:privacy@paddokk.com">privacy@paddokk.com</Anchor>
          . Your use of Paddokk is also governed by our{" "}
          <Anchor component={Link} to="/terms" inherit>
            Terms of Service
          </Anchor>
          .
        </Text>
      </LegalSection>
    </LegalDocument>
  );
}
