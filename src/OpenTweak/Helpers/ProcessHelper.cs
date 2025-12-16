using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OpenTweak.Helpers;

/// <summary>
/// Helper methods for process and system operations.
/// </summary>
public static class ProcessHelper
{
    /// <summary>
    /// Run a command and return the output.
    /// </summary>
    public static async Task<(bool Success, string Output, string Error)> RunCommandAsync(
        string fileName, string arguments, int timeoutMs = 30000)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            var completed = await Task.Run(() => process.WaitForExit(timeoutMs));

            if (!completed)
            {
                process.Kill();
                return (false, string.Empty, "Process timed out");
            }

            return (process.ExitCode == 0, await outputTask, await errorTask);
        }
        catch (Exception ex)
        {
            return (false, string.Empty, ex.Message);
        }
    }

    /// <summary>
    /// Run PowerShell command.
    /// </summary>
    public static Task<(bool Success, string Output, string Error)> RunPowerShellAsync(string command)
    {
        return RunCommandAsync("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"");
    }

    /// <summary>
    /// Check if running as administrator.
    /// </summary>
    public static bool IsAdministrator()
    {
        using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        var principal = new System.Security.Principal.WindowsPrincipal(identity);
        return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// Get a process by name, or null if not found.
    /// </summary>
    public static Process? FindProcess(string name)
    {
        return Process.GetProcessesByName(name).FirstOrDefault();
    }

    /// <summary>
    /// Set process priority.
    /// </summary>
    public static bool SetProcessPriority(string processName, ProcessPriorityClass priority)
    {
        try
        {
            var process = FindProcess(processName);
            if (process == null) return false;

            process.PriorityClass = priority;
            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Native Windows API calls.
/// </summary>
public static class NativeMethods
{
    // Timer Resolution
    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern int NtSetTimerResolution(int DesiredResolution, bool SetResolution, out int CurrentResolution);

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern int NtQueryTimerResolution(out int MinimumResolution, out int MaximumResolution, out int CurrentResolution);

    // Memory Management
    [DllImport("psapi.dll", SetLastError = true)]
    public static extern bool EmptyWorkingSet(IntPtr hProcess);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetCurrentProcess();

    // Standby List Clearing (requires SE_PROFILE_SINGLE_PROCESS_PRIVILEGE)
    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern int NtSetSystemInformation(int SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength);

    public const int SystemMemoryListInformation = 80;
    public const int MemoryPurgeStandbyList = 4;
}
