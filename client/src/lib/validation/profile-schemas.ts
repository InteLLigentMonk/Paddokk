import { z } from "zod";

export const updateProfileSchema = z.object({
  firstName: z
    .string()
    .min(1, "First name is required")
    .max(50, "First name cannot exceed 50 characters"),
  lastName: z.string().max(50, "Last name cannot exceed 50 characters"),
  displayName: z
    .string()
    .min(1, "Display name is required")
    .max(100, "Display name cannot exceed 100 characters"),
  bio: z.string().max(500, "Bio cannot exceed 500 characters"),
});

export type UpdateProfileFormData = z.infer<typeof updateProfileSchema>;

export const changeUsernameSchema = z.object({
  username: z
    .string()
    .min(2, "Username must be at least 2 characters")
    .max(30, "Username cannot exceed 30 characters")
    .regex(
      /^[a-z0-9._-]+$/,
      "Use lowercase letters, digits, dot, underscore or hyphen only",
    ),
});

export type ChangeUsernameFormData = z.infer<typeof changeUsernameSchema>;

export const changePasswordSchema = z
  .object({
    currentPassword: z.string().min(1, "Current password is required"),
    newPassword: z
      .string()
      .min(8, "Password must be at least 8 characters")
      .regex(
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/,
        "Password must contain at least one uppercase letter, one lowercase letter, and one number",
      ),
    confirmPassword: z.string(),
    revokeOtherSessions: z.boolean(),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
  });

export type ChangePasswordFormData = z.infer<typeof changePasswordSchema>;

export const changeEmailSchema = z.object({
  newEmail: z.email("Please enter a valid email address"),
});

export type ChangeEmailFormData = z.infer<typeof changeEmailSchema>;

export const deleteAccountSchema = z.object({
  confirmation: z.literal("DELETE", {
    message: "Type DELETE to confirm",
  }),
  password: z.string().min(1, "Password is required"),
});

export type DeleteAccountFormData = z.infer<typeof deleteAccountSchema>;
