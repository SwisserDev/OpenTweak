# Game DVR Disable

> Disables Xbox Game Bar and background recording.

| | |
|---|---|
| **Category** | Windows |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | Recommended |

## What It Does

Turns off Windows' built-in Xbox Game Bar overlay (Win+G) and its background video recording feature.

## Technical Details

```
Registry: HKCU\System\GameConfigStore
GameDVR_Enabled: 0

Registry: HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR
AppCaptureEnabled: 0

Registry: HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR
AllowGameDVR: 0
```

## The Reality

**What Game DVR does:**
- Background recording (last 30 seconds, etc.)
- Win+G overlay for screenshots/recording
- Performance monitoring overlay
- Xbox social features

**Impact when enabled:**
- Uses ~1-3% GPU for video encoding (when recording)
- Small amount of RAM for buffer
- Can cause stuttering in some games
- Overlay can interfere with other overlays

**Impact when disabled:**
- Lose instant replay feature
- No Win+G overlay
- No Xbox Game Bar widgets

## When to Disable

Disable if:
- You never use Game Bar features
- You use OBS/Shadowplay/ReLive instead
- You notice micro-stutters that might be from encoding
- You're trying to squeeze out every bit of performance

Keep enabled if:
- You use instant replay (Win+Alt+G)
- You like the overlay features
- You don't have performance issues

## Recommendation

If you don't use Xbox Game Bar, disable it. It's not a massive performance gain, but there's no reason to keep it running if you use other recording software.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
