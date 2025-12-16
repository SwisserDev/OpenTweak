# TCP Optimizer (Nagle's Algorithm)

> Disables Nagle's Algorithm for TCP connections.

| | |
|---|---|
| **Category** | Network |
| **Effectiveness** | ❌ Placebo |
| **Restart Required** | No |

## What It Does

Nagle's Algorithm buffers small TCP packets to reduce network overhead. Disabling it sends packets immediately without waiting.

## Technical Details

```
Registry: HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\{adapter-guid}

TcpNoDelay: 0 → 1
TcpAckFrequency: default → 1
```

## The Reality

This is one of the most commonly sold "ping reducers" that does essentially nothing for gaming.

**Why it doesn't help FiveM/GTA V:**
- **Games use UDP, not TCP.** Nagle's only affects TCP.
- TCP is used for: web requests, file downloads, initial connection
- UDP is used for: actual gameplay data, position updates, actions
- Disabling Nagle on TCP doesn't touch UDP at all

**What "TCP optimization" tools don't tell you:**
- The game's netcode decides how packets are sent
- Server tick rate is the bottleneck, not your TCP settings
- Your ISP's routing matters infinitely more than registry tweaks

## The Marketing

Sellers claim:
- "Reduces ping by disabling packet buffering"
- "Sends game data immediately"
- "Optimizes network for gaming"

Reality:
- Might reduce latency for TCP web requests by a few ms
- Has zero effect on actual gameplay networking
- Your ping is determined by physical distance and routing

## Recommendation

Skip this tweak. If you want better ping:
- Use ethernet instead of WiFi
- Choose servers closer to you
- Check if your ISP has gaming-optimized routing
- Accept that physics limits the speed of light

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
