using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;


namespace ANTIBigBoss_MGS_Mod_Manager
{
    public class DownloadManager
    {
        private static readonly HttpClient client = new HttpClient();
        private List<ModRepository> modRepositories;
        private List<Mod> gamebananaMods;

        public DownloadManager()
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ANTIBigBoss_MGS_Mod_Manager/1.0");
            InitializeModRepositories();
            InitializeGamebananaMods();
        }

        private void InitializeModRepositories()
        {
            modRepositories = new List<ModRepository>
            {
                new ModRepository { Owner = "cipherxof", Repo = "MGS3CrouchWalk", Name = "MGS3 Crouch Walk" },
                new ModRepository { Owner = "Lyall", Repo = "MGSHDFix", Name = "MGS HD Fix" },
                new ModRepository { Owner = "nuggslet", Repo = "MGSM2Fix", Name = "MGS M2 Fix" },
                new ModRepository { Owner = "ANTIBigBoss", Repo = "MGS3-Cheat-Trainer-GUI", Name = "MGS3 Cheat Trainer" },
                new ModRepository { Owner = "sagefantasma", Repo = "MGS2-Cheat-Trainer", Name = "MGS2 Cheat Trainer" },
                new ModRepository { Owner = "ANTIBigBoss", Repo = "MGS1-MC-Cheat-Trainer", Name = "MGS1-MC Cheat Trainer" }
            };
        }

        private void InitializeGamebananaMods()
        {
            gamebananaMods = new List<Mod>
            {
                new Mod { Name = "Dark-Haired Raiden", Author = "goostabo", Link = "https://gamebanana.com/dl/1084055", Description = "For Raiden, a simple brown recolor for Raiden's hair and eyebrows." },
                new Mod { Name = "Silver-Haired Raiden", Author = "goostabo", Link = "https://gamebanana.com/dl/1084055", Description = "For Raiden, a dashing silver recolor for Raiden's hair." },
                new Mod { Name = "The Pride Facepaint Package", Author = "Avaresst", Link = "https://gamebanana.com/dl/1105899", Description = "This package includes several facepaint texture variants (can only have 1 installed at a time)." },
                new Mod { Name = "Menu Screen and Loading Icon", Author = "Avaresst", Link = "https://gamebanana.com/dl/1105891", Description = "This mod replaces the old HD collection menu logo with a new Master Collection one that I've made. Also replaces a loading icon leftover from the HD collection." },
                new Mod { Name = "Save and Codec Menu Sprite Change", Author = "Avaresst", Link = "https://gamebanana.com/dl/1105913", Description = "This pack includes 15 sprites you can replace the fox logo from the save menu of both mgs3 and the msx games with and the codec / general menus from mgs3." },
                new Mod { Name = "Crafted", Author = "Avaresst", Link = "https://gamebanana.com/dl/1148464", Description = "Turns most of the games textures into Minecraft theme." },
                new Mod { Name = "Bloodmoon", Author = "Avaresst", Link = "https://gamebanana.com/dl/1311845", Description = "Thematically changes the game for the Halloween spirit with texture and animation changes." },
                new Mod { Name = "Peace Walker Sneaking Suit", Author = "Avaresst", Link = "https://gamebanana.com/dl/1115509", Description = "This mod replaces the mgs3 sneaking suit with the one from Peace Walker / Ground Zeroes." },
                new Mod { Name = "Metal Gear Jolly", Author = "Avaresst", Link = "https://gamebanana.com/dl/1109719", Description = "This mod makes most of the environment snowy and festive, replaces a bunch of enemies, allies, codec sprites, items here and there all to match a Christmas theme." }, 
                new Mod { Name = "Portable Ops USSR Soldiers", Author = "Avaresst", Link = "https://gamebanana.com/dl/1105980", Description = "This mod replaces the mgs3 Gru Soldiers with the yellow suit USSR Soldiers from portable ops." },
                new Mod { Name = "Portable Ops Sneaking Suit", Author = "Avaresst", Link = "https://gamebanana.com/dl/1105976", Description = "This mod replaces the mgs3 sneaking suit with the one from portable ops." },
                new Mod { Name = "More MGSV Accurate Parasites", Author = "Avaresst", Link = "https://gamebanana.com/dl/1105906", Description = "This mod replaces a few textures of the parasitic cobra members to make the Pain, the Fear and the End more accurate / fitting to MGSV's lore and parasites." },
                new Mod { Name = "Menu Screen and Loading Icon", Author = "Avaresst", Link = "https://gamebanana.com/dl/1105886", Description = "This mod replaces the old HD collection menu logo with a new Master Collection one that I've made. Also replaces a loading icon leftover from the HD collection." },
                new Mod { Name = "The Pride Bandana Package", Author = "Avaresst", Link = "https://gamebanana.com/dl/1105897", Description = "This package includes several bandanna texture variants (can only have 1 installed at a time)." },
                new Mod { Name = "Grey Fabric Bandana for Naked Snake", Author = "goostabo", Link = "https://gamebanana.com/dl/1083464", Description = "For Naked Snake, a simple grey fabric bandana neatly tucked in." }
            };
        }

        public List<ModRepository> GetModRepositories()
        {
            return modRepositories;
        }

        public List<Mod> GetGamebananaMods()
        {
            return gamebananaMods;
        }

        public async Task<string> GetModInfo(ModRepository modRepo)
        {
            try
            {
                string apiUrl = $"https://api.github.com/repos/{modRepo.Owner}/{modRepo.Repo}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject repoInfo = JObject.Parse(responseBody);

                string modName = modRepo.Name ?? repoInfo["name"]?.ToString();
                string modAuthor = repoInfo["owner"]?["login"]?.ToString();
                string modDescription = repoInfo["description"]?.ToString();

                return $"Name: {modName}\nAuthor: {modAuthor}\nDescription: {modDescription}";
            }
            catch (Exception ex)
            {
                return $"Error fetching mod info: {ex.Message}";
            }
        }

        public async Task<string> GetLatestReleaseDownloadUrl(ModRepository modRepo)
        {
            try
            {
                string apiUrl = $"https://api.github.com/repos/{modRepo.Owner}/{modRepo.Repo}/releases/latest";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("No releases found for this repository.");
                }

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject releaseInfo = JObject.Parse(responseBody);

                JArray assets = (JArray)releaseInfo["assets"];
                if (assets != null && assets.Count > 0)
                {
                    string downloadUrl = assets[0]["browser_download_url"]?.ToString();
                    return downloadUrl;
                }
                else
                {
                    throw new Exception("No assets found in the latest release.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting download URL: {ex.Message}");
            }
        }

