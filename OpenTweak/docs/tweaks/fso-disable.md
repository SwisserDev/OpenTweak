# Fullscreen Optimizations Disable

> Disables Windows 10/11 Fullscreen Optimizations for GTA V.

| | |
|---|---|
| **Category** | System |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | No |

## What It Does

Disables "Fullscreen Optimizations" - a Windows 10/11 feature that runs fullscreen games in a special borderless window mode instead of true exclusive fullscreen.

## Technical Details

```
Registry: HKCU\Software\Microsoft\Windows NT\CurrentVersion\
          AppCompatFlags\Layers

Key: <path>\GTA5.exe
Value: DISABLEDXMAXIMIZEDWINDOWEDMODE
```

**Also applied to:**
- FiveM.exe
- FiveM_b2802_GTAProcess.exe

## The Reality

Fullscreen Optimizations was introduced to:
- Allow faster Alt+Tab
- Enable Windows notifications over games
- Support mixed refresh rate displays
- Enable HDR switching

**When disabling helps:**
- Games with vsync tearing issues
- Stuttering in some older games
- Input lag sensitive to windowed mode
- Games that don't play nice with DWM

**When it doesn't matter:**
- Most modern games work fine with it
- GTA V/FiveM generally handles it well
- If you haven't noticed issues, you probably don't have them

## How FSO Works

With FSO enabled:
- Game runs in "optimized" borderless mode
- Windows compositor (DWM) is still active
- Faster Alt+Tab, but potentially more input lag

With FSO disabled:
- True exclusive fullscreen
- DWM is bypassed
- Potentially lower input lag, but slower Alt+Tab

## Recommendation

Try disabling if:
- You notice screen tearing with vsync on
- You feel input lag in fullscreen
- You have stuttering issues

Keep enabled if:
- Everything works fine
- You Alt+Tab frequently
- You use multiple monitors with different refresh rates

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
