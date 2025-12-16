# RAM Cleaner

> Clears Windows standby memory list.

| | |
|---|---|
| **Category** | Memory |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | No |

## What It Does

Frees up RAM that Windows is keeping in "standby" - memory that was recently used but isn't actively needed right now.

## Technical Details

```c
// Uses Windows API
NtSetSystemInformation(SystemMemoryListInformation, &command, sizeof(command));
// command = MemoryPurgeStandbyList
```

## The Reality

This is where we need to talk about how Windows actually manages RAM.

**How Windows memory works:**
- "Free" RAM = completely empty, unused
- "Standby" RAM = cached data that CAN be freed instantly if needed
- Windows automatically converts standby to free when apps need it

**The RAM cleaner myth:**
- Seeing "16GB used, 0GB free" doesn't mean you're out of RAM
- Standby memory IS available memory
- Windows prioritizes keeping recently-used data cached for speed

**What clearing standby actually does:**
- Makes the "Free RAM" number go up (satisfying, but meaningless)
- Discards useful cached data
- Next time you need that data, it has to reload from disk
- Can actually HURT performance temporarily

## When It Might Help

The only scenario where this helps:
- You have a memory leak in a game/application
- Even then, just restart the application instead

## Recommendation

Skip this tweak. "RAM cleaners" are a holdover from Windows XP days when memory management was worse. Modern Windows handles this perfectly.

If you see high RAM usage:
- That's normal and good
- Empty RAM is wasted RAM
- Windows will free it when needed

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
