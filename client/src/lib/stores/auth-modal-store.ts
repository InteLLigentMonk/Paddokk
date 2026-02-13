import { Store } from "@tanstack/store";

/**
 * Auth modal view types
 */
export type AuthModalView = "login" | "signup" | "forgot-password";

/**
 * Auth modal store state
 */
interface AuthModalState {
  isOpen: boolean;
  view: AuthModalView;
}

/**
 * TanStack Store for managing auth modal state
 */
export const authModalStore = new Store<AuthModalState>({
  isOpen: false,
  view: "login",
});

/**
 * Open the auth modal with the login form
 */
export function openLogin() {
  authModalStore.setState((state) => ({
    ...state,
    isOpen: true,
    view: "login",
  }));
}

/**
 * Open the auth modal with the signup form
 */
export function openSignup() {
  authModalStore.setState((state) => ({
    ...state,
    isOpen: true,
    view: "signup",
  }));
}

/**
 * Open the auth modal with the forgot password form
 */
export function openForgotPassword() {
  authModalStore.setState((state) => ({
    ...state,
    isOpen: true,
    view: "forgot-password",
  }));
}

/**
 * Close the auth modal
 */
export function closeAuthModal() {
  authModalStore.setState((state) => ({
    ...state,
    isOpen: false,
  }));
}

/**
 * Switch to a different auth view
 */
export function switchAuthView(view: AuthModalView) {
  authModalStore.setState((state) => ({
    ...state,
    view,
  }));
}
