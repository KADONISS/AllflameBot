# Test Fixtures

Fixtures must be sanitized, versioned with their source assumptions and small
enough to review. Each fixture should state its area/client/DPB identity,
expected result and whether it represents a failure or success path.

Native UI fixtures should include the DPB build, native open/enabled state,
input mode, fresh postcondition and the matching stable Element counts when a
Voyage Planner comparison is involved. The current repository has no live
client fixture yet; runtime evidence remains an integration check.
