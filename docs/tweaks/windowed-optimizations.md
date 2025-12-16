# Windowed Game Optimizations

> Enables Windows 11 latency optimizations for windowed games.

| | |
|---|---|
| **Category** | Windows |
| **Effectiveness** | ✅ Effective |
| **Restart Required** | No |

## What It Does

Enables Windows 11's "Optimizations for windowed games" feature, which significantly reduces input latency when playing games in borderless windowed mode.

## Technical Details

```
Registry: HKCU\Software\Microsoft\DirectX\UserGpuPreferences
DirectXUserGlobalSettings: SwapEffectUpgradeEnable=1;
```

Also accessible via: Settings → System → Display → Graphics → Change default graphics settings

## Requirements

- Windows 11 (no effect on Windows 10)
- DirectX 10 or DirectX 11 games
- Games running in borderless windowed mode

## The Reality

**What this optimization does:**
- Upgrades swap chain handling for DX10/11 games
- Reduces latency in borderless windowed mode
- Makes windowed mode closer to exclusive fullscreen
- Microsoft claims "significant" latency improvements

**Impact when enabled:**
- Lower input latency in windowed games
- Better Alt+Tab behavior
- Smoother frame pacing
- May cause issues with some older games

**Impact when disabled:**
- Traditional windowed mode behavior
- Higher latency than fullscreen
- More compatible with older games

## When to Enable

Enable if:
- You play games in borderless windowed mode
- You're on Windows 11
- You want lower input latency
- You frequently Alt+Tab

Keep disabled if:
- You always use exclusive fullscreen
- You experience issues with specific games
- You're on Windows 10 (no effect)
- A game specifically recommends disabling it

## Recommendation

If you're on Windows 11 and prefer borderless windowed mode, enable this. It's one of the few tweaks that provides a real, measurable latency improvement.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