        public async Task DownloadModFile(string downloadUrl, string destinationPath)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(downloadUrl);
                response.EnsureSuccessStatusCode();

                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                System.IO.File.WriteAllBytes(destinationPath, fileBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading mod file: {ex.Message}");
            }
        }

        public async Task EnsureModToolsDownloaded(string modToolsPath)
        {
            if (!Directory.Exists(modToolsPath))
            {
                Directory.CreateDirectory(modToolsPath);
                LoggingManager.Instance.Log($"Created mod tools folder: {modToolsPath}");
            }

            string ctxrToolPath = Path.Combine(modToolsPath, "CtxrTool.exe");
            if (!File.Exists(ctxrToolPath))
            {
                LoggingManager.Instance.Log("CtxrTool.exe not found. Downloading...");
                await DownloadModFile(
                    "https://github.com/Jayveer/CtxrTool/releases/download/1.3/CtxrTool.exe",
                    ctxrToolPath
                );
            }

            string texconvPath = Path.Combine(modToolsPath, "texconv.exe");
            if (!File.Exists(texconvPath))
            {
                LoggingManager.Instance.Log("texconv.exe not found. Downloading...");
                await DownloadModFile(
                    "https://github.com/Microsoft/DirectXTex/releases/latest/download/texconv.exe",
                    texconvPath
                );
            }

            string assetsFolder = Path.Combine(modToolsPath, "3D Models and Textures");
            string assetsZipUrl =
                "https://github.com/ANTIBigBoss/MGS-MC-Mod-Manager-and-Tool/releases/download/ToolsModelsandTextures/3D.Models.and.Textures.zip";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "3DModelsAndTextures.zip");

            string[] requiredFolders = new[]
            {
        "MGS3 Snake SE","MGS3 Snake Sneaking Suit", "MGS3 Tanya (Eva)", "MGS3 Raikov", "MGS3 GRU", "MGS3 KGB", "MGS3 Ocelot Unit", "MGS3 Officer", "MGS3 Scientist",
        "MGS2 Snake Tanker", "MGS2 Raiden", "MGS2 Tanker Guards", "MGS2 Big Shell Guards", "MGS2 Ames", "MGS2 Coolant Spray", "MGS2 Cypher", "MGS2 Directional Microphone", "MGS2 Fatman Bombs", "MGS2 Item Box 1", "MGS2 Item Box 2", "MGS2 M4", "MGS2 M9", "MGS2 Marine", "MGS2 Meryl",  "MGS2 Ocelot", "MGS2 Olga Ninja", "MGS2 Olga Plant", "MGS2 Olga Tanker", "MGS2 Otacon", "MGS2 Pliskin", "MGS2 Raiden", "MGS2 Raiden Ninja", "MGS2 Raiden Scuba", "MGS2 SAA", "MGS2 Scott Dolph", "MGS2 Seal", "MGS2 Snake (MGS1)", "MGS2 Snake Tanker", "MGS2 Socom", "MGS2 Solidus", "MGS2 Stillman", "MGS2 Tuxedo Snake", "MGS2 USP",
        "Scripts"
    };

            bool needsExtract =
                !Directory.Exists(assetsFolder) ||
                requiredFolders.Any(f => !Directory.Exists(Path.Combine(assetsFolder, f)));

            if (needsExtract)
            {
                await DownloadModFile(assetsZipUrl, tempZipPath);

                using (var archive = ZipFile.OpenRead(tempZipPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name))
                            continue;

                        var parts = entry.FullName.Split(new[] { '/', '\\' }, 2);
                        if (parts.Length != 2)
                            continue;
                        string relative = parts[1];

                        if (relative.Equals("Scripts/PythonFU.py", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string destPath = Path.Combine(assetsFolder, relative);
                        string destDir = Path.GetDirectoryName(destPath);
                        Directory.CreateDirectory(destDir);

                        if (!File.Exists(destPath))
                        {
                            entry.ExtractToFile(destPath);
                            LoggingManager.Instance.Log($"Extracted asset: {relative}");
                        }
                    }
                }

                File.Delete(tempZipPath);
            }

            string scriptsFolder = Path.Combine(assetsFolder, "Scripts");
            Directory.CreateDirectory(scriptsFolder);

            string scriptName = "PythonFU.py";
            string scriptPath = Path.Combine(scriptsFolder, scriptName);
            DateTime requiredDate = new DateTime(2025, 4, 19);

            bool needScriptUpdate =
                !File.Exists(scriptPath) ||
                File.GetLastWriteTime(scriptPath).Date != requiredDate;

            if (needScriptUpdate)
            {
                await DownloadModFile(assetsZipUrl, tempZipPath);

                using (var archive = ZipFile.OpenRead(tempZipPath))
                {
                    var entry = archive.Entries.FirstOrDefault(e =>
                        e.FullName.Replace('\\', '/').EndsWith("Scripts/" + scriptName,
                                                             StringComparison.OrdinalIgnoreCase)
                    );
                    if (entry != null)
                    {
                        entry.ExtractToFile(scriptPath, overwrite: true);
                        LoggingManager.Instance.Log(
                            $"Updated {scriptName} to version dated {requiredDate:yyyy-MM-dd}"
                        );
                    }
                }
                File.Delete(tempZipPath);
            }
        }

        public async Task EnsureMGSM2FixDownloaded(string mgsm2FixFolder)
        {
            if (!Directory.Exists(mgsm2FixFolder))
            {
                Directory.CreateDirectory(mgsm2FixFolder);
                LoggingManager.Instance.Log($"Created MGSM2Fix folder: {mgsm2FixFolder}");
            }

            string expectedFile = Path.Combine(mgsm2FixFolder, "MGSM2Fix.ini");
            if (File.Exists(expectedFile))
            {
                LoggingManager.Instance.Log("MGSM2Fix mod already exists. No download necessary.");
                return;
            }

            LoggingManager.Instance.Log("MGSM2Fix mod not found. Downloading...");
            string downloadUrl = "https://github.com/nuggslet/MGSM2Fix/releases/download/v2.2/MGSM2Fix_v_2_2_MGS1.zip";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "MGSM2Fix.zip");

            await DownloadModFile(downloadUrl, tempZipPath);
            LoggingManager.Instance.Log("Extracting MGSM2Fix zip...");

            using (var archive = System.IO.Compression.ZipFile.OpenRead(tempZipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    string destinationPath = Path.Combine(mgsm2FixFolder, entry.FullName);
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }

            File.Delete(tempZipPath);
            LoggingManager.Instance.Log("MGSM2Fix downloaded and extracted successfully.");
        }

        public async Task EnsureMGSHDFixDownloaded(string mgsHDFixFolder)
        {
            if (!Directory.Exists(mgsHDFixFolder))
            {
                Directory.CreateDirectory(mgsHDFixFolder);
                LoggingManager.Instance.Log($"Created MGSHDFix folder: {mgsHDFixFolder}");
            }

            string expectedFile = Path.Combine(mgsHDFixFolder, "MGSHDFix.ini");
            if (File.Exists(expectedFile))
            {
                LoggingManager.Instance.Log("MGSHDFix mod already exists. No download necessary.");
                return;
            }

            LoggingManager.Instance.Log("MGSHDFix mod not found. Downloading...");
            string downloadUrl = "https://github.com/Lyall/MGSHDFix/releases/download/2.4.1/MGSHDFix_2.4.1.zip";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "MGSM2Fix.zip");

            await DownloadModFile(downloadUrl, tempZipPath);
            LoggingManager.Instance.Log("Extracting MGSHDFix zip...");

            using (var archive = System.IO.Compression.ZipFile.OpenRead(tempZipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    string destinationPath = Path.Combine(mgsHDFixFolder, entry.FullName);
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }

            File.Delete(tempZipPath);
            LoggingManager.Instance.Log("MGSHDFix downloaded and extracted successfully.");
        }

        public async Task EnsureMgsFpsUnlockDownloaded(string MgsFpsUnlockFolder)
        {
            if (!Directory.Exists(MgsFpsUnlockFolder))
            {
                Directory.CreateDirectory(MgsFpsUnlockFolder);
                LoggingManager.Instance.Log($"Created MgsFpsUnlock folder: {MgsFpsUnlockFolder}");
            }

            string expectedFile = Path.Combine(MgsFpsUnlockFolder, "MGSFPSUnlock.ini");
            if (File.Exists(expectedFile))
            {
                LoggingManager.Instance.Log("MGSFPSUnlock mod already exists. No download necessary.");
                return;
            }

            LoggingManager.Instance.Log("MGSFPSUnlock mod not found. Downloading...");
            string downloadUrl = "https://github.com/cipherxof/MGSFPSUnlock/releases/tag/0.0.6";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "MGSFPSUnlock.zip");

            await DownloadModFile(downloadUrl, tempZipPath);
            LoggingManager.Instance.Log("Extracting MGSFPSUnlock zip...");

            using (var archive = System.IO.Compression.ZipFile.OpenRead(tempZipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    string destinationPath = Path.Combine(MgsFpsUnlockFolder, entry.FullName);
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }

            File.Delete(tempZipPath);
            LoggingManager.Instance.Log("MGSFPSUnlock downloaded and extracted successfully.");
        }

        public async Task EnsureMgs3CrouchWalkDownloaded(string Mgs3CrouchWalkFolder)
        {
            if (!Directory.Exists(Mgs3CrouchWalkFolder))
            {
                Directory.CreateDirectory(Mgs3CrouchWalkFolder);
                LoggingManager.Instance.Log($"Created Mgs3CrouchWalk folder: {Mgs3CrouchWalkFolder}");
            }

            string expectedFile = Path.Combine(Mgs3CrouchWalkFolder, "Mgs3CrouchWalk.ini");
            if (File.Exists(expectedFile))
            {
                LoggingManager.Instance.Log("Mgs3CrouchWalk mod already exists. No download necessary.");
                return;
            }

            LoggingManager.Instance.Log("Mgs3CrouchWalk mod not found. Downloading...");
            string downloadUrl = "https://github.com/cipherxof/MGS3CrouchWalk/releases/download/0.2.1/MGS3CrouchWalk.zip";
            string tempZipPath = Path.Combine(Path.GetTempPath(), "Mgs3CrouchWalkFolder.zip");

            await DownloadModFile(downloadUrl, tempZipPath);
            LoggingManager.Instance.Log("Extracting Mgs3CrouchWalkFolder zip...");

            using (var archive = System.IO.Compression.ZipFile.OpenRead(tempZipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    string destinationPath = Path.Combine(Mgs3CrouchWalkFolder, entry.FullName);
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }

            File.Delete(tempZipPath);
            LoggingManager.Instance.Log("MGS3CrouchWalk downloaded and extracted successfully.");
        }
    }
}