# Tweaks Reference

Complete documentation for all OpenTweak optimizations. Each tweak is rated honestly for its real-world impact.

## Effectiveness Legend

| Rating | Meaning |
|--------|---------|
| ✅ **Effective** | Measurable impact in most scenarios |
| ⚠️ **Minimal** | Helps in specific cases only |
| ❌ **Placebo** | No real performance benefit |

---

## Power & Performance

| Tweak | Impact | Description |
|-------|--------|-------------|
| [Ultimate Performance](power-plan.md) | ⚠️ Minimal | Activates hidden power plan |
| [Power Throttling Disable](power-throttling.md) | ✅ Effective | Prevents CPU throttling |
| [Timer Resolution](timer-resolution.md) | ✅ Effective | Sets Windows timer to 0.5ms |
| [VBS/HVCI Disable](vbs-disable.md) | ✅ Effective | Disables virtualization security (5-25% FPS) |

## GPU & Graphics

| Tweak | Impact | Description |
|-------|--------|-------------|
| [GPU Priority](gpu-priority.md) | ⚠️ Minimal | Forces dedicated GPU for GTA V |
| [HAGS](hags.md) | ⚠️ Minimal | Hardware Accelerated GPU Scheduling |
| [Game DVR Disable](game-dvr.md) | ⚠️ Minimal | Disables Xbox Game Bar overlay |
| [Windowed Optimizations](windowed-optimizations.md) | ✅ Effective | Win11 latency improvements |

## System & CPU

| Tweak | Impact | Description |
|-------|--------|-------------|
| [System Responsiveness](system-responsiveness.md) | ⚠️ Minimal | Reduces background CPU reservation |
| [Game Priority](game-priority.md) | ⚠️ Minimal | Boosts game scheduler priority |
| [CPU Unparking](cpu-parking.md) | ❌ Placebo | Prevents CPU core parking |
| [Priority Boost](priority-boost.md) | ❌ Placebo | Modifies process scheduling |

## Input & Windows

| Tweak | Impact | Description |
|-------|--------|-------------|
| [Mouse Acceleration Fix](mouse-acceleration.md) | ✅ Effective | Removes mouse acceleration for 1:1 input |
| [FSO Disable](fso-disable.md) | ⚠️ Minimal | Disables Fullscreen Optimizations |
| [Startup Delay Remove](startup-delay.md) | ✅ Effective | Faster Windows boot |
| [Visual Effects Disable](visual-effects.md) | ⚠️ Minimal | Disables Windows animations |

## Network & Cleanup

| Tweak | Impact | Description |
|-------|--------|-------------|
| [TCP Optimizer](network-tcp.md) | ❌ Placebo | Disables Nagle's Algorithm |
| [Network Throttling](network-throttling.md) | ❌ Placebo | Removes network throttling index |
| [Debloater](debloater.md) | ⚠️ Minimal | Disables Windows telemetry services |
| [Service Manager](service-manager.md) | ⚠️ Minimal | Disables unnecessary services |

## Utilities

| Tool | Impact | Description |
|------|--------|-------------|
| [Cache Cleaner](cache-cleaner.md) | ✅ Effective | Clears FiveM cache and logs |
| [Shader Cache Cleaner](shader-cache-cleaner.md) | ✅ Effective | Clears GPU shader caches |
| [RAM Cleaner](ram-cleaner.md) | ⚠️ Minimal | Clears Windows standby memory |

---

## The Honest Summary

**Actually useful:**
- VBS/HVCI Disable (real 5-25% FPS gain, but security trade-off)
- Power Throttling Disable (especially on laptops)
- Timer Resolution (if you notice input lag)
- Mouse Acceleration Fix (for FPS games)
- Startup Delay Remove (faster boot on SSDs)
- Windowed Optimizations (Win11 only, real latency reduction)
- Cache Cleaner (fixes corrupted assets)
- Shader Cache Cleaner (fixes stuttering after driver updates)

**Situational:**
- Ultimate Performance (laptops, older hardware)
- HAGS (test per game, mixed results)
- GPU Priority (laptops with dual GPUs)
- FSO Disable (games with vsync issues)
- Game DVR Disable (if you don't use it)
- Visual Effects (preference, minimal impact)

**Pure marketing:**
- TCP Optimizer (games use UDP, not TCP)
- CPU Unparking (Windows 10/11 handles this)
- Network Throttling (doesn't affect games)
- Priority Boost (negligible difference)

---

← [Back to Main](../../README.md)
