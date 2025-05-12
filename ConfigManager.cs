using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public static class ConfigManager
    {
        private static readonly string configPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "MGS Mod Manager and Trainer",
    "NotificationsAndSettings.json"
    );

        public static ConfigSettings LoadSettings()
        {
            string configDirectory = Path.GetDirectoryName(configPath);
            if (!Directory.Exists(configDirectory))
                Directory.CreateDirectory(configDirectory);

            ConfigSettings settings;

            if (!File.Exists(configPath))
            {
                settings = new ConfigSettings();
                SaveSettings(settings);
            }
            else
            {
                var json = File.ReadAllText(configPath);
                settings = JsonConvert.DeserializeObject<ConfigSettings>(json)
                           ?? new ConfigSettings();
            }

            settings.Assets.ModelsAndTexturesFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer",
                "MGS Modding Tools",
                "3D Models and Textures"
            );

            return settings;
        }


        public static void SaveSettings(ConfigSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(configPath, json);
        }
    }

    public class ConfigSettings
    {


        public Dictionary<string, string> GamePaths { get; set; } = new Dictionary<string, string>
        {
            { "MG1andMG2", "" },
            { "MGS1",       "" },
            { "MGS2",       "" },
            { "MGS3",       "" }
        };

        public string MG1andMG2VanillaFolderPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MG1andMG2", "MG1andMG2 Vanilla Files"
            );

        public string MGS1VanillaFolderPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MGS1", "MGS1 Vanilla Files"
            );

        public string MGS2VanillaFolderPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MGS2", "MGS2 Vanilla Files"
            );

        public string MGS3VanillaFolderPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MGS3", "MGS3 Vanilla Files"
            );

        public string MG1andMG2ModFolderPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MG1andMG2", "MG1andMG2 Mods"
            );

        public string MGS1ModFolderPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MGS1", "MGS1 Mods"
            );

        public string MGS2ModFolderPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MGS2", "MGS2 Mods"
            );

        public string MGS3ModFolderPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MGS3", "MGS3 Mods"
            );

        public string ModToolsPath { get; set; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer", "MGS Modding Tools"
            );

        public bool ModToolsFolderSet { get; set; } = false;
        public bool MG1andMG2ModFolderSet { get; set; } = false;
        public bool MGS1VanillaFolderSet { get; set; } = false;
        public bool MGS1ModFolderSet { get; set; } = false;
        public bool MGS1GamePathSet { get; set; } = false;
        public bool MG1andMG2VanillaFolderSet { get; set; } = false;
        public bool MGS2VanillaFolderSet { get; set; } = false;
        public bool MGS2ModFolderSet { get; set; } = false;
        public bool MGS3VanillaFolderSet { get; set; } = false;
        public bool MGS3ModFolderSet { get; set; } = false;

        public AssetsConfig Assets { get; set; } = new AssetsConfig();
        public AppSettings Settings { get; set; } = new AppSettings();
        public BackupStatus Backup { get; set; } = new BackupStatus();
        public ModTracking Mods { get; set; } = new ModTracking();
        public Dictionary<string, string> AudioReplacements { get; set; } = new Dictionary<string, string>();
        public string GimpConsolePath { get; set; } = string.Empty;
        public string GimpPythonScriptPath { get; set; } = string.Empty;
        public string PythonExePath { get; set; } = "";

    }

    public class AssetsConfig
    {
        public string ModelsAndTexturesFolder { get; set; }

        [JsonIgnore]
        public string PythonScriptsFolderPath =>
            Path.Combine(ModelsAndTexturesFolder, "Scripts");

        [JsonIgnore]
        public string PythonScriptPath =>
            Path.Combine(PythonScriptsFolderPath, "PythonFU.py");

        public AssetsConfig()
        {
            ModelsAndTexturesFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MGS Mod Manager and Trainer",
                "MGS Modding Tools",
                "3D Models and Textures"
            );
        }
    }

    public class BackupStatus
    {
        public bool MG1andMG2BackupCompleted { get; set; } = false;
        public bool MGS1BackupCompleted { get; set; } = false;
        public bool MGS2BackupCompleted { get; set; } = false;
        public bool MGS3BackupCompleted { get; set; } = false;
    }

    public class ModTracking
    {
        public Dictionary<string, string> ActiveVariants { get; set; } = new Dictionary<string, string>();
        public bool ModFolderCreated { get; set; } = false;
        public Dictionary<string, bool> ActiveMods { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, List<ModMapping>> ModMappings { get; set; } = new Dictionary<string, List<ModMapping>>();
        public Dictionary<string, List<string>> ReplacedFiles { get; set; } = new Dictionary<string, List<string>>();
    }

    public class AppSettings
    {
        public bool AutoDetectGamePaths { get; set; } = true;
        public bool EnableLogging { get; set; } = true;
        public bool ShowNotifications { get; set; } = true;

        public string LastSeenVersion { get; set; } = "0.0.0";
    }


    public class ModInfo
    {
        public List<ModMapping> Files { get; set; }
    }

    public class ModMapping
    {
        public string ModFile { get; set; }
        public string TargetPath { get; set; }
    }

    public class ModRepository
    {
        public string Owner { get; set; }
        public string Repo { get; set; }
        public string Name { get; set; }
        public string Game { get; set; }
    }

    public class Mod
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Game { get; set; }
    }
}
