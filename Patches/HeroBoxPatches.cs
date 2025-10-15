using HarmonyLib;
using HKSS.NoEnemies.Behaviour;
using UnityEngine;

namespace HKSS.NoEnemies.Patches;

[HarmonyPatch(typeof(HeroBox))]
internal class HeroBoxPatches
{
    [HarmonyPatch(nameof(HeroBox.CheckForDamage))]
    private static bool CheckForDamage(HeroBox __instance, GameObject otherGameObject)
    {
        if (!otherGameObject) return true;
        Utils.Logger.Debug($"TakeDamage by {otherGameObject.gameObject.FullName()}");
        if (Configs.Mode != NoEnemiesMode.KillWhenTryAttack) return true;
        var parentObj = otherGameObject.transform.parent ? otherGameObject.transform.parent.gameObject : null;
        var rootObj = otherGameObject.transform.root ? otherGameObject.transform.root.gameObject : null;
        var healthManager = otherGameObject.TryGetComponent<HealthManager>(out var value) ? value :
            parentObj?.TryGetComponent<HealthManager>(out value) ?? false ? value :
            rootObj?.TryGetComponent<HealthManager>(out value) ?? false ? value : null;
        if (!healthManager) return true;
        Utils.Logger.Debug($"{otherGameObject.gameObject.FullName()} Trying to attack");
        healthManager.DieDropFling(NoEnemiesController.HitInstance.ToolDamageFlags, true);
        healthManager.BeGone(null, NoEnemiesController.HitInstance.AttackType,
            NoEnemiesController.HitInstance.NailElement, NoEnemiesController.HitInstance.Source,
            NoEnemiesController.HitInstance.IgnoreInvulnerable, 0.0f);
        return false;
    }
}