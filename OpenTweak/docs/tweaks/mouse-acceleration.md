# Mouse Acceleration Fix

> Removes Windows mouse acceleration for true 1:1 input.

| | |
|---|---|
| **Category** | Input |
| **Effectiveness** | ✅ Effective |
| **Restart Required** | Yes (or log out) |

## What It Does

Disables Windows "Enhance Pointer Precision" (mouse acceleration) and applies linear mouse curves. This is the famous "MarkC Mouse Fix" that competitive FPS players use.

## Technical Details

```
Registry: HKCU\Control Panel\Mouse
MouseSpeed: 0
MouseThreshold1: 0
MouseThreshold2: 0

Registry: HKCU\Control Panel\Mouse
SmoothMouseXCurve: [linear values]
SmoothMouseYCurve: [linear values]
```

**What acceleration does:**
- Moving mouse slowly = cursor moves proportionally
- Moving mouse fast = cursor moves EXTRA distance
- The faster you move, the more "bonus" movement you get

**What 1:1 means:**
- 1 inch of mouse movement = same cursor distance, always
- No speed-based scaling
- Predictable, learnable muscle memory

## The Reality

This is one of the few tweaks that genuinely matters for gaming.

**Why acceleration is bad for games:**
- Inconsistent aiming (fast flicks vs. slow tracking differ)
- Hard to build muscle memory
- Can't predict where crosshair will end up

**Why 1:1 is better:**
- Same physical movement = same result, every time
- Easier to build consistent aim
- Used by virtually all competitive FPS players

**Note:** Many games disable acceleration internally anyway. But system-level 1:1 ensures consistency everywhere.

## Recommendation

Enable this if you play any FPS games. There's no downside for gaming, and it's what every competitive player uses.

The only reason to keep acceleration is if you:
- Prefer it for desktop use (some people do)
- Have limited desk space and need fast traversal

You can always revert if you don't like it.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
