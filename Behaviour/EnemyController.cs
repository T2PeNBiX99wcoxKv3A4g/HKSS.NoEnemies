using UnityEngine;

namespace HKSS.NoEnemies.Behaviour;

public class EnemyController : MonoBehaviour
{
    internal HealthManager? HealthManager;
    internal static Dictionary<GameObject, EnemyController> EnemyControllerCache = new();
    internal static Dictionary<GameObject, HealthManager> HealthManagerCache = new();
    private const int MaybeBoss = 105;
    private bool _maybeIsBoss;
    private bool _killWhenSpawn;

    private HitInstance _hitInstance = new()
    {
        AttackType = AttackTypes.Explosion,
        CircleDirection = false,
        DamageDealt = 9999,
        Direction = 0.0f,
        IgnoreInvulnerable = true,
        Multiplier = 1f
    };

    private void Awake()
    {
        EnemyControllerCache[gameObject] = this;
    }

    private void Start()
    {
        if (HealthManager)
            HealthManagerCache[gameObject] = HealthManager;

        _killWhenSpawn = Configs.Mode == NoEnemiesMode.KillWhenSpawn;
        var hp = HealthManager?.hp ?? -1;
        if (hp is > MaybeBoss or < 0)
        {
            _killWhenSpawn = true;
            _maybeIsBoss = true;
        }

        _hitInstance.Source = HeroController.instance.gameObject;

        Utils.Logger.Debug($"Enemy {gameObject.name} hp {hp}");
        EnemyHandle();
    }

    private void EnemyHandle()
    {
        if (!gameObject.activeSelf || Configs.Mode == NoEnemiesMode.Off) return;
        if (_killWhenSpawn)
            EnemyKillWhenSpawn();
        else if (Configs.Mode == NoEnemiesMode.BeGone)
            EnemyBeGone();
    }

    private void EnemyBeGone()
    {
        Utils.Logger.Info($"Enemy {gameObject.name} be gone");

        if (HealthManager)
        {
            HealthManager.DieDropFling(_hitInstance.ToolDamageFlags, false);
            HealthManager.Die(null, _hitInstance.AttackType, _hitInstance.NailElement, _hitInstance.Source,
                _hitInstance.IgnoreInvulnerable, 0.0f);
        }

        gameObject.SetActive(false);
    }

    private void EnemyKillWhenSpawn()
    {
        if (!HealthManager) return;
        HealthManager.ApplyExtraDamage(_hitInstance);
        Utils.Logger.Info($"Enemy {gameObject.name} be die");
    }

    private void Update()
    {
        if (Configs.Mode is NoEnemiesMode.Off or NoEnemiesMode.KillWhenTryAttack || !_maybeIsBoss || !HealthManager ||
            HealthManager.hp <= 0 || HealthManager.isDead) return;
        Utils.Logger.Debug($"Boss: {gameObject.name} HP: {HealthManager.hp}");
        FSMUtility.SendEventToGameObject(gameObject, "TOOK TAG DAMAGE");
        FSMUtility.SendEventToGameObject(gameObject, "TOOK DAMAGE");
        if (!HealthManager.hasBlackThreadState || !HealthManager.blackThreadState.IsInForcedSing)
            FSMUtility.SendEventToGameObject(gameObject, "SING DURATION END");
        HealthManager.DieDropFling(_hitInstance.ToolDamageFlags, false);
        HealthManager.Die(null, _hitInstance.AttackType, _hitInstance.NailElement, _hitInstance.Source,
            _hitInstance.IgnoreInvulnerable, 0.0f);
    }
}