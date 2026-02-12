//  @ts-check

import { tanstackConfig } from "@tanstack/eslint-config";

export default [
  ...tanstackConfig,
  {
    ignores: [
      "src/generated/**/*",
      "*.config.js",
      "*.config.ts",
      ".output/**/*",
      "dist/**/*",
      "node_modules/**/*",
    ],
  },
];
