# Shader Cache Cleaner

> Deletes GPU shader caches (NVIDIA, AMD, DirectX).

| | |
|---|---|
| **Category** | Cleanup |
| **Effectiveness** | ✅ Effective |
| **Restart Required** | No (but close games first) |

## What It Does

Clears compiled shader caches from your GPU driver and Windows DirectX. Shaders are small programs that run on your GPU to render graphics. They get compiled and cached for faster loading.

## Technical Details

**Directories cleaned:**
```
%LocalAppData%\
├── NVIDIA\DXCache\                    - NVIDIA DirectX shaders
├── NVIDIA\GLCache\                    - NVIDIA OpenGL shaders
├── AMD\DxCache\                       - AMD DirectX shaders
├── D3DSCache\                         - General DirectX cache
└── Microsoft\DirectX Shader Cache\    - Windows shader cache
```

**What gets deleted:**
- Compiled shader binaries (rebuilt automatically)
- Pipeline state objects
- Shader compilation metadata

## The Reality

**When this actually fixes problems:**
- Stuttering after GPU driver updates
- Stuttering after Windows updates
- Shader compilation stutters in GTA V
- Visual glitches or corrupted rendering
- "Micro-stutters" during gameplay

**Why it works:**
- Old cached shaders can be incompatible with new drivers
- Corrupt shader cache causes compilation errors
- Forces fresh shader compilation with current driver

**Downside:**
- First game launch after clearing will have shader compilation stutters
- GTA V will need to rebuild its shader cache (can take 1-2 minutes)
- This is temporary - only happens once after cleaning

## When to Use

**Do use when:**
- You updated your GPU drivers
- You're experiencing new stuttering in games
- GTA V has shader-related visual bugs
- After Windows feature updates

**Don't use:**
- Before every gaming session (wasteful)
- If your games are running fine
- Just for "optimization" - the cache exists to help performance

## How It Works

1. GPU drivers compile HLSL/GLSL shaders to GPU-specific bytecode
2. This compilation is cached to avoid doing it every game launch
3. Sometimes these caches become stale or corrupt
4. Clearing forces fresh compilation with your current driver

## Notes

- Only cleans caches for installed GPU vendors (NVIDIA/AMD)
- Safe to run - Windows and drivers rebuild caches automatically
- Close games before running for best results
- Consider running after major driver updates

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
