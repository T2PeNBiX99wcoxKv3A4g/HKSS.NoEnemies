using UnityEngine;

namespace HKSS.NoEnemies.Behaviour;

public class NoEnemiesController : MonoBehaviour
{
    internal static HitInstance HitInstance = new()
    {
        AttackType = AttackTypes.Generic,
        CircleDirection = false,
        DamageDealt = 9999,
        Direction = 0.0f,
        IgnoreInvulnerable = true,
        Multiplier = 1f
    };

    internal HealthManager? HealthManager;

    private void Start()
    {
        HitInstance.Source = HeroController.instance.gameObject;
        Utils.Logger.Debug($"Enemy {gameObject.name} hp {HealthManager?.hp}");
        EnemyHandle();
    }

    private void EnemyHandle()
    {
        if (Configs.Mode == NoEnemiesMode.Off) return;
        if (Configs.Mode != NoEnemiesMode.BeGone) return;
        EnemyBeGone();
    }

    private void EnemyBeGone()
    {
        Utils.Logger.Info($"Enemy {gameObject.name} be gone");

        if (!HealthManager) return;
        HealthManager.DieDropFling(HitInstance.ToolDamageFlags, true);
        HealthManager.BeGone(null, HitInstance.AttackType, HitInstance.NailElement, HitInstance.Source,
            HitInstance.IgnoreInvulnerable, 0.0f);
    }
}