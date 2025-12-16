# Network Throttling Index

> Disables Windows Network Throttling Index.

| | |
|---|---|
| **Category** | Network |
| **Effectiveness** | ❌ Placebo |
| **Restart Required** | No |

## What It Does

Changes the NetworkThrottlingIndex registry value, which limits how much network bandwidth multimedia applications can use.

## Technical Details

```
Registry: HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile

NetworkThrottlingIndex: 10 → 0xFFFFFFFF (disabled)
```

## The Reality

This is a **placebo tweak** that's been copy-pasted around "optimization" guides since Windows Vista.

**What sellers claim:**
- "Removes network throttling"
- "Improves ping in games"
- "More bandwidth for gaming"
- "Microsoft limits your internet speed"

**What it actually does:**
- Changes throttling for **multimedia streaming apps** (like Windows Media Player)
- Designed to prevent video stuttering during high network load
- Has absolutely nothing to do with games

**Why it doesn't affect gaming:**
- Games aren't classified as "multimedia" applications
- This setting targets media playback, not real-time networking
- Your game's network code bypasses this entirely
- FiveM/GTA use UDP which isn't affected anyway

## The Evidence

Microsoft's documentation clearly states this is for multimedia streaming quality, not general network performance. The setting exists to prevent audio/video stuttering when the network is busy.

## Recommendation

Skip this tweak. It's a perfect example of:
- Misunderstood registry settings
- Copy-paste "optimization" guides
- Placebo effect ("I changed something, it must be faster")

Your ping is determined by:
- Physical distance to server
- ISP routing quality
- Network congestion
- Server performance

Not by a multimedia streaming registry key.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
