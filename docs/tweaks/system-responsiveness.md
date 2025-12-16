# System Responsiveness

> Reduces CPU resources reserved for background tasks.

| | |
|---|---|
| **Category** | System |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | No |

## What It Does

Adjusts the Windows Multimedia Class Scheduler Service (MMCSS) to reduce the percentage of CPU time reserved for background system tasks.

## Technical Details

```
Registry: HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile
SystemResponsiveness: 10 (default: 20)
```

The value represents the percentage of CPU time guaranteed for background tasks. Lower = more resources for foreground apps.

## The Reality

**What SystemResponsiveness does:**
- Default value is 20 (20% reserved for background)
- Setting to 10 halves background reservation
- Allows games to use more CPU resources
- Part of the MMCSS scheduling system

**Impact when optimized:**
- Slightly more CPU available for games
- Background tasks may run slower
- Downloads/updates could be affected
- Minimal measurable difference on modern CPUs

**Impact at default:**
- More balanced CPU distribution
- Background tasks complete faster
- System feels more responsive for multitasking

## When to Optimize

Optimize if:
- You play games on a single-core focused CPU
- You want maximum resources for gaming
- You don't run many background tasks while gaming
- You have a CPU-limited system

Keep default if:
- You have a modern multi-core CPU (6+ cores)
- You multitask while gaming (Discord, browser, etc.)
- You don't notice CPU bottlenecks

## Recommendation

On modern 6+ core CPUs, this tweak has minimal impact. It's more useful on older quad-core systems where every bit of CPU matters.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
