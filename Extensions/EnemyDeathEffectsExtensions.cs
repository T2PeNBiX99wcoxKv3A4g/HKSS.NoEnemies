using System.Diagnostics.CodeAnalysis;
using BepInExUtils.Attributes;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace HKSS.NoEnemies.Extensions;

[AccessExtensions]
[AccessInstance<EnemyDeathEffects>]
[AccessField<bool>("didFire")]
[AccessField<bool>("isCorpseRecyclable")]
[AccessField<string>("awardAchievement")]
[AccessField<AudioMixerSnapshot>("audioSnapshotOnDeath")]
[AccessField<string>("sendEventRegister")]
[AccessField<bool>("recycle")]
[AccessMethod("ShakeCameraIfVisible")]
[AccessMethod("EmitEffects", typeof(GameObject))]
public static partial class EnemyDeathEffectsExtensions
{
    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    public static void ReceiveBeGoneEvent(
        this EnemyDeathEffects effects,
        float? attackDirection,
        AttackTypes attackType,
        NailElements nailElement,
        GameObject damageSource,
        float corpseFlingMultiplier,
        bool resetDeathEvent,
        Action<Transform>? onCorpseBegin,
        out bool didCallCorpseBegin,
        out GameObject? corpseObj)
    {
        didCallCorpseBegin = false;
        corpseObj = null;
        if (effects is { didFire: true, isCorpseRecyclable: false })
            return;
        effects.didFire = true;
        effects.RecordKillForJournal();
        if (corpseFlingMultiplier > 1.350000023841858)
            corpseFlingMultiplier = 1.35f;
        int num;
        switch (attackType)
        {
            case AttackTypes.RuinsWater:
                num = 1;
                break;
            case AttackTypes.Lava:
                if (Configs.Mode != NoEnemiesMode.BeGone)
                    effects.ShakeCameraIfVisible();
                if (GlobalSettings.Corpse.EnemyLavaDeath)
                    GlobalSettings.Corpse.EnemyLavaDeath.Spawn().transform
                        .SetPosition2D(effects.transform.TransformPoint(effects.effectOrigin));

                goto label_10;
            default:
                num = attackType == AttackTypes.Acid ? 1 : 0;
                break;
        }

        corpseObj = effects.EmitCorpse(attackDirection, corpseFlingMultiplier, attackType, nailElement, damageSource,
            onCorpseBegin, out didCallCorpseBegin);
        Utils.Logger.Debug($"corpseObj is spawn: {effects.FullName()} {corpseObj} {corpseObj.FullName()}");
        if (num == 0 && Configs.Mode != NoEnemiesMode.BeGone)
            effects.EmitEffects(corpseObj);

        label_10:
        var instance = GameManager.instance;
        if (!string.IsNullOrEmpty(effects.setPlayerDataBool))
            instance.playerData.SetBool(effects.setPlayerDataBool, true);
        if (!string.IsNullOrWhiteSpace(effects.awardAchievement))
            instance.AwardAchievement(effects.awardAchievement);
        if (!effects.doNotSetHasKilled)
            instance.playerData.SetBool("hasKilled", true);
        if (effects.audioSnapshotOnDeath)
            effects.audioSnapshotOnDeath.TransitionTo(2f);
        if (!string.IsNullOrEmpty(effects.sendEventRegister))
            EventRegister.SendEvent(effects.sendEventRegister);
        if (!resetDeathEvent)
        {
            var component1 = effects.GetComponent<PersistentBoolItem>();
            if (component1)
                component1.SaveState();
            if (effects.recycle)
            {
                var playMakerFsm = FSMUtility.LocateFSM(effects.gameObject, "health_manager_enemy");
                if (playMakerFsm)
                    playMakerFsm.FsmVariables.GetFsmBool("Activated").Value = false;
                var component2 = effects.GetComponent<HealthManager>();
                if (component2)
                    component2.SetIsDead(false);
                effects.didFire = false;
                effects.gameObject.Recycle();
            }
            else
                Object.Destroy(effects.gameObject);
        }
        else
        {
            FSMUtility.SendEventToGameObject(effects.gameObject, "CENTIPEDE DEATH");
            effects.didFire = false;
        }
    }

    public static GameObject EmitCorpse(this EnemyDeathEffects instance, float? attackDirection, float flingMultiplier,
        AttackTypes attackType, NailElements nailElement, GameObject damageSource, Action<Transform>? onCorpseBegin,
        out bool didCallCorpseBegin)
    {
        object?[] args = [attackDirection, flingMultiplier, attackType, nailElement, damageSource, onCorpseBegin, null];
        var method = AccessTools.Method(instance.GetType(), nameof(EmitCorpse));
        if (method == null)
            throw new MethodAccessException($"{nameof(EmitCorpse)} method not found.");
        var ret = method.Invoke(instance, args);
        didCallCorpseBegin = args.GetValueOrDefault(6) is true;
        return ret as GameObject ??
               throw new MethodAccessException($"{nameof(EmitCorpse)} method return value is not GameObject.");
    }
}