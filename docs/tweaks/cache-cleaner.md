# FiveM Cache Cleaner

> Deletes FiveM cache, crash dumps, and log files.

| | |
|---|---|
| **Category** | Cleanup |
| **Effectiveness** | ✅ Effective |
| **Restart Required** | No (but close FiveM first) |

## What It Does

Clears temporary files from FiveM's data directory. This is actually one of the most useful "tweaks" because it fixes real issues.

## Technical Details

**Directories cleaned:**
```
%LocalAppData%\FiveM\FiveM.app\
├── cache/      - Temporary game assets (safe to delete)
├── crashes/    - Crash dump files
└── logs/       - Log files
```

**What gets deleted:**
- Downloaded server resources (re-downloaded on next connect)
- Compiled shaders (rebuilt automatically)
- Crash reports (only useful for debugging)
- Old log files

## The Reality

**When this actually fixes problems:**
- Texture glitches or missing assets
- "Failed to load resource" errors
- Crashes after server updates
- Stuck on loading screen
- General FiveM weirdness

**Why it works:**
- Corrupt cache files cause visual bugs
- Old cached resources can conflict with updated server files
- Crash dumps pile up and waste space

**Downside:**
- First server connect after clearing takes longer (re-downloading assets)
- You'll need to re-download resources for each server

## Recommendation

Use this when:
- You see texture bugs or missing props
- A server updated and things are broken
- FiveM is crashing randomly
- You want to free up disk space

Don't run it constantly - only when you have issues. The cache exists to make loading faster.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
