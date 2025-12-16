# Timer Resolution (0.5ms)

> Sets the Windows system timer to 0.5ms for more precise timing.

| | |
|---|---|
| **Category** | System |
| **Effectiveness** | ✅ Effective |
| **Restart Required** | No |

## What It Does

Windows has a default timer resolution of approximately 15.6ms. This tweak reduces it to 0.5ms (500 microseconds), which can improve the precision of time-sensitive operations in games.

## Technical Details

```c
// Uses Windows NT API
NtSetTimerResolution(5000, true, out actualResolution);
// 5000 = 0.5ms in 100-nanosecond intervals
```

**Important:** This only stays active while OpenTweak is running. Close the app and it reverts.

## The Reality

**What it actually helps with:**
- More precise frame pacing
- Potentially smoother input timing
- Better sleep() precision for games that use it

**What it doesn't do:**
- Magically reduce input lag by 15ms
- "Sync with server tick rate" (that's not how networking works)
- Double your FPS

**Side effects:**
- Slightly higher CPU power consumption
- Most modern games already request high timer resolution themselves

## Recommendation

This is one of the few tweaks that can have a measurable effect. Worth enabling if:
- You're sensitive to micro-stutters
- You play competitive shooters
- You want consistent frame timing

Not needed if your games already feel smooth.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
