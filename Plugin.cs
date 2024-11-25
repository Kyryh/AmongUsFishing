﻿using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using System;
using System.Linq;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace AmongUsFishing;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public const string GUID = $"Kyryh.{MyPluginInfo.PLUGIN_GUID}";

    internal static new ManualLogSource Log;

    static CatchFishMinigame minigamePrefab;

    public static CatchFishMinigame MinigameInstance { get; private set; }
    static NormalPlayerTask minigameTask;
    public static CatchFishMinigame MinigamePrefab {
        get {
            return minigamePrefab;
        }
        set {
            if (minigamePrefab != null)
                throw new InvalidOperationException();
            minigamePrefab = value;
            allFishSprites = value.fishCounterSprites.Concat(ModConfig.FishSprites).ToArray();
        } 
    }

    static Sprite[] allFishSprites;
    public static Sprite[] AllFishSprites => allFishSprites;
    

    public override void Load() {
        // Plugin startup logic
        Log = base.Log;

        IL2CPPMethods.Init();
        Patches.Init();
        ModConfig.Init(Config);


        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public static void ToggleFishingMinigame() {
        if (MinigameInstance == null) {
            MinigameInstance = UObject.Instantiate(MinigamePrefab, Camera.main.transform, false);
            MinigameInstance.transform.Find("FishCounters").gameObject.SetActive(false);
            MinigameInstance.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
            MinigameInstance.fishCounterSprites = AllFishSprites;
            minigameTask = MinigameInstance.gameObject.AddComponent<NormalPlayerTask>();
            minigameTask.Data = new byte[4];
        }

        if (!MinigameInstance.isActiveAndEnabled) {
            if (Minigame.Instance == null) {
                MinigameInstance.gameObject.SetActive(true);
                MinigameInstance.Begin(minigameTask);
                MinigameInstance.RandomizeFishSprite();
            }
        }
        else {
            MinigameInstance.Close();
        }
    }
}
