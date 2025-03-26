using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public static class CtxrConverter
{

    public static void CtxrToPng(string ctxrPath, string pngPath)
    {
        if (!File.Exists(ctxrPath))
            throw new FileNotFoundException("CTXR file not found.", ctxrPath);

        byte[] fileBytes = File.ReadAllBytes(ctxrPath);
        if (fileBytes.Length < 132)
            throw new InvalidDataException("Invalid or too-small CTXR file.");

        ushort width = ReadUInt16BE(fileBytes, 8);
        ushort height = ReadUInt16BE(fileBytes, 10);
        uint pixelDataLen = ReadUInt32BE(fileBytes, 0x80);
        if (132 + pixelDataLen > fileBytes.Length)
            throw new InvalidDataException("Pixel data exceeds file size.");

        byte[] pixelData = new byte[pixelDataLen];
        Buffer.BlockCopy(fileBytes, 132, pixelData, 0, (int)pixelDataLen);

        using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
        {
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int stride = bmpData.Stride;
            int bpp = 4;

            for (int y = 0; y < height; y++)
            {
                int srcRowOffset = y * (width * bpp);
                IntPtr destRowPtr = bmpData.Scan0 + (y * stride);
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = srcRowOffset + x * bpp;
                    byte B = pixelData[srcIndex + 0];
                    byte G = pixelData[srcIndex + 1];
                    byte R = pixelData[srcIndex + 2];
                    byte A = pixelData[srcIndex + 3];
                    int color = (A << 24) | (R << 16) | (G << 8) | B;
                    Marshal.WriteInt32(destRowPtr, x * 4, color);
                }
            }
            bmp.UnlockBits(bmpData);

            bmp.Save(pngPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    public static void CtxrToPngNoAlpha(string ctxrPath, string pngPath)
    {
        if (!File.Exists(ctxrPath))
            throw new FileNotFoundException("CTXR file not found.", ctxrPath);

        byte[] fileBytes = File.ReadAllBytes(ctxrPath);
        if (fileBytes.Length < 132)
            throw new InvalidDataException("Invalid or too-small CTXR file.");

        ushort width = ReadUInt16BE(fileBytes, 8);
        ushort height = ReadUInt16BE(fileBytes, 10);
        uint pixelDataLen = ReadUInt32BE(fileBytes, 0x80);
        if (132 + pixelDataLen > fileBytes.Length)
            throw new InvalidDataException("Pixel data exceeds file size.");

        byte[] pixelData = new byte[pixelDataLen];
        Buffer.BlockCopy(fileBytes, 132, pixelData, 0, (int)pixelDataLen);
        Bitmap bmp32 = new Bitmap(width, height, PixelFormat.Format32bppArgb);

        Rectangle rect = new Rectangle(0, 0, width, height);
        BitmapData bmpData = bmp32.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        int stride = bmpData.Stride;
        int bpp = 4;
        for (int y = 0; y < height; y++)
        {
            int srcRowOffset = y * (width * bpp);
            IntPtr destRowPtr = bmpData.Scan0 + (y * stride);
            for (int x = 0; x < width; x++)
            {
                int srcIndex = srcRowOffset + x * bpp;
                byte B = pixelData[srcIndex + 0];
                byte G = pixelData[srcIndex + 1];
                byte R = pixelData[srcIndex + 2];
                int color = (255 << 24) | (R << 16) | (G << 8) | B;
                Marshal.WriteInt32(destRowPtr, x * 4, color);
            }
        }
        bmp32.UnlockBits(bmpData);
        Bitmap bmp24 = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(bmp24))
        {
            g.DrawImage(bmp32, new Rectangle(0, 0, width, height));
        }
        bmp32.Dispose();

        bmp24.Save(pngPath, ImageFormat.Png);
        bmp24.Dispose();
    }

    public static void PngToCtxr(string modToolsPath, string texconvExePath, string inputPng)
    {
        string ddsPath = Path.ChangeExtension(inputPng, ".dds");
        PngToDdsWithTexconv(texconvExePath, inputPng, ddsPath);

        string ctxrToolExe = Path.Combine(modToolsPath, "CtxrTool.exe");
        DdsToCtxr(ddsPath, ctxrToolExe);
    }


    public static void PngToDdsWithTexconv(string texconvExePath, string inputPng, string outputDds)
    {
        if (!File.Exists(texconvExePath))
            throw new FileNotFoundException("texconv.exe not found.", texconvExePath);
        if (!File.Exists(inputPng))
            throw new FileNotFoundException("PNG file not found.", inputPng);

        string outDir = Path.GetDirectoryName(outputDds);
        if (string.IsNullOrEmpty(outDir))
            outDir = Directory.GetCurrentDirectory();
        Directory.CreateDirectory(outDir);
        string baseName = Path.GetFileNameWithoutExtension(inputPng) + ".dds";
        string generatedDds = Path.Combine(outDir, baseName);

        // Use B8G8R8A8_UNORM to match the settings I would use in GIMP or Photoshop
        string chosenFormat = "B8G8R8A8_UNORM";
        string arguments = string.Join(" ", new string[]
        {
            "-ft dds",                // Output DDS format
            $"-f {chosenFormat}",     // Use B8G8R8A8_UNORM ordering
            "-if POINT",              // Use nearest filter for mipmaps
            "-m 0",                   // Generate full mip chain
            "-nologo",                // Don't print the texconv header
            "-y",                     // Overwrite without prompting
            $"-o \"{outDir}\"",
            $"\"{inputPng}\""
        });

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = texconvExePath,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process proc = new Process { StartInfo = psi })
        {
            proc.Start();
            string stdout = proc.StandardOutput.ReadToEnd();
            string stderr = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new Exception($"texconv exited with code {proc.ExitCode}.\nStdOut:\n{stdout}\n\nStdErr:\n{stderr}");
        }

        if (!File.Exists(generatedDds))
            throw new FileNotFoundException($"texconv claimed success, but '{generatedDds}' not found.");

        if (!generatedDds.Equals(outputDds, StringComparison.OrdinalIgnoreCase))
        {
            if (File.Exists(outputDds))
                File.Delete(outputDds);
            File.Move(generatedDds, outputDds);
        }
    }

    public static void DdsToCtxr(string ddsPath, string ctxrToolExe)
    {
        if (!File.Exists(ddsPath))
            throw new FileNotFoundException("DDS file not found.", ddsPath);
        if (!File.Exists(ctxrToolExe))
            throw new FileNotFoundException("CtxrTool.exe not found.", ctxrToolExe);

        string ddsDir = Path.GetDirectoryName(ddsPath);
        string arguments = $"\"{ddsPath}\"";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = ctxrToolExe,
            Arguments = arguments,
            WorkingDirectory = ddsDir, // Set working directory so output is generated here.
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process proc = new Process { StartInfo = psi })
        {
            proc.Start();
            string stdout = proc.StandardOutput.ReadToEnd();
            string stderr = proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            if (proc.ExitCode != 0)
            {
                throw new Exception($"CtxrTool.exe exited with code {proc.ExitCode}.\nStdOut:\n{stdout}\n\nStdErr:\n{stderr}");
            }
        }

        string generatedFileName = Path.GetFileNameWithoutExtension(ddsPath) + ".ctxr";
        string generatedCtxrPath = Path.Combine(ddsDir, generatedFileName);

        if (!File.Exists(generatedCtxrPath))
            throw new FileNotFoundException($"CtxrTool.exe claimed success, but '{generatedCtxrPath}' was not found.");
    }

    // Big-endian helper methods
    private static ushort ReadUInt16BE(byte[] data, int offset)
    {
        return (ushort)((data[offset] << 8) | data[offset + 1]);
    }

    private static uint ReadUInt32BE(byte[] data, int offset)
    {
        return (uint)(
            (data[offset + 0] << 24) |
            (data[offset + 1] << 16) |
            (data[offset + 2] << 8) |
             data[offset + 3]);
    }

    private static void WriteUInt16BE(byte[] data, int offset, ushort value)
    {
        data[offset + 0] = (byte)((value >> 8) & 0xFF);
        data[offset + 1] = (byte)(value & 0xFF);
    }

    private static void WriteUInt32BE(byte[] data, int offset, uint value)
    {
        data[offset + 0] = (byte)((value >> 24) & 0xFF);
        data[offset + 1] = (byte)((value >> 16) & 0xFF);
        data[offset + 2] = (byte)((value >> 8) & 0xFF);
        data[offset + 3] = (byte)(value & 0xFF);
    }
}