using System.Linq;
using HarmonyLib;
using UnityEngine;
using static CatchFishMinigame;


namespace AmongUsFishing {
    internal static class Patches {
        public static void Init() {
            Harmony.CreateAndPatchAll(typeof(Patches), Plugin.GUID);
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
        [HarmonyPostfix]
        static void InitMinigamePrefab(AmongUsClient __instance) {
            if (Plugin.MinigamePrefab != null)
                return;

            Plugin.Log.LogInfo("Obtaining fishing minigame prefab");

            var funglePrefab = __instance.ShipPrefabs[(int)MapNames.Fungle].LoadAsset<GameObject>().WaitForCompletion(); ;
            var fishingTask = funglePrefab.GetComponent<ShipStatus>().GetAllTasks().First(t => t.TaskType == TaskTypes.CatchFish);
            Plugin.MinigamePrefab = fishingTask.MinigamePrefab.transform.Find("CatchFishMinigame").GetComponent<CatchFishMinigame>();

            Plugin.Log.LogInfo("Prefab obtained");
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        static void CheckInput() {

            if (Input.GetKeyDown(ModConfig.FishingKey.Value)) {
                Plugin.ToggleFishingMinigame();
            }
        }

        [HarmonyPatch(typeof(_CoUpdateProgress_d__42), nameof(_CoUpdateProgress_d__42.MoveNext))]
        [HarmonyPrefix]
        static bool CoUpdateProgressPatch(_CoUpdateProgress_d__42 __instance, ref bool __result) {
            // This updates the task's progress and we definitely don't want that
            // since this isn't a real task, so we skip it completely
            // Also we start the CoBeginFishing coroutine since
            // the original method did it and we skipped it

            __instance.__4__this.StartCoroutine("CoBeginFishing");
            __result = false; 

            // Change the fish sprite just for fun
            __instance.__4__this.RandomizeFishSprite();

            return false;
        }


        [HarmonyPatch(typeof(_CoAnimateCaughtFish_d__41), nameof(_CoAnimateCaughtFish_d__41.MoveNext))]
        [HarmonyPrefix]
        static bool CoAnimateCaughtFishPatch(_CoAnimateCaughtFish_d__41 __instance, ref bool __result) {
            // This just does some animation stuff or something idk
            // The last step calls CatchFishMinigame.SetCaughtFish and increments
            // CatchFishMinigame.numCaughtFish, so we skip only the last step

            if (__instance.__1__state == 2) {
                __result = false;
                return false;
            }

            return true;
        }

        public static void RandomizeFishSprite(this CatchFishMinigame minigame) {
            minigame.ChangeFishSprite((byte)Random.Range(0, minigame.fishCounterSprites.Length));
        }
        public static void ChangeFishSprite(this CatchFishMinigame minigame, byte spriteIndex) {
            minigame.MyNormTask.Data[1] = spriteIndex;
        }

    }
}
