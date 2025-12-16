using Microsoft.Win32;

namespace OpenTweak.Helpers;

/// <summary>
/// Helper methods for registry operations with backup support.
/// </summary>
public static class RegistryHelper
{
    /// <summary>
    /// Get a registry value, returning null if it doesn't exist.
    /// </summary>
    public static object? GetValue(string keyPath, string valueName, RegistryHive hive = RegistryHive.LocalMachine)
    {
        using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
        using var key = baseKey.OpenSubKey(keyPath);
        return key?.GetValue(valueName);
    }

    /// <summary>
    /// Set a registry value, creating the key if it doesn't exist.
    /// </summary>
    public static bool SetValue(string keyPath, string valueName, object value, RegistryValueKind kind,
        RegistryHive hive = RegistryHive.LocalMachine)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var key = baseKey.CreateSubKey(keyPath, true);
            if (key == null) return false;

            key.SetValue(valueName, value, kind);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Delete a registry value.
    /// </summary>
    public static bool DeleteValue(string keyPath, string valueName, RegistryHive hive = RegistryHive.LocalMachine)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var key = baseKey.OpenSubKey(keyPath, true);
            if (key == null) return true; // Already doesn't exist

            key.DeleteValue(valueName, false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get all subkey names under a path.
    /// </summary>
    public static string[] GetSubKeyNames(string keyPath, RegistryHive hive = RegistryHive.LocalMachine)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var key = baseKey.OpenSubKey(keyPath);
            return key?.GetSubKeyNames() ?? Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Backup a registry value before modifying.
    /// </summary>
    public static RegistryBackup? BackupValue(string keyPath, string valueName,
        RegistryHive hive = RegistryHive.LocalMachine)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var key = baseKey.OpenSubKey(keyPath);

            if (key == null)
            {
                return new RegistryBackup
                {
                    KeyPath = keyPath,
                    ValueName = valueName,
                    Hive = hive,
                    OriginalValue = null,
                    OriginalKind = null,
                    Existed = false
                };
            }

            var value = key.GetValue(valueName);
            var kind = value != null ? key.GetValueKind(valueName) : (RegistryValueKind?)null;

            return new RegistryBackup
            {
                KeyPath = keyPath,
                ValueName = valueName,
                Hive = hive,
                OriginalValue = value,
                OriginalKind = kind,
                Existed = value != null
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Restore a registry value from backup.
    /// </summary>
    public static bool RestoreBackup(RegistryBackup backup)
    {
        if (!backup.Existed)
        {
            return DeleteValue(backup.KeyPath, backup.ValueName, backup.Hive);
        }

        if (backup.OriginalValue == null || backup.OriginalKind == null)
            return false;

        return SetValue(backup.KeyPath, backup.ValueName, backup.OriginalValue,
            backup.OriginalKind.Value, backup.Hive);
    }
}

public class RegistryBackup
{
    public required string KeyPath { get; init; }
    public required string ValueName { get; init; }
    public required RegistryHive Hive { get; init; }
    public object? OriginalValue { get; init; }
    public RegistryValueKind? OriginalKind { get; init; }
    public bool Existed { get; init; }
}
