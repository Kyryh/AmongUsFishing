using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using System;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace AmongUsFishing;

[BepInPlugin(GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public const string GUID = $"Kyryh.{MyPluginInfo.PLUGIN_GUID}";

    internal static new ManualLogSource Log;

    static CatchFishMinigame minigamePrefab;

    static CatchFishMinigame minigameInstance;
    static NormalPlayerTask minigameTask;
    public static CatchFishMinigame MinigamePrefab {
        get {
            return minigamePrefab;
        }
        set {
            if (minigamePrefab != null)
                throw new InvalidOperationException();
            minigamePrefab = value;
        } 
    }
    public override void Load() {
        // Plugin startup logic
        Log = base.Log;

        IL2CPPMethods.Init();
        Patches.Init();
        ModConfig.Init(Config);


        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public static void ToggleFishingMinigame() {
        if (minigameInstance == null) {
            minigameInstance = UObject.Instantiate(MinigamePrefab, Camera.main.transform, false);
            minigameInstance.transform.Find("FishCounters").gameObject.SetActive(false);
            minigameInstance.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
            minigameInstance.fishCounterSprites = ModConfig.FishSprites;
            minigameTask = minigameInstance.gameObject.AddComponent<NormalPlayerTask>();
            minigameTask.Data = new byte[4];
        }

        if (!minigameInstance.isActiveAndEnabled) {
            minigameInstance.gameObject.SetActive(true);
            minigameInstance.Begin(minigameTask);
            minigameInstance.RandomizeFishSprite();
        }
        else {
            minigameInstance.Close();
        }
    }
}
