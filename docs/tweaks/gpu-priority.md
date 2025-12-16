# GPU Priority

> Forces Windows to use your dedicated GPU for GTA V.

| | |
|---|---|
| **Category** | GPU |
| **Effectiveness** | ⚠️ Minimal |
| **Restart Required** | No |

## What It Does

Sets a registry preference telling Windows to use the high-performance GPU for GTA5.exe and FiveM.exe. This is mainly useful for laptops with both integrated and dedicated graphics.

## Technical Details

```
Registry: HKCU\Software\Microsoft\DirectX\UserGpuPreferences
Key: <path>\GTA5.exe
Value: GpuPreference=2 (High performance)
```

**Also sets for:**
- FiveM.exe
- FiveM_b2802_GTAProcess.exe

## The Reality

**When this actually helps:**
- Laptops with Intel/AMD integrated graphics + NVIDIA/AMD dedicated GPU
- When Windows incorrectly selects the integrated GPU
- Systems where NVIDIA Control Panel settings aren't being respected

**When it doesn't matter:**
- Desktop PCs with only one GPU
- Systems where the game already uses the correct GPU
- If you've already set this in NVIDIA Control Panel or AMD Software

## How to Check

1. Run GTA V / FiveM
2. Open Task Manager → Performance tab
3. Check which GPU shows activity
4. If it's your dedicated GPU, this tweak isn't needed

## Recommendation

Enable if:
- You have a laptop with dual GPUs
- Task Manager shows the wrong GPU being used
- You're getting unexpectedly low FPS

Skip if:
- You have a desktop with one GPU
- Your game already runs on the dedicated GPU

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
