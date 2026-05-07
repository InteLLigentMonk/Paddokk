import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { useQueryClient } from "@tanstack/react-query";
import {
  requestPasswordReset as requestPasswordResetApi,
  resetPassword,
  signIn,
  signOut,
  signUp,
  useSession,
} from "@/lib/auth-client";
import { useNotifications } from "@/integrations/mantine";

/**
 * Custom hook for authentication operations
 *
 * Wraps Better Auth client methods with:
 * - Automatic error handling
 * - User notifications
 * - Navigation after success
 * - Loading state management
 */
export function useAuth() {
  const navigate = useNavigate();
  const notifications = useNotifications();
  const queryClient = useQueryClient();
  const { data: session, isPending: isSessionLoading } = useSession();

  const [isLoggingIn, setIsLoggingIn] = useState(false);
  const [isRegistering, setIsRegistering] = useState(false);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [isRequestingReset, setIsRequestingReset] = useState(false);
  const [isResettingPassword, setIsResettingPassword] = useState(false);

  /**
   * Login with email and password
   */
  const login = async (email: string, password: string) => {
    setIsLoggingIn(true);
    try {
      const result = await signIn.email({
        email,
        password,
      });

      if (result.error) {
        notifications.error({
          message: result.error.message || "Failed to sign in",
        });
        return { success: false, error: result.error };
      }

      notifications.success({
        message: "Welcome back!",
      });

      // Navigate to app dashboard
      navigate({ to: "/dashboard" });

      return { success: true };
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "An unexpected error occurred";
      notifications.error({ message });
      return { success: false, error };
    } finally {
      setIsLoggingIn(false);
    }
  };

  /**
   * Register a new user
   */
  const register = async (
    firstName: string,
    lastName: string | undefined,
    email: string,
    password: string,
  ) => {
    setIsRegistering(true);
    try {
      const fullName = lastName?.trim()
        ? `${firstName.trim()} ${lastName.trim()}`
        : firstName.trim();

      const result = await signUp.email({
        name: fullName,
        email,
        password,
      });

      if (result.error) {
        notifications.error({
          message: result.error.message || "Failed to create account",
        });
        return { success: false, error: result.error };
      }

      notifications.success({
        message:
          "Account created! Please check your email to verify your account.",
      });

      // Navigate to app dashboard
      navigate({ to: "/dashboard" });

      return { success: true };
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "An unexpected error occurred";
      notifications.error({ message });
      return { success: false, error };
    } finally {
      setIsRegistering(false);
    }
  };

  /**
   * Logout the current user
   */
  const logout = async () => {
    setIsLoggingOut(true);
    try {
      await signOut();

      notifications.success({
        message: "You've been signed out",
      });

      // Navigate first so authenticated routes unmount their queries before
      // we wipe the cache — otherwise the still-mounted observers would
      // immediately refetch without a session and trigger 401s on the API.
      await navigate({ to: "/" });
      queryClient.clear();

      return { success: true };
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "An unexpected error occurred";
      notifications.error({ message });
      return { success: false, error };
    } finally {
      setIsLoggingOut(false);
    }
  };

  /**
   * Request a password reset email
   */
  const requestPasswordReset = async (email: string) => {
    setIsRequestingReset(true);
    try {
      const result = await requestPasswordResetApi({
        email,
        redirectTo: "/reset-password",
      });

      if (result.error) {
        notifications.error({
          message: result.error.message || "Failed to send reset email",
        });
        return { success: false, error: result.error };
      }

      notifications.success({
        message: "Check your email for a password reset link",
      });

      return { success: true };
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "An unexpected error occurred";
      notifications.error({ message });
      return { success: false, error };
    } finally {
      setIsRequestingReset(false);
    }
  };

  /**
   * Reset password with token
   */
  const resetPasswordWithToken = async (token: string, password: string) => {
    setIsResettingPassword(true);
    try {
      const result = await resetPassword({
        newPassword: password,
        token,
      });

      if (result.error) {
        notifications.error({
          message: result.error.message || "Failed to reset password",
        });
        return { success: false, error: result.error };
      }

      notifications.success({
        message: "Password reset successfully! You can now sign in.",
      });

      // Navigate to login page
      navigate({ to: "/login" });

      return { success: true };
    } catch (error) {
      const message =
        error instanceof Error ? error.message : "An unexpected error occurred";
      notifications.error({ message });
      return { success: false, error };
    } finally {
      setIsResettingPassword(false);
    }
  };

  return {
    // Auth operations
    login,
    register,
    logout,
    requestPasswordReset,
    resetPasswordWithToken,

    // Session data
    session: session?.session ?? null,
    user: session?.user ?? null,

    // Derived state
    isAuthenticated: !!session?.user,

    // Loading states
    isLoading:
      isSessionLoading ||
      isLoggingIn ||
      isRegistering ||
      isLoggingOut ||
      isRequestingReset ||
      isResettingPassword,
    isLoggingIn,
    isRegistering,
    isLoggingOut,
    isRequestingReset,
    isResettingPassword,
  };
}
