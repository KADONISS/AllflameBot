# Changelog

All notable changes to AllflameBot are documented in this file.

## 0.23.108 - 2026-08-03

- Use single-click movement near interaction targets instead of leaving
  `BeginUseAt` held, so the player stops inside the Allflame Capsule ring.
- Clear movement input before and after DPB action synchronization and accept
  the full configured Capsule interaction range.
- Bound zero-lantern Capsule rescue to three cycles and a 60-second deadline.
- Treat registry-only Capsule anchors as bounded live-object reacquisition targets.
- Prevent interaction stages from running without a valid live Capsule object.
- Track explicit convergence, live-target acquisition, UI progress, and route deltas.
- Stop with `CapsuleRescueExhausted` and `RecoveryStop` after bounded recovery fails.
- Establish the standalone AllflameBot source repository baseline.
