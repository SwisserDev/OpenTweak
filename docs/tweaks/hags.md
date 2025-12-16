# Hardware Accelerated GPU Scheduling (HAGS)

> Lets the GPU manage its own memory scheduling for potentially lower latency.

| | |
|---|---|
| **Category** | GPU |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | Yes |

## What It Does

Enables Hardware Accelerated GPU Scheduling, which offloads GPU memory scheduling from the CPU to the GPU's dedicated scheduling processor.

## Technical Details

```
Registry: HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers
HwSchMode: 2 (enabled) / 1 (disabled)
```

## Requirements

- Windows 10 2004 or later / Windows 11
- WDDM 2.7+ compatible GPU driver
- NVIDIA GTX 1000 series or newer
- AMD RX 5000 series or newer
- Intel UHD 600 series or newer

## The Reality

**What HAGS does:**
- Moves GPU scheduling from Windows to the GPU itself
- GPU processes work in batches more efficiently
- Can reduce CPU overhead during GPU-intensive tasks
- Potentially lower input latency

**Impact when enabled:**
- Mixed results: some games improve, others don't
- May reduce input latency by 1-2ms
- Slight CPU usage reduction
- Can cause issues with screen recording (OBS)

**Impact when disabled:**
- Traditional CPU-managed GPU scheduling
- More predictable behavior
- Better compatibility with older software

## When to Enable

Enable if:
- You have a modern GPU (GTX 10xx+, RX 5xxx+)
- You want to test for latency improvements
- You don't use OBS or screen recording software
- Your specific games benefit from it

Keep disabled if:
- You use OBS or screen recording software
- You experience stuttering with HAGS on
- You have an older GPU
- Your games run worse with it enabled

## Recommendation

HAGS is very game/hardware dependent. Enable it and test your specific games. If you notice improvements, keep it. If not, or if you have issues, disable it.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
