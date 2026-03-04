/** @type {import('@commitlint/types').UserConfig} */
export default {
  extends: ["@commitlint/config-conventional"],
  rules: {
    "type-enum": [
      2,
      "always",
      [
        "feat",
        "fix",
        "docs",
        "style",
        "refactor",
        "perf",
        "test",
        "chore",
        "ci",
        "revert",
      ],
    ],
    "scope-enum": [
      1,
      "always",
      [
        "auth",
        "api",
        "ui",
        "journey",
        "car",
        "routing",
        "query",
        "form",
        "db",
        "landing",
        "deps",
        "release",
      ],
    ],
    "header-max-length": [2, "always", 100],
  },
};
