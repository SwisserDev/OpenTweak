# Startup Delay Remove

> Removes the Windows startup delay for faster boot.

| | |
|---|---|
| **Category** | Windows |
| **Effectiveness** | ✅ Effective |
| **Restart Required** | No |

## What It Does

Removes the built-in Windows delay before launching startup programs, making your desktop usable faster after login.

## Technical Details

```
Registry: HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize
StartupDelayInMSec: 0 (default: not set, ~10000ms)
```

When this key doesn't exist, Windows waits approximately 10 seconds after login before launching startup programs.

## The Reality

**What the startup delay does:**
- Gives Windows time to fully initialize
- Prevents startup programs from competing with boot
- Makes the login-to-desktop transition smoother
- Added to reduce early boot resource contention

**Impact when removed:**
- Desktop becomes usable faster
- Startup programs launch immediately
- Brief moment of higher resource usage
- May feel more responsive overall

**Impact at default:**
- 10 second delay before programs start
- Slower time to "fully ready" desktop
- More staggered resource usage

## When to Remove

Remove if:
- You have an SSD (handles concurrent loads well)
- You want to get to work/gaming faster
- You don't mind brief initial busyness
- You have a modern multi-core CPU

Keep default if:
- You're on an HDD (slower disk = more contention)
- You have many startup programs
- Your system struggles during boot

## Recommendation

On any SSD-based system, removing this delay makes boot feel significantly snappier. The default delay is a holdover from HDD era optimization.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
