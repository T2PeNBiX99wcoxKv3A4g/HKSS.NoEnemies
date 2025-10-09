using HarmonyLib;
using HKSS.NoEnemies.Behaviour;

// ReSharper disable InconsistentNaming

namespace HKSS.NoEnemies.Patch;

[HarmonyPatch(typeof(HealthManager))]
internal class HealthManagerPatches
{
    [HarmonyPatch(nameof(Awake))]
    [HarmonyPrefix]
    private static void Awake(HealthManager __instance)
    {
        Utils.Logger.Debug($"__instance {__instance} {__instance.gameObject.name}");
        var component = !__instance.gameObject.TryGetComponent<EnemyController>(out var getController)
            ? __instance.gameObject.AddComponent<EnemyController>()
            : getController;
        Utils.Logger.Debug($"controller {component}");
        component.HealthManager = __instance;
    }
}