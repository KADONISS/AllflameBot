# Bug Investigation Checklist

- [ ] Capture the exact log interval, area hash, phase and active task.
- [ ] Separate observation failure, route failure, input failure and UI failure.
- [ ] Reconstruct the task ownership sequence.
- [ ] Compare state snapshots before and after the failed action.
- [ ] Check all recovery and stop call sites before changing behavior.
- [ ] Identify whether the failure is reproducible from a fixture.
- [ ] Add diagnostic logging only at a useful state boundary.
- [ ] Update known limitations or lessons learned with evidence.
