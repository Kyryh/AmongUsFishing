using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmongUsFishing {
    internal static class ModConfig {
        public static ConfigEntry<KeyCode> FishingKey { get; private set; }

        public static void Init(ConfigFile config) {
            FishingKey = config.Bind(
                "Input",
                "FishingKey",
                KeyCode.P,
                "The key to use to start (and stop) fishing"
            );
        }
    }
}
