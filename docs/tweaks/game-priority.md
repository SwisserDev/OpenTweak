# Game Priority Boost

> Increases Windows scheduler priority for gaming processes.

| | |
|---|---|
| **Category** | System |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | No |

## What It Does

Optimizes the Windows Multimedia Class Scheduler Service (MMCSS) "Games" task category to give games higher CPU and GPU priority.

## Technical Details

```
Registry: HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games
GPU Priority: 8 (maximum, default: 2)
Priority: 6 (high, default: 2)
Scheduling Category: High (default: Medium)
SFIO Priority: High (default: Normal)
```

## The Reality

**What these settings do:**
- GPU Priority: Tells Windows to favor GPU resources for games
- Priority: Sets CPU scheduling priority (1-8 scale)
- Scheduling Category: Sets thread scheduling behavior
- SFIO Priority: Sets file I/O priority for game assets

**Impact when optimized:**
- Games may get slightly more CPU/GPU time
- Asset loading could be faster
- Only works with MMCSS-aware games
- Effect varies by game and system load

**Impact at default:**
- Balanced resource distribution
- Other apps compete more equally
- MMCSS games get medium priority

## When to Optimize

Optimize if:
- You run many background apps while gaming
- You play MMCSS-aware games
- You want to maximize game priority
- You're on a system with limited resources

Keep default if:
- You don't run much in the background
- You haven't noticed resource contention
- Your games already run smoothly

## Recommendation

This is a "can't hurt" tweak. Not all games use MMCSS, so the effect varies. It's most useful when running multiple applications alongside games.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
