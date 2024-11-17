using BepInEx.Configuration;
using BepInEx;
using Cpp2IL.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmongUsFishing {
    internal static class ModConfig {
        public static ConfigEntry<KeyCode> FishingKey { get; private set; }

        public static void Init(ConfigFile config) {
            InitFishSpritesFolder(Paths.ConfigPath);

            FishingKey = config.Bind(
                "Input",
                "FishingKey",
                KeyCode.P,
                "The key to use to start (and stop) fishing"
            );
        }

        static void InitFishSpritesFolder(string path) {
            var fishSpritesFolder = Path.Combine(path, $"{Plugin.GUID}-FishSprites");
            if (Directory.Exists(fishSpritesFolder))
                return;

            Directory.CreateDirectory(fishSpritesFolder);
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var fishSprite in assembly.GetManifestResourceNames()) {
                var fileName = fishSprite.Split('.', 3)[2];

                using Stream stream = assembly.GetManifestResourceStream(fishSprite);
                using Stream file = File.OpenWrite(Path.Combine(fishSpritesFolder, fileName));

                stream.CopyTo(file);
            }
        }

    }
}
