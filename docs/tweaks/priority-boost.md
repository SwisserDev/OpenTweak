# Priority Boost (Win32PrioritySeparation)

> Modifies Windows process priority scheduling.

| | |
|---|---|
| **Category** | CPU |
| **Effectiveness** | ❌ Placebo |
| **Restart Required** | No |

## What It Does

Changes the Win32PrioritySeparation registry value, which controls how Windows allocates CPU time between foreground and background processes.

## Technical Details

```
Registry: HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl

Win32PrioritySeparation: default → 0x26 (or other "magic" values)
```

**The value is a bitmask:**
- Bits 0-1: Priority separation (foreground boost)
- Bits 2-3: Variable/fixed quantum
- Bits 4-5: Short/long quantum

## The Reality

This is another **placebo tweak** from the Windows XP era that refuses to die.

**What "optimization guides" claim:**
- "Optimizes CPU for gaming"
- "Gives more CPU time to games"
- "Reduces background process interference"
- "Secret Windows setting for performance"

**What actually happens:**
- The default setting (0x2) is already optimized for interactive use
- Foreground applications already get priority boost by default
- Changing this value has negligible measurable impact
- Can actually cause issues if set incorrectly

## Why It Doesn't Matter

Modern Windows scheduler is incredibly sophisticated:
- Already boosts foreground process priority
- Games typically run at high/realtime priority anyway
- CPU utilization in games is rarely scheduler-limited
- The quantum (time slice) difference is microseconds

## The Evidence

Run any benchmark with different Win32PrioritySeparation values. Results will be within margin of error. The CPU scheduler isn't your bottleneck.

If CPU scheduling were a real problem, you'd notice:
- Background tasks stealing CPU time visibly
- Stuttering when things happen in the background
- Performance improving when closing other apps

In practice, modern Windows handles this fine.

## Recommendation

Skip this tweak. The Windows defaults are well-tuned for desktop/gaming use. Changing scheduler values is:
- Negligible performance impact
- Potential stability issues
- Solving a problem that doesn't exist

If a game needs more CPU priority, set it specifically for that process rather than changing system-wide scheduler behavior.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
