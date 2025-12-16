# Ultimate Performance Power Plan

> Activates Windows' hidden "Ultimate Performance" power plan.

| | |
|---|---|
| **Category** | Power |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | No |

## What It Does

Windows has a hidden power plan called "Ultimate Performance" that's normally only available on Workstation editions. This tweak activates it on any Windows 10/11 system.

## Technical Details

```powershell
# Duplicates the hidden Ultimate Performance plan
powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61

# Sets it as active
powercfg -setactive <GUID>
```

**Changes from Balanced/High Performance:**
- Hard disk never sleeps
- USB selective suspend disabled
- CPU minimum state set to 100%
- Display dimming disabled
- PCI Express link state power management disabled

## The Reality

**Where it might help:**
- Laptops that throttle aggressively
- Systems with older CPUs that downclock too aggressively
- Preventing brief stutters from power state changes

**Where it won't help:**
- Modern desktop CPUs (they boost instantly anyway)
- Systems already on "High Performance" plan
- GPU-bound games (this only affects CPU power management)

**Downsides:**
- Higher idle power consumption
- More heat when idle
- Laptop battery drains faster

## Recommendation

Try it if:
- You're on a laptop and notice throttling
- You have brief stutters that might be CPU-related
- You don't care about power consumption

Skip it if:
- You're on a modern desktop (Ryzen 5000+, Intel 10th gen+)
- Power/heat is a concern
- Your games are already smooth

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
