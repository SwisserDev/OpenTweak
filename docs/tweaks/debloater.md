# Windows Debloater

> Disables Windows telemetry and diagnostic services.

| | |
|---|---|
| **Category** | System |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | Recommended |

## What It Does

Disables various Windows background services that collect telemetry data and perform diagnostics. This can reduce background CPU/disk activity.

## Technical Details

**Services disabled:**
```
DiagTrack          - Connected User Experiences and Telemetry
dmwappushservice   - WAP Push Message Routing Service
diagnosticshub.*   - Diagnostic collection services
```

**Registry changes:**
```
HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection
AllowTelemetry: 0 (disabled)
```

## The Reality

**What it actually does:**
- Reduces some background data collection
- Slightly less disk/network activity
- May improve privacy (debatable)
- Frees up minimal system resources

**What it won't do:**
- Noticeably improve FPS
- Free up significant RAM
- Make Windows faster in any meaningful way

**Potential downsides:**
- Windows Update might work less reliably
- Some diagnostic features won't work
- Microsoft feedback/reporting disabled

## Real-World Impact

The telemetry services use maybe 0.1% CPU occasionally and a few MB of RAM. Disabling them won't give you extra frames. The main benefit is privacy, not performance.

## Recommendation

Enable if:
- You care about reducing telemetry
- You want to minimize background processes on principle
- You're on a very low-spec system where every bit helps

Skip if:
- You're doing this for FPS gains (won't happen)
- You rely on Windows diagnostics
- You don't want to deal with potential update issues

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
