using System;
using System.IO;
using System.Threading.Tasks;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public static class MGS3CrouchWalkManager
    {
        private static readonly string[] RootFiles = {
            "MGS3CrouchWalk.asi",
            "MGS3CrouchWalk.ini",
            "d3d11.dll"
        };

        public static async Task EnsureDownloadedAsync(string modFolder)
        {
            var dm = new DownloadManager();
            await dm.EnsureMgs3CrouchWalkDownloaded(modFolder);
        }

        public static void Enable(string modFolder, string gameInstallPath, FileExplorerManager fm, ConfigSettings config)
        {
            // 1) copy root files
            foreach (var f in RootFiles)
            {
                var src = Path.Combine(modFolder, f);
                var dst = Path.Combine(gameInstallPath, f);
                if (!File.Exists(src))
                    throw new FileNotFoundException($"Missing CrouchWalk file: {src}");
                File.Copy(src, dst, true);
            }

            // 2) install assets/mtar/... just like any other mod
            fm.ApplyModFiles(modFolder, gameInstallPath);

            config.Mods.ActiveMods["MGS3CrouchWalk"] = true;
            ConfigManager.SaveSettings(config);
        }

        public static void Disable(string modFolder, string gameInstallPath, FileExplorerManager fm, ConfigSettings config)
        {
            // 1) uninstall all assets (this will pull from your vanilla backup)
            fm.RestoreVanillaFiles(modFolder, gameInstallPath);

            // 2) restore or delete the three root files
            var vanRoot = config.MGS3VanillaFolderPath;
            foreach (var f in RootFiles)
            {
                var backup = Path.Combine(vanRoot, f);
                var current = Path.Combine(gameInstallPath, f);
                if (File.Exists(backup))
                {
                    File.Copy(backup, current, true);
                }
                else if (File.Exists(current))
                {
                    File.Delete(current);
                }
            }

            config.Mods.ActiveMods["MGS3CrouchWalk"] = false;
            ConfigManager.SaveSettings(config);
        }
    }
}
