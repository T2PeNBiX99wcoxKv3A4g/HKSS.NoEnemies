using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace HKSS.NoEnemies.Patch;

[HarmonyPatch(typeof(BlackThreadState))]
internal class BlackThreadStatePatches
{
    [HarmonyPatch(nameof(BlackThreadState.ChooseAttack))]
    [HarmonyPrefix]
    private static void ChooseAttack(BlackThreadState __instance, bool force)
    {
        Utils.Logger.Debug($"{__instance.gameObject.name} Trying to choose attack, is force: {force}");
    }
}