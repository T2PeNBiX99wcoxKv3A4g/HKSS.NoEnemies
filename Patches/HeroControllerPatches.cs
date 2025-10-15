using GlobalEnums;
using HarmonyLib;
using HKSS.NoEnemies.Behaviour;
using UnityEngine;

namespace HKSS.NoEnemies.Patches;

[HarmonyPatch(typeof(HeroController))]
internal static class HeroControllerPatches
{
    [HarmonyPatch(nameof(HeroController.TakeDamage))]
    [HarmonyPrefix]
    private static bool TakeDamage(HeroController __instance, GameObject go, CollisionSide damageSide, int damageAmount,
        HazardType hazardType, DamagePropertyFlags damagePropertyFlags)
    {
        if (!go) return true;
        Utils.Logger.Debug($"TakeDamage by {go.gameObject.FullName()}");
        if (Configs.Mode != NoEnemiesMode.KillWhenTryAttack) return true;
        var parentObj = go.transform.parent ? go.transform.parent.gameObject : null;
        var rootObj = go.transform.root ? go.transform.root.gameObject : null;
        var healthManager = go.TryGetComponent<HealthManager>(out var value) ? value :
            parentObj?.TryGetComponent<HealthManager>(out value) ?? false ? value :
            rootObj?.TryGetComponent<HealthManager>(out value) ?? false ? value : null;
        if (!healthManager) return true;
        Utils.Logger.Debug($"{go.gameObject.FullName()} Trying to attack");
        healthManager.DieDropFling(NoEnemiesController.HitInstance.ToolDamageFlags, true);
        healthManager.BeGone(null, NoEnemiesController.HitInstance.AttackType,
            NoEnemiesController.HitInstance.NailElement, NoEnemiesController.HitInstance.Source,
            NoEnemiesController.HitInstance.IgnoreInvulnerable, 0.0f);
        __instance.SetState(ActorStates.idle);
        __instance.AcceptInput();
        return false;
    }
}