using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public class MGS3MemoryManager
    {
        private static MGS3MemoryManager _instance;
        public static MGS3MemoryManager Instance => _instance ?? (_instance = new MGS3MemoryManager());
        public static IntPtr PROCESS_BASE_ADDRESS = IntPtr.Zero;
        public const string PROCESS_NAME = "METAL GEAR SOLID3";
        private IntPtr lastKnownPointerAddress = IntPtr.Zero;
        private static IntPtr lastLoggedAobAddress = IntPtr.Zero;

        public static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out short lpBuffer, uint size, out int lpNumberOfBytesRead);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesRead);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hObject);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        }

        public static Process GetMGS3Process()
        {
            Process? process = Process.GetProcessesByName(PROCESS_NAME).FirstOrDefault();
            if (process == null)
            {
            }
            return process;
        }

        public static IntPtr OpenGameProcess(Process process)
        {
            try
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Process not found.");
                }

                IntPtr processHandle = NativeMethods.OpenProcess(0x1F0FFF, false, process.Id);
                if (processHandle == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Failed to open process.");
                }

                return processHandle;
            }
            catch (Exception ex)
            {
                LoggingManager.Instance.Log($"OpenGameProcess Exception: {ex.Message}");
                return IntPtr.Zero;
            }
        }

        public static bool ReadProcessMemory(IntPtr processHandle, IntPtr address, byte[] buffer, uint size, out int bytesRead)
        {
            return NativeMethods.ReadProcessMemory(processHandle, address, buffer, size, out bytesRead);
        }

        public static byte[] ReadMemoryBytes(IntPtr processHandle, IntPtr address, int bytesToRead)
        {
            byte[] buffer = new byte[bytesToRead];
            if (NativeMethods.ReadProcessMemory(processHandle, address, buffer, (uint)buffer.Length, out _))
            {
                return buffer;
            }
            return null;
        }

        public float[] ReadSnakePosition(IntPtr processHandle)
        {
            IntPtr snakePointerAddress = FindPointerMemory(11810, 0x130);

            if (snakePointerAddress == IntPtr.Zero)
            {
                return new float[] { 0, 0, 0 }; // No valid pointer
            }

            float[] position = new float[3];
            for (int i = 0; i < position.Length; i++)
            {
                IntPtr currentAddress = IntPtr.Add(snakePointerAddress, i * 4);
                byte[] bytes = MGS3MemoryManager.ReadMemoryBytes(processHandle, currentAddress, 4);
                if (bytes != null && bytes.Length == 4)
                {
                    position[i] = BitConverter.ToSingle(bytes, 0);
                }
                else
                {
                    position[i] = 0;
                }
            }

            if (snakePointerAddress != lastKnownPointerAddress)
            {
                lastKnownPointerAddress = snakePointerAddress;
                LoggingManager.Instance.Log($"Snake's position updated: X={position[0]}, Y={position[1]}, Z={position[2]}");
                LoggingManager.Instance.Log($"Snake's memory address: {snakePointerAddress.ToString("X")}");
            }

            return position;
        }

        public IntPtr FindPointerMemory(int bytesBefore = 11810, int pointerOffset = 0x130)
        {
            IntPtr alphabetAobAddress = MGS3MemoryManager.Instance.FindLastAob("Alphabet", "Alphabet");

            if (alphabetAobAddress == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            IntPtr baseAddress = IntPtr.Subtract(alphabetAobAddress, bytesBefore);

            var process = GetMGS3Process();
            if (process == null || process.MainModule == null)
            {
                return IntPtr.Zero;
            }

            IntPtr processHandle = OpenGameProcess(process);
            if (processHandle == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            IntPtr pointerValue = MGS3MemoryManager.Instance.ReadIntPtr(processHandle, baseAddress);
            NativeMethods.CloseHandle(processHandle);

            if (pointerValue == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            return IntPtr.Add(pointerValue, pointerOffset);
        }

        public bool IsMatch(byte[] buffer, int position, byte[] pattern, string mask)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                char maskChar = mask[i] == ' ' ? 'x' : mask[i];

                if (maskChar == '?' || buffer[position + i] == pattern[i])
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        public List<IntPtr> ScanForAllInstances(IntPtr processHandle, IntPtr startAddress, long size, byte[] pattern, string mask)
        {
            List<IntPtr> foundAddresses = new List<IntPtr>();
            int bufferSize = 10_000_000; // 10 MB buffer
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            long endAddress = startAddress.ToInt64() + size;
            for (long address = startAddress.ToInt64(); address < endAddress; address += bufferSize)
            {
                int effectiveSize = (int)Math.Min(bufferSize, endAddress - address);
                bool success = ReadProcessMemory(processHandle, new IntPtr(address), buffer, (uint)effectiveSize, out bytesRead);
                if (!success || bytesRead == 0) continue;

                for (int i = 0; i <= bytesRead - pattern.Length; i++)
                {
                    if (IsMatch(buffer, i, pattern, mask))
                    {
                        foundAddresses.Add(new IntPtr(address + i));
                        i += pattern.Length - 1;
                    }
                }
            }

            return foundAddresses;
        }

        public List<IntPtr> ScanForAllAobInstances(IntPtr processHandle, IntPtr baseAddress, long moduleSize, byte[] pattern, string mask)
        {
            List<IntPtr> foundAddresses = new List<IntPtr>();
            IntPtr endAddress = IntPtr.Add(baseAddress, (int)moduleSize);
            int patternLength = pattern.Length;
            byte[] searchBuffer = new byte[65536]; // Example buffer size, adjust based on your needs
            long currentPosition = baseAddress.ToInt64();

            while (currentPosition < endAddress.ToInt64())
            {
                if (!NativeMethods.ReadProcessMemory(processHandle, new IntPtr(currentPosition), searchBuffer, (uint)searchBuffer.Length, out int bytesRead) || bytesRead == 0)
                {
                    break; // Exit if read fails or reads zero bytes
                }

                for (int i = 0; i <= bytesRead - patternLength; i++)
                {
                    if (IsMatch(searchBuffer, i, pattern, mask))
                    {
                        IntPtr foundAddress = new IntPtr(currentPosition + i);
                        foundAddresses.Add(foundAddress);
                        i += patternLength - 1; // Move past this match to avoid overlapping finds
                    }
                }

                currentPosition += bytesRead - patternLength + 1; // Move window, avoiding missing overlaps
            }

            return foundAddresses;
        }

        public IntPtr FindLastAob(string key, string aobName)
        {
            if (!AOBs.TryGetValue(key, out var aobData))
            {
                LoggingManager.Instance.Log($"{aobName} AOB not found in AOB Manager.");
                return IntPtr.Zero;
            }

            var process = GetMGS3Process();
            if (process == null || process.MainModule == null)
            {
                return IntPtr.Zero;
            }

            IntPtr processHandle = OpenGameProcess(process);
            if (processHandle == IntPtr.Zero)
            {
                LoggingManager.Instance.Log($"{aobName}: Failed to open game process.");
                return IntPtr.Zero;
            }

            IntPtr baseAddress = process.MainModule.BaseAddress;
            IntPtr startAddress = aobData.StartOffset.HasValue ? IntPtr.Add(baseAddress, (int)aobData.StartOffset.Value) : baseAddress;
            IntPtr endAddress = aobData.EndOffset.HasValue ? IntPtr.Add(baseAddress, (int)aobData.EndOffset.Value) : IntPtr.Add(baseAddress, (int)process.MainModule.ModuleMemorySize);
            long size = endAddress.ToInt64() - startAddress.ToInt64();

            // Use ScanForAllInstances to get all occurrences
            List<IntPtr> foundAddresses = ScanForAllInstances(processHandle, startAddress, size, aobData.Pattern, aobData.Mask);

            NativeMethods.CloseHandle(processHandle);

            if (foundAddresses.Count == 0)
            {
                //LoggingManager.Instance.Log($"{aobName} AOB not found.");
                return IntPtr.Zero;
            }

            // Get the last found address
            IntPtr lastFoundAddress = foundAddresses[^1]; // Using C# 8.0 index from end

            // Only log if the last found AOB address has changed
            if (lastFoundAddress != lastLoggedAobAddress)
            {
                long lastRelativeOffset = lastFoundAddress.ToInt64() - baseAddress.ToInt64();
                LoggingManager.Instance.Log($"Last instance of {aobName} AOB found at: {lastFoundAddress.ToString("X")} (METAL GEAR SOLID 3.exe+{lastRelativeOffset:X})");

                // Update the cached AOB address
                lastLoggedAobAddress = lastFoundAddress;
            }

            return lastFoundAddress;
        }

        /// <summary>
        /// Reads a pointer from the target process's memory.
        /// </summary>
        /// <param name="processHandle">Handle to the target process.</param>
        /// <param name="address">Address to read the pointer from.</param>
        /// <returns>The pointer read from memory; otherwise, IntPtr.Zero.</returns>
        public IntPtr ReadIntPtr(IntPtr processHandle, IntPtr address)
        {
            byte[] buffer = new byte[IntPtr.Size]; // Allocate a buffer to hold the pointer (4 bytes for 32-bit, 8 bytes for 64-bit)
            bool success = ReadProcessMemory(processHandle, address, buffer, (uint)buffer.Length, out int bytesRead);

            if (!success || bytesRead != buffer.Length)
            {
                LoggingManager.Instance.Log($"Failed to read pointer at address: {address.ToString("X")}");
                return IntPtr.Zero;
            }

            // Convert the buffer to an IntPtr
            if (IntPtr.Size == 4) // 32-bit system
            {
                return new IntPtr(BitConverter.ToInt32(buffer, 0));
            }
            else // 64-bit system
            {
                return new IntPtr(BitConverter.ToInt64(buffer, 0));
            }
        }

        public static readonly Dictionary<string, (byte[] Pattern, string Mask, IntPtr? StartOffset, IntPtr? EndOffset)> AOBs =
            new Dictionary<string, (byte[] Pattern, string Mask, IntPtr? StartOffset, IntPtr? EndOffset)>

            {
                {
                    "Alphabet", // 30 00 00 31 00 00 32 00 00 33 - 10958/2ACE is the camo index
                    (new byte[] { 0x30, 0x00, 0x00, 0x31, 0x00, 0x00, 0x32, 0x00, 0x00, 0x33 },
                        "x x x x x x x x x x",
                        new IntPtr(0x1D00000),
                        new IntPtr(0x1F00000)
                    )
                },
            };
    }
}
