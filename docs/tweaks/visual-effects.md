# Visual Effects Disable

> Disables Windows visual effects for snappier UI.

| | |
|---|---|
| **Category** | Windows |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | No |

## What It Does

Disables Windows visual effects like animations, shadows, and transparency to reduce GPU/CPU overhead and make the UI feel more responsive.

## Technical Details

```
Registry: HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects
VisualFXSetting: 2 (Best Performance)

Registry: HKCU\Software\Microsoft\Windows\DWM
EnableAeroPeek: 0

Registry: HKCU\Control Panel\Desktop
DragFullWindows: 0
MenuShowDelay: 0
```

## The Reality

**What visual effects include:**
- Window minimize/maximize animations
- Menu fade/slide animations
- Smooth scrolling
- Window shadows
- Aero Peek (taskbar previews)
- Transparency effects

**Impact when disabled:**
- Snappier UI interactions
- Lower GPU usage for desktop
- Windows looks more "basic"
- Instant menu/window responses
- Minimal FPS gain in games

**Impact when enabled:**
- Polished modern Windows look
- Smooth animations
- Small amount of GPU overhead
- Slightly higher compositor load

## When to Disable

Disable if:
- You prefer instant UI responses
- You're on integrated graphics
- You want the cleanest possible look
- Every bit of GPU matters (low-end system)

Keep enabled if:
- You like Windows' visual polish
- You have a dedicated GPU
- Visual effects don't bother you
- You're not on extremely limited hardware

## Recommendation

This is primarily a preference tweak. On modern systems with dedicated GPUs, the performance impact is negligible. But if you prefer snappy UI over pretty animations, disable away.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
