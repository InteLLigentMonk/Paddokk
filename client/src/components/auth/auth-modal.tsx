import { Modal } from "@mantine/core";
import { useStore } from "@tanstack/react-store";
import { LoginForm } from "./login-form";
import { SignupForm } from "./signup-form";
import { ForgotPasswordForm } from "./forgot-password-form";
import {
  authModalStore,
  closeAuthModal,
  switchAuthView,
} from "@/lib/stores/auth-modal-store";

/**
 * Auth modal component
 * Renders login, signup, or forgot password forms based on store state
 */
export function AuthModal() {
  const { isOpen, view } = useStore(authModalStore);

  return (
    <Modal
      opened={isOpen}
      onClose={closeAuthModal}
      centered
      size="md"
      padding="xl"
      overlayProps={{
        backgroundOpacity: 0.55,
        blur: 3,
      }}
    >
      {view === "login" && (
        <LoginForm
          mode="modal"
          onSwitchToSignup={() => switchAuthView("signup")}
          onSwitchToForgotPassword={() => switchAuthView("forgot-password")}
        />
      )}
      {view === "signup" && (
        <SignupForm
          mode="modal"
          onSwitchToLogin={() => switchAuthView("login")}
        />
      )}
      {view === "forgot-password" && (
        <ForgotPasswordForm
          mode="modal"
          onSwitchToLogin={() => switchAuthView("login")}
        />
      )}
    </Modal>
  );
}
