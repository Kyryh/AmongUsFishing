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
using UObject = UnityEngine.Object;

namespace AmongUsFishing {
    internal static class ModConfig {
        public static ConfigEntry<KeyCode> FishingKey { get; private set; }
        public static ConfigEntry<float> CustomFishProbability { get; private set; }

        public static Sprite[] FishSprites { get; private set; }

        public static void Init(ConfigFile config) {
            InitFishSpritesFolder(Paths.ConfigPath, out var fishSpritesFolder);

            FishSprites = Directory
                .EnumerateFiles(fishSpritesFolder)
                .Select(File.ReadAllBytes)
                .Select(SpriteFromImage)
                .ToArray();

            FishingKey = config.Bind(
                "Input",
                "FishingKey",
                KeyCode.P,
                "The key to use to start (and stop) fishing"
            );

            CustomFishProbability = config.Bind(
                "Fish",
                "CustomFishProbability",
                0.10f,
                new ConfigDescription(
                    "The probability that you catch a custom fish",
                    new AcceptableValueRange<float>(0f, 1f)
                )
            );
        }

        static void InitFishSpritesFolder(string path, out string fishSpritesFolder) {
            fishSpritesFolder = Path.Combine(path, $"{Plugin.GUID}-FishSprites");
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

        static Sprite SpriteFromImage(byte[] bytes) {
            var texture = new Texture2D(0, 0);

            texture.LoadImage(bytes);
            UObject.DontDestroyOnLoad(texture);
            texture.hideFlags |= HideFlags.HideAndDontSave;

            var pixelsPerUnit = Math.Max(texture.width, texture.height) / 2;
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            UObject.DontDestroyOnLoad(sprite);
            sprite.hideFlags |= HideFlags.HideAndDontSave;

            return sprite;
        }
    }
}
