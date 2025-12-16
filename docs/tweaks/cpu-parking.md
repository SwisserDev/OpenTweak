# CPU Unparking

> Prevents Windows from "parking" (sleeping) CPU cores.

| | |
|---|---|
| **Category** | CPU |
| **Effectiveness** | ❌ Placebo |
| **Restart Required** | No |

## What It Does

Core Parking is a Windows feature that puts idle CPU cores into a low-power state. This tweak disables it, keeping all cores active at all times.

## Technical Details

```
Registry: HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerSettings\
          54533251-82be-4824-96c1-47b60b740d00\
          0cc5b647-c1df-4637-891a-dec35c318583

ValueMin: 0 → 100 (all cores always active)
ValueMax: 0 → 100
```

## The Reality

This tweak was relevant around 2010-2012 when Windows 7 had issues with core parking and some games. Modern Windows handles this intelligently.

**Why it's a placebo now:**
- Windows 10/11 unparks cores in microseconds when needed
- Gaming workloads naturally use multiple cores
- The "Ultimate Performance" power plan already disables parking
- Modern CPUs boost so fast that parking is irrelevant

**Why "FPS boosters" still sell it:**
- It sounds technical and impressive
- Easy to implement (just registry changes)
- "More cores active = more better" sounds logical

## The Evidence

Run a benchmark with and without this tweak. You won't see a difference outside of margin of error. The cores are already awake when the game needs them.

## Recommendation

Skip this tweak. It:
- Doesn't improve gaming performance
- Slightly increases idle power consumption
- Is already handled by Ultimate Performance plan

If you really want it, just use Ultimate Performance instead.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
