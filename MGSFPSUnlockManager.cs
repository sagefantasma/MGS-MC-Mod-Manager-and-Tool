using System;
using System.IO;
using System.Threading.Tasks;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public static class MGSFPSUnlockManager
    {
        private static readonly string[] FileNames = {
            "MGSFPSUnlock.asi",
            "MGSFPSUnlock.ini",
            "d3d11.dll"
        };

        public static async Task EnsureDownloadedAsync(string modFolder)
        {
            var dm = new DownloadManager();
            await dm.EnsureMgsFpsUnlockDownloaded(modFolder);
        }

        public static void Enable(string modFolder, string gameInstallPath, ConfigSettings config)
        {
            if (!Directory.Exists(modFolder))
                throw new DirectoryNotFoundException($"FPS-Unlock mod folder not found: {modFolder}");

            foreach (var file in FileNames)
            {
                var src = Path.Combine(modFolder, file);
                var dst = Path.Combine(gameInstallPath, file);
                if (!File.Exists(src))
                    throw new FileNotFoundException($"Missing FPS-Unlock file: {src}");

                File.Copy(src, dst, true);
            }

            config.Mods.ActiveMods["MGSFPSUnlock"] = true;
            ConfigManager.SaveSettings(config);
        }

        public static void Disable(string gameInstallPath, ConfigSettings config)
        {
            foreach (var file in FileNames)
            {
                var dst = Path.Combine(gameInstallPath, file);
                if (File.Exists(dst))
                    File.Delete(dst);
            }

            config.Mods.ActiveMods["MGSFPSUnlock"] = false;
            ConfigManager.SaveSettings(config);
        }
    }
}
