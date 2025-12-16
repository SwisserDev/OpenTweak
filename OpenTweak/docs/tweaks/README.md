# Tweaks Reference

Complete documentation for all OpenTweak optimizations. Each tweak is rated honestly for its real-world impact.

## Effectiveness Legend

| Rating | Meaning |
|--------|---------|
| ✅ **Effective** | Measurable impact in most scenarios |
| ⚠️ **Minimal** | Helps in specific cases only |
| ❌ **Placebo** | No real performance benefit |

---

## System Tweaks

| Tweak | Impact | Description |
|-------|--------|-------------|
| [Timer Resolution](timer-resolution.md) | ✅ Effective | Sets Windows timer to 0.5ms |
| [Game DVR Disable](game-dvr.md) | ⚠️ Minimal | Disables Xbox Game Bar overlay |
| [FSO Disable](fso-disable.md) | ⚠️ Minimal | Disables Fullscreen Optimizations |
| [Debloater](debloater.md) | ⚠️ Minimal | Disables Windows telemetry services |

## Power & CPU

| Tweak | Impact | Description |
|-------|--------|-------------|
| [Ultimate Performance](power-plan.md) | ⚠️ Minimal | Activates hidden power plan |
| [CPU Unparking](cpu-parking.md) | ❌ Placebo | Prevents CPU core parking |
| [Priority Boost](priority-boost.md) | ❌ Placebo | Modifies process scheduling |

## GPU

| Tweak | Impact | Description |
|-------|--------|-------------|
| [GPU Priority](gpu-priority.md) | ⚠️ Minimal | Forces dedicated GPU for GTA V |

## Network

| Tweak | Impact | Description |
|-------|--------|-------------|
| [TCP Optimizer](network-tcp.md) | ❌ Placebo | Disables Nagle's Algorithm |
| [Network Throttling](network-throttling.md) | ❌ Placebo | Removes network throttling index |

## Input

| Tweak | Impact | Description |
|-------|--------|-------------|
| [Mouse Acceleration Fix](mouse-acceleration.md) | ✅ Effective | Removes mouse acceleration for 1:1 input |

## Utilities

| Tool | Impact | Description |
|------|--------|-------------|
| [Cache Cleaner](cache-cleaner.md) | ✅ Effective | Clears FiveM cache and logs |
| [RAM Cleaner](ram-cleaner.md) | ⚠️ Minimal | Clears Windows standby memory |

---

## The Honest Summary

**Actually useful:**
- Timer Resolution (if you notice input lag)
- Mouse Acceleration Fix (for FPS games)
- Cache Cleaner (fixes corrupted assets)
- Game DVR Disable (if you don't use it)

**Situational:**
- Ultimate Performance (laptops, older hardware)
- GPU Priority (laptops with dual GPUs)
- FSO Disable (games with vsync issues)

**Pure marketing:**
- TCP Optimizer (games use UDP, not TCP)
- CPU Unparking (Windows 10/11 handles this)
- Network Throttling (doesn't affect games)
- Priority Boost (negligible difference)

---

← [Back to Main](../../README.md)
