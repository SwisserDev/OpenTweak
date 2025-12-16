# Power Throttling Disable

> Prevents Windows from throttling CPU performance to save power.

| | |
|---|---|
| **Category** | Power |
| **Effectiveness** | ✅ Effective |
| **Restart Required** | No |

## What It Does

Disables Windows Power Throttling, which normally reduces CPU clock speeds for background and less important processes to save energy.

## Technical Details

```
Registry: HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerThrottling
PowerThrottlingOff: 1 (disabled) / 0 (enabled)
```

## The Reality

**What Power Throttling does:**
- Reduces CPU frequency for "less important" processes
- Saves power and reduces heat
- Can mistakenly throttle game-related processes
- Part of Windows' modern power management

**Impact when disabled:**
- All processes run at full CPU speed
- More consistent gaming performance
- Higher power consumption
- More heat generation
- Especially effective on laptops

**Impact when enabled:**
- Windows decides what processes to throttle
- Can cause inconsistent game performance
- GPU may bottleneck waiting for CPU
- Better battery life on laptops

## When to Disable

Disable if:
- You're on a laptop with aggressive power management
- You notice CPU clock speed drops during gaming
- You want consistent maximum performance
- You're plugged in and don't care about power

Keep enabled if:
- You're on battery and need power savings
- Your system runs too hot
- You prefer quieter operation
- You don't have performance issues

## Recommendation

This is one of the more effective tweaks, especially for laptops. Combined with Ultimate Performance power plan, it ensures your CPU always runs at maximum speed during gaming.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
