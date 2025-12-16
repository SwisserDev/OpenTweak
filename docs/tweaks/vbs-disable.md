# VBS/HVCI Disable

> Disables Virtualization-Based Security for maximum gaming performance.

| | |
|---|---|
| **Category** | Security |
| **Effectiveness** | ✅ Effective |
| **Restart Required** | Yes |

## What It Does

Disables Windows Virtualization-Based Security (VBS) and Hypervisor-Enforced Code Integrity (HVCI/Memory Integrity), which can significantly improve gaming performance.

## Technical Details

```
Registry: HKLM\SYSTEM\CurrentControlSet\Control\DeviceGuard
EnableVirtualizationBasedSecurity: 0

Registry: HKLM\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity
Enabled: 0
```

## The Reality

**What VBS/HVCI does:**
- Creates a secure memory enclave isolated from the OS
- Prevents malware from modifying protected code
- Protects credentials (Credential Guard)
- Uses Hyper-V to run security checks

**Impact when enabled:**
- 5-25% performance reduction depending on CPU/game
- Extra CPU overhead for virtualization
- Some older games may run worse
- Noticeable on older CPUs (pre-10th gen Intel, pre-Ryzen 3000)

**Impact when disabled:**
- Full CPU performance restored
- Reduced security against kernel-level attacks
- Credential Guard disabled
- Windows Hello may have issues

## When to Disable

Disable if:
- You're a competitive gamer needing every FPS
- You have an older CPU that struggles with VBS overhead
- You don't need enterprise security features
- You understand the security trade-offs

Keep enabled if:
- You play games with anti-cheat (Valorant Vanguard needs VBS)
- Security is more important than 5-10 FPS
- You use Windows Hello/Credential Guard
- You're on a shared/work computer

## Warning

⚠️ **This significantly reduces system security.** VBS protects against sophisticated malware and credential theft. Only disable if you understand the risks and prioritize gaming performance over security.

## Recommendation

For most gamers: Test with VBS disabled. If you gain significant FPS (10%+), keep it disabled. If the gain is minimal, re-enable for better security.

---

← [Back to Tweaks Overview](README.md) · [Back to Main](../../README.md)
