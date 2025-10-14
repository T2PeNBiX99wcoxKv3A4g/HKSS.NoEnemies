using HarmonyLib;
using HKSS.NoEnemies.Behaviour;

// ReSharper disable InconsistentNaming

namespace HKSS.NoEnemies.Patches;

[HarmonyPatch(typeof(HealthManager))]
internal class HealthManagerPatches
{
    [HarmonyPatch(nameof(Awake))]
    [HarmonyPrefix]
    private static void Awake(HealthManager __instance)
    {
        Utils.Logger.Debug($"__instance {__instance} {__instance.gameObject.name}");
        var component = !__instance.gameObject.TryGetComponent<NoEnemiesController>(out var getController)
            ? __instance.gameObject.AddComponent<NoEnemiesController>()
            : getController;
        Utils.Logger.Debug($"controller {component}");
        component.HealthManager = __instance;
    }
}