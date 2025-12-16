# Service Manager

> Disables unnecessary Windows services to free resources.

| | |
|---|---|
| **Category** | Windows |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | No |

## What It Does

Disables Windows services that are typically not needed for gaming, freeing up small amounts of RAM and CPU cycles.

## Services Disabled

| Service | Purpose |
|---------|---------|
| PrintSpooler | Print queue management |
| Fax | Fax sending/receiving |
| WSearch | Windows Search indexing |
| MapsBroker | Offline maps download manager |
| WalletService | Windows Wallet/Payment |
| PhoneSvc | Your Phone companion app |
| TabletInputService | Touch keyboard/handwriting |

## Technical Details

```
Registry: HKLM\SYSTEM\CurrentControlSet\Services\[ServiceName]
Start: 4 (disabled) / 2 (automatic) / 3 (manual)
```

## The Reality

**What these services do:**
- PrintSpooler: Manages print jobs (needed for printers)
- Fax: Legacy fax functionality
- WSearch: Indexes files for fast searching
- MapsBroker: Downloads map data for Maps app
- Others: Various Windows features you may not use

**Impact when disabled:**
- Frees ~50-200 MB RAM depending on services
- Reduces background CPU usage
- Disables functionality you may need
- Faster boot time (slightly)

**Impact when enabled:**
- Services consume resources even when idle
- WSearch can cause disk activity
- Features remain available when needed

## When to Disable

Disable if:
- You don't have a printer (PrintSpooler)
- You never use fax (Fax)
- You use Everything/Listary instead (WSearch)
- You don't use Windows Maps (MapsBroker)
- You don't use Your Phone app (PhoneSvc)
- You don't use touch input (TabletInputService)

Keep enabled if:
- You use a printer regularly
- You rely on Windows Search
- You use the affected Windows features

## Warning

⚠️ Disabling WSearch breaks Windows Search in Start menu and File Explorer. Use an alternative like Everything if you disable it.

## Recommendation

Only disable services you're certain you don't need. The performance gain is minimal on modern systems with plenty of RAM.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
