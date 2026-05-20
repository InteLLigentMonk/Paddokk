# Changelog

## [0.23.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.22.0...v0.23.0) (2026-05-20)


### Features

* **car:** per-section inline editing on car detail page ([#110](https://github.com/InteLLigentMonk/Paddokk/issues/110)) ([73666bc](https://github.com/InteLLigentMonk/Paddokk/commit/73666bc21e8b8bbc5b8cccc8643e091e821b3d6d))

## [0.22.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.21.2...v0.22.0) (2026-05-20)


### Features

* **car:** rebuild detail page hero + spec strip + sidebar ([#108](https://github.com/InteLLigentMonk/Paddokk/issues/108)) ([249a8fc](https://github.com/InteLLigentMonk/Paddokk/commit/249a8fc3b7407b46e3a5ec0b60b1c79964c12987))

## [0.21.2](https://github.com/InteLLigentMonk/Paddokk/compare/v0.21.1...v0.21.2) (2026-05-20)


### Maintenance

* **car:** add spec/vital fields + GetCarJourneys endpoint ([#106](https://github.com/InteLLigentMonk/Paddokk/issues/106)) ([c074ac7](https://github.com/InteLLigentMonk/Paddokk/commit/c074ac714ecf1a356312ac028af33d983c8c98e2))

## [0.21.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.21.0...v0.21.1) (2026-05-20)


### Bug Fixes

* **auth:** clear session cache before redirect to ensure auth state updates ([#104](https://github.com/InteLLigentMonk/Paddokk/issues/104)) ([419059b](https://github.com/InteLLigentMonk/Paddokk/commit/419059b4755661b0692ec0a886233c6b3ff9abcb))

## [0.21.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.20.0...v0.21.0) (2026-05-19)


### Features

* **car:** public browse page with trigram fuzzy search ([#102](https://github.com/InteLLigentMonk/Paddokk/issues/102)) ([a7bac7b](https://github.com/InteLLigentMonk/Paddokk/commit/a7bac7bd11be04f8ee8bb04cfdc0917dab411f4f))

## [0.20.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.19.1...v0.20.0) (2026-05-19)


### Features

* **journeys:** link author and car avatars to their pages ([#99](https://github.com/InteLLigentMonk/Paddokk/issues/99)) ([06e8447](https://github.com/InteLLigentMonk/Paddokk/commit/06e844765da45617fe4598970b6d262207a19ef7))

## [0.19.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.19.0...v0.19.1) (2026-05-19)


### Refactoring

* consolidate route loaders and query caching with shared queryOptions ([#97](https://github.com/InteLLigentMonk/Paddokk/issues/97)) ([baf2dd4](https://github.com/InteLLigentMonk/Paddokk/commit/baf2dd478c11276e290df164ee0805458a49a9c5))

## [0.19.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.18.2...v0.19.0) (2026-05-08)


### Features

* **auth:** implement user identity with slug-based URLs and privacy controls ([#95](https://github.com/InteLLigentMonk/Paddokk/issues/95)) ([3d5a0f6](https://github.com/InteLLigentMonk/Paddokk/commit/3d5a0f6f89858c5c483d8d8316640265a9c022cb))

## [0.18.2](https://github.com/InteLLigentMonk/Paddokk/compare/v0.18.1...v0.18.2) (2026-05-07)


### Refactoring

* rename ownership FKs to PrincipalId and authorship FKs to AuthorId ([#93](https://github.com/InteLLigentMonk/Paddokk/issues/93)) ([345f2c9](https://github.com/InteLLigentMonk/Paddokk/commit/345f2c9b29a7c1d61ce39e6d6012ba84ddbd2e5f))

## [0.18.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.18.0...v0.18.1) (2026-05-06)


### Bug Fixes

* **journey:** display posts in descending order and restore create post bar ([#91](https://github.com/InteLLigentMonk/Paddokk/issues/91)) ([9c1303c](https://github.com/InteLLigentMonk/Paddokk/commit/9c1303c0e289800857e8bcb73e677f4c94eceecb))

## [0.18.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.17.3...v0.18.0) (2026-05-06)


### Features

* **journey:** add create and manage journey post functionality ([#87](https://github.com/InteLLigentMonk/Paddokk/issues/87)) ([0c88ec4](https://github.com/InteLLigentMonk/Paddokk/commit/0c88ec4aa40f2c9b35e96a81afc4c9bbcc8a6076))

## [0.17.3](https://github.com/InteLLigentMonk/Paddokk/compare/v0.17.2...v0.17.3) (2026-05-06)


### Bug Fixes

* **security:** sanitize HTML fields in command handlers to prevent XSS ([#85](https://github.com/InteLLigentMonk/Paddokk/issues/85)) ([cd146d6](https://github.com/InteLLigentMonk/Paddokk/commit/cd146d6cb9c7a44fe39ff9b02240d6f4c420da23))

## [0.17.2](https://github.com/InteLLigentMonk/Paddokk/compare/v0.17.1...v0.17.2) (2026-05-06)


### Refactoring

* **journey:** replace status model with activity tiers and IsPublic toggle ([#83](https://github.com/InteLLigentMonk/Paddokk/issues/83)) ([4b6221e](https://github.com/InteLLigentMonk/Paddokk/commit/4b6221ef66a9f81942e6fbb9f96b002ee3914bf4))

## [0.17.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.17.0...v0.17.1) (2026-05-06)


### Maintenance

* **ui:** remove unused imports and variables to clear TypeScript warnings ([#81](https://github.com/InteLLigentMonk/Paddokk/issues/81)) ([6c17d5d](https://github.com/InteLLigentMonk/Paddokk/commit/6c17d5d65755a90ea82fe9ac501314f2bdadd3f9))

## [0.17.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.16.0...v0.17.0) (2026-05-06)


### Features

* **journeys:** auto-reassign active journey on status change or delete ([#79](https://github.com/InteLLigentMonk/Paddokk/issues/79)) ([770665d](https://github.com/InteLLigentMonk/Paddokk/commit/770665d47c7b869260f5741f6bc6012dfa9ae13c))

## [0.16.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.15.2...v0.16.0) (2026-05-06)


### Features

* **journey:** implement edit journey modal with pre-filled form ([#77](https://github.com/InteLLigentMonk/Paddokk/issues/77)) ([ec21f21](https://github.com/InteLLigentMonk/Paddokk/commit/ec21f211b52c3dcb4a13021475731fed0cd0e493))

## [0.15.2](https://github.com/InteLLigentMonk/Paddokk/compare/v0.15.1...v0.15.2) (2026-05-06)


### Bug Fixes

* **ui:** use aspect ratio for responsive card and detail images ([#75](https://github.com/InteLLigentMonk/Paddokk/issues/75)) ([3cc0320](https://github.com/InteLLigentMonk/Paddokk/commit/3cc03205a1aaf0f8f823e4623531ac607e679e11))

## [0.15.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.15.0...v0.15.1) (2026-05-06)


### Bug Fixes

* **cars:** add mobile drag-and-drop support for image reordering ([#73](https://github.com/InteLLigentMonk/Paddokk/issues/73)) ([99a0e91](https://github.com/InteLLigentMonk/Paddokk/commit/99a0e910d3ed40701eb3c07db3adf3ee43bbde7e))

## [0.15.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.14.2...v0.15.0) (2026-05-06)


### Features

* **comments:** add 1-deep reply support, delete, and UI improvements ([#70](https://github.com/InteLLigentMonk/Paddokk/issues/70)) ([b64fdcb](https://github.com/InteLLigentMonk/Paddokk/commit/b64fdcb081305716323874dd98c9ad5547f6b546))

## [0.14.2](https://github.com/InteLLigentMonk/Paddokk/compare/v0.14.1...v0.14.2) (2026-05-05)


### Bug Fixes

* **ui:** rewrite ExpandableText with CSS line-clamp and ResizeObserver ([#68](https://github.com/InteLLigentMonk/Paddokk/issues/68)) ([92b0080](https://github.com/InteLLigentMonk/Paddokk/commit/92b0080daeacf875ef898b5c45a51bf13a74b591))

## [0.14.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.14.0...v0.14.1) (2026-05-05)


### Bug Fixes

* **car:** reassign active car when primary car is deleted ([#66](https://github.com/InteLLigentMonk/Paddokk/issues/66)) ([29e6da1](https://github.com/InteLLigentMonk/Paddokk/commit/29e6da1a0fd6f5a0b48b649941b2b9cbf876b37d))

## [0.14.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.13.0...v0.14.0) (2026-05-05)


### Features

* **car:** add active car badge and set as active action ([#64](https://github.com/InteLLigentMonk/Paddokk/issues/64)) ([a37afc3](https://github.com/InteLLigentMonk/Paddokk/commit/a37afc323a4574951d750b2bc69e993846ae780e))

## [0.13.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.12.0...v0.13.0) (2026-05-04)


### Features

* **routing:** replace back buttons with automatic breadcrumbs ([#62](https://github.com/InteLLigentMonk/Paddokk/issues/62)) ([c760b60](https://github.com/InteLLigentMonk/Paddokk/commit/c760b60a0c2be007b85f9936c76aa5f915c9966b))

## [0.12.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.11.3...v0.12.0) (2026-05-04)


### Features

* **journey:** mount create journey modal globally and navigate to detail on creation ([#60](https://github.com/InteLLigentMonk/Paddokk/issues/60)) ([73034c3](https://github.com/InteLLigentMonk/Paddokk/commit/73034c318879d3122ea510dca6a02d34f6a8b5c3))

## [0.11.3](https://github.com/InteLLigentMonk/Paddokk/compare/v0.11.2...v0.11.3) (2026-05-04)


### Refactoring

* **ui:** restructure navigation into Me / Discover / Tools groups ([#58](https://github.com/InteLLigentMonk/Paddokk/issues/58)) ([98a8d1a](https://github.com/InteLLigentMonk/Paddokk/commit/98a8d1adb625f71fb41179425bd776b2c8d45325))

## [0.11.2](https://github.com/InteLLigentMonk/Paddokk/compare/v0.11.1...v0.11.2) (2026-04-29)


### Refactoring

* **routes:** move public detail pages under /journeys and /cars ([#56](https://github.com/InteLLigentMonk/Paddokk/issues/56)) ([b902453](https://github.com/InteLLigentMonk/Paddokk/commit/b902453dd8cdce147f7bf05e82e1c426f0565e14))

## [0.11.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.11.0...v0.11.1) (2026-04-29)


### Bug Fixes

* ui tweaks for dark mode, nav active state, and theme ([#54](https://github.com/InteLLigentMonk/Paddokk/issues/54)) ([afab34d](https://github.com/InteLLigentMonk/Paddokk/commit/afab34d31efd64f52efd8f38b45ced1158e50b6a))

## [0.11.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.10.0...v0.11.0) (2026-04-29)


### Features

* **journey:** journey detail view with infinite scroll ([#52](https://github.com/InteLLigentMonk/Paddokk/issues/52)) ([6e86744](https://github.com/InteLLigentMonk/Paddokk/commit/6e867442dc384dde4335f0e72313ff44dec33d07))

## [0.10.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.9.0...v0.10.0) (2026-04-29)


### Features

* **journey:** add journey detail view with posts, comments, and image lightbox ([#50](https://github.com/InteLLigentMonk/Paddokk/issues/50)) ([a5e08ab](https://github.com/InteLLigentMonk/Paddokk/commit/a5e08ab1147ee9f0388fbc1ad8c3918c32b32f96))

## [0.9.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.8.2...v0.9.0) (2026-04-29)


### Features

* **journey:** add /me/journeys summary page ([#48](https://github.com/InteLLigentMonk/Paddokk/issues/48)) ([f0b3018](https://github.com/InteLLigentMonk/Paddokk/commit/f0b3018575a5059dd8864bcb1ead34fd522a6a85))

## [0.8.2](https://github.com/InteLLigentMonk/Paddokk/compare/v0.8.1...v0.8.2) (2026-04-22)


### Bug Fixes

* **release:** add checkout step before auto-merge ([#46](https://github.com/InteLLigentMonk/Paddokk/issues/46)) ([0ee01d1](https://github.com/InteLLigentMonk/Paddokk/commit/0ee01d1df435e31a16e3a28b007e4b8b8335253c))

## [0.8.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.8.0...v0.8.1) (2026-04-22)


### Refactoring

* **routing:** move /cars to /me/cars, add public /cars/:id view ([#44](https://github.com/InteLLigentMonk/Paddokk/issues/44)) ([da71e8d](https://github.com/InteLLigentMonk/Paddokk/commit/da71e8dc508a6de49be1459d82ba21adcb7d3247))

## [0.8.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.7.0...v0.8.0) (2026-04-22)


### Features

* **dev:** add Bogus seed data for development ([#42](https://github.com/InteLLigentMonk/Paddokk/issues/42)) ([fd2fee0](https://github.com/InteLLigentMonk/Paddokk/commit/fd2fee043e2cf5734650cfce46a1178219a6f19a))


### Bug Fixes

* **ci:** release-please fixes and cars custom build feature ([#41](https://github.com/InteLLigentMonk/Paddokk/issues/41)) ([fd5d3c9](https://github.com/InteLLigentMonk/Paddokk/commit/fd5d3c93ad09c2807fab1d55c62df61e4b92eb45))

## [0.7.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.6.0...v0.7.0) (2026-03-09)


### Features

* **cars:** add like and subscribe engagement to cars ([#38](https://github.com/InteLLigentMonk/Paddokk/issues/38)) ([210d655](https://github.com/InteLLigentMonk/Paddokk/commit/210d655f485b4b5ca27615fe160349c5e1a8564b))


### Bug Fixes

* **ci:** use pr_number output from release-please action ([#40](https://github.com/InteLLigentMonk/Paddokk/issues/40)) ([b0f14fb](https://github.com/InteLLigentMonk/Paddokk/commit/b0f14fbd8a6e2b649bd8fb61ea0bd646afc062df))

## [0.6.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.5.1...v0.6.0) (2026-03-09)


### Features

* **cars:** navigate to detail page with edit mode from car card ([#36](https://github.com/InteLLigentMonk/Paddokk/issues/36)) ([8518373](https://github.com/InteLLigentMonk/Paddokk/commit/851837339c7b0f97135e6e57769608346d6b4fa0))

## [0.5.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.5.0...v0.5.1) (2026-03-07)


### Bug Fixes

* **car:** sync image state after edit to show updated photos immediately ([#33](https://github.com/InteLLigentMonk/Paddokk/issues/33)) ([903a10b](https://github.com/InteLLigentMonk/Paddokk/commit/903a10b534f492913e28d8396f0fb5380aba6e41))

## [0.5.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.4.0...v0.5.0) (2026-03-07)


### Features

* **layout:** sticky header and constrain scroll above mobile bottom nav ([#31](https://github.com/InteLLigentMonk/Paddokk/issues/31)) ([cb72709](https://github.com/InteLLigentMonk/Paddokk/commit/cb72709360d2e0c04a71a196ff65a6d59737e9fc))

## [0.4.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.3.1...v0.4.0) (2026-03-07)


### Features

* **car:** open add car modal from FAB menu ([#29](https://github.com/InteLLigentMonk/Paddokk/issues/29)) ([8eca04e](https://github.com/InteLLigentMonk/Paddokk/commit/8eca04eecf4ce8697288a267f96da6055fab864c))

## [0.3.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.3.0...v0.3.1) (2026-03-06)


### Maintenance

* add trigger definitions to skill files ([#27](https://github.com/InteLLigentMonk/Paddokk/issues/27)) ([d092035](https://github.com/InteLLigentMonk/Paddokk/commit/d09203550d497ec8f1d1352d59bb394fbe675919))

## [0.3.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.2.3...v0.3.0) (2026-03-06)


### Features

* **cars:** add car detail page with inline editing, WYSIWYG specs, and deferred API calls ([#25](https://github.com/InteLLigentMonk/Paddokk/issues/25)) ([e939f14](https://github.com/InteLLigentMonk/Paddokk/commit/e939f147b432808560f3790dfeb87fe1dd407717))

## [0.2.3](https://github.com/InteLLigentMonk/Paddokk/compare/v0.2.2...v0.2.3) (2026-03-06)


### Refactoring

* **cars:** defer all API calls in add-car flow to Finish button ([#22](https://github.com/InteLLigentMonk/Paddokk/issues/22)) ([8257fa3](https://github.com/InteLLigentMonk/Paddokk/commit/8257fa30415532e5e5ff3973a093aa0600326de7))

## [0.2.2](https://github.com/InteLLigentMonk/Paddokk/compare/v0.2.1...v0.2.2) (2026-03-06)


### Refactoring

* resolve subscription tier from user entity in handlers ([#20](https://github.com/InteLLigentMonk/Paddokk/issues/20)) ([a9318b7](https://github.com/InteLLigentMonk/Paddokk/commit/a9318b7fbb343087f740cc761916f269c3e3ddb0))

## [0.2.1](https://github.com/InteLLigentMonk/Paddokk/compare/v0.2.0...v0.2.1) (2026-03-06)


### Maintenance

* **claude:** update skills, rules, and CLAUDE.md configuration ([#17](https://github.com/InteLLigentMonk/Paddokk/issues/17)) ([270dc64](https://github.com/InteLLigentMonk/Paddokk/commit/270dc6498e5477e8fe4feef60563bdafc3ed751e))
* **db:** migrate from SQL Server to PostgreSQL and refactor data layer ([#19](https://github.com/InteLLigentMonk/Paddokk/issues/19)) ([b1972c2](https://github.com/InteLLigentMonk/Paddokk/commit/b1972c2a4e1b4add4ca423300acd366f930d334d))

## [0.2.0](https://github.com/InteLLigentMonk/Paddokk/compare/v0.1.0...v0.2.0) (2026-03-04)


### Features

* **auth:** add EdDSA JWKS async key loader for JWT validation ([5b702ed](https://github.com/InteLLigentMonk/Paddokk/commit/5b702ed47fc4a514cfdb1bbd7931cfc9fe8ef231))
* **auth:** add password reset and email verification flows ([441008b](https://github.com/InteLLigentMonk/Paddokk/commit/441008bf40ab21c0a5227385719f4b4a5ace2e61))
* **auth:** add password reset and email verification flows ([f5706b1](https://github.com/InteLLigentMonk/Paddokk/commit/f5706b13601016c4526b0e1555e82fa2ad36c5fd))
* **car:** add car images feature with CQRS handlers and UI improvements ([#13](https://github.com/InteLLigentMonk/Paddokk/issues/13)) ([8b4082c](https://github.com/InteLLigentMonk/Paddokk/commit/8b4082c4a3ea9984cf563a7a98b8e4d0e51ff3a9))
* **cars:** implement comprehensive car management system ([eedc018](https://github.com/InteLLigentMonk/Paddokk/commit/eedc0180b5096964bacbec51d2b23213c4ca0a09))
* **docs:** add documentation agent and git workflow guide ([2dfe26a](https://github.com/InteLLigentMonk/Paddokk/commit/2dfe26a0cd32b07ad1cc6d579dd62a663fc7bdc6))
* **docs:** add documentation agent and git workflow guide ([44487e2](https://github.com/InteLLigentMonk/Paddokk/commit/44487e236c86b165905cc41c0ddaf89a33d7ef87))
* **ui:** add mobile-first app header with spotlight search ([60dd1c7](https://github.com/InteLLigentMonk/Paddokk/commit/60dd1c7e466da1110fd7f2b5b4e81d465127312d))
* **ui:** add mobile-first app header with spotlight search ([25b321a](https://github.com/InteLLigentMonk/Paddokk/commit/25b321a73f34296e33387ecb75cae0270238985b))
* **ui:** implement comprehensive navigation system ([#10](https://github.com/InteLLigentMonk/Paddokk/issues/10)) ([0372482](https://github.com/InteLLigentMonk/Paddokk/commit/0372482dce7f101eaf96bafaf4a4cfc46059c26b))


### Bug Fixes

* **ui:** resolve all pre-existing TypeScript errors ([0e256b2](https://github.com/InteLLigentMonk/Paddokk/commit/0e256b2aaac44f9d245fcf00a19b9f40bef8d018))


### Refactoring

* **api:** break out comments to repository ([df54357](https://github.com/InteLLigentMonk/Paddokk/commit/df54357abb9a0c923377eaa0dfe2bc2e54c76498))
* **api:** break out data layer and start repository pattern on cars ([c47b79a](https://github.com/InteLLigentMonk/Paddokk/commit/c47b79a4bec5783f720268ec0eae98920cea72a4))
* **api:** break out repositorys (images) from service-layer, ongoing ([3f522d2](https://github.com/InteLLigentMonk/Paddokk/commit/3f522d2439304f33872b129c07d7dc9b815d2e89))
* **api:** break out repositorys (journeys) from service-layer, ongoing ([017e978](https://github.com/InteLLigentMonk/Paddokk/commit/017e978d0ae9932bd7355e488ba828412fac1a83))
* **api:** break out repositorys from service layer, ongoing ([f13c022](https://github.com/InteLLigentMonk/Paddokk/commit/f13c022428a6b95d73a10c9de1e7ff9b39d7d18a))
* **api:** break out repositorys from service-layer, finished ([2f1e554](https://github.com/InteLLigentMonk/Paddokk/commit/2f1e554800fef27acd1129819afd2bf827be49ea))
* **api:** clean up image controllers ([dec794b](https://github.com/InteLLigentMonk/Paddokk/commit/dec794b975312de0c54df145d3ee954771d24878))
* **api:** create ImageRepository, ongoing ([00b5cab](https://github.com/InteLLigentMonk/Paddokk/commit/00b5cab68066793e176a08f1e2cb5c05717654a5))
* **api:** fix method calls on EmailService ([b297de4](https://github.com/InteLLigentMonk/Paddokk/commit/b297de44a490e09b5f3ec7d608f27b19f677103c))
* **api:** make cleaner controllers ([3c306ab](https://github.com/InteLLigentMonk/Paddokk/commit/3c306ab7c20494bda0ed62328478fd8659e0ace9))
* **api:** migrate to CQRS with MediatR, add required to all DTOs ([f56c7b7](https://github.com/InteLLigentMonk/Paddokk/commit/f56c7b7f305483f065555fbefaa9360f50d3d019))
* **api:** migrate to OpenAPI transformer pattern with Scalar UI ([dec794b](https://github.com/InteLLigentMonk/Paddokk/commit/dec794b975312de0c54df145d3ee954771d24878))
* **api:** move logic from CarImagesController to service ([e3bfc9a](https://github.com/InteLLigentMonk/Paddokk/commit/e3bfc9ad72c7a698cb363858f019e21e2aca4556))
* **api:** move logic from CarImagesController to service, finished ([dd732c6](https://github.com/InteLLigentMonk/Paddokk/commit/dd732c6954610ac756657686cf6aae4e96fb03ab))
* **api:** rename project from API to Paddokk.Api ([def0408](https://github.com/InteLLigentMonk/Paddokk/commit/def0408d1ce758fcd00b6942adb5c7c68a62def0))
* **api:** reorganize API folder structure ([a25ec8f](https://github.com/InteLLigentMonk/Paddokk/commit/a25ec8f53eae2f508b70da444d14e08c84b77054))
* **api:** replace axios with plain fetch mutator ([71ab86d](https://github.com/InteLLigentMonk/Paddokk/commit/71ab86ddea74420bba481cb68007ff7548de8dbd))
* **api:** replace axios with plain fetch mutator ([13c53b4](https://github.com/InteLLigentMonk/Paddokk/commit/13c53b40d3ab4b7841fc1436b90bc66280405514))
* **api:** split api into class librarys, added core ([8e0959f](https://github.com/InteLLigentMonk/Paddokk/commit/8e0959f1b6aa53f50f224485aa25aa9a249e1175))
* **api:** standardize controllers to consistent pattern ([166780a](https://github.com/InteLLigentMonk/Paddokk/commit/166780affbd8c9c318eb10ea914c7e342e3ac5b8))
* **client:** migrate from Orval hooks to plain fetch functions ([2aa530a](https://github.com/InteLLigentMonk/Paddokk/commit/2aa530a8fd971e9797e06efcaf3f17dfadd197b7))
* **controllers:** ditch ActionResult in controllers and use throw instead, ongoing ([e03999c](https://github.com/InteLLigentMonk/Paddokk/commit/e03999cd7525cb2cd269cb8b968c9f9c03f056b2))


### Maintenance

* add root-level node_modules to gitignore ([15e0f8c](https://github.com/InteLLigentMonk/Paddokk/commit/15e0f8c3756ae62f2e8742b0433c0cda7c9194d8))
* **api:** migrate to net10 ([a706460](https://github.com/InteLLigentMonk/Paddokk/commit/a7064604872b280be63b7600ce00b98a63a23c1b))
* **deps:** clean up csproj files ([8e8976c](https://github.com/InteLLigentMonk/Paddokk/commit/8e8976cc8594ac9f99472b6bb598b8056a808d4e))
* **deps:** migrate from npm to pnpm ([#14](https://github.com/InteLLigentMonk/Paddokk/issues/14)) ([ffa33ad](https://github.com/InteLLigentMonk/Paddokk/commit/ffa33add49d9358f3c92884164354a9cd554db07))
* **deps:** remove Swashbuckle.AspNetCore from Paddokk.Core ([dc18a35](https://github.com/InteLLigentMonk/Paddokk/commit/dc18a35efac5f6ff0e1308c1e67fee68b1caff83))
* **docs:** reorganize Claude config to root for monorepo support ([dda647a](https://github.com/InteLLigentMonk/Paddokk/commit/dda647ae074517099de1699dba760f208c4a5f1e))
* **docs:** reorganize Claude config to root for monorepo support ([6c73ae9](https://github.com/InteLLigentMonk/Paddokk/commit/6c73ae9dec18fd861e7c669378e9a1e0b279ecca))
* **release:** migrate from commit-and-tag-version to release-please ([#15](https://github.com/InteLLigentMonk/Paddokk/issues/15)) ([59d77c7](https://github.com/InteLLigentMonk/Paddokk/commit/59d77c78b9359b502f433ca9a43ea457dc4b60da))
* remove accidentally committed .Backup.tmp, add to gitignore ([3f15cc7](https://github.com/InteLLigentMonk/Paddokk/commit/3f15cc7beca5c92d6b9484dcfed56e7cf2cb3286))
