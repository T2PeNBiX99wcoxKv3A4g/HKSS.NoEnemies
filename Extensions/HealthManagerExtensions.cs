using BepInExUtils.Attributes;
using GlobalEnums;
using GlobalSettings;
using HKSS.NoEnemies.Proxy;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace HKSS.NoEnemies.Extensions;

[AccessExtensions]
[AccessInstance<HealthManager>]
[AccessField<bool>("hasBlackThreadState")]
[AccessField<BlackThreadState>("blackThreadState")]
[AccessField<GameObject>("battleScene")]
[AccessField<bool>("bigEnemyDeath")]
[AccessField<bool>("preventDeathAfterHero")]
[AccessField<int>("shellShardDrops")]
[AccessField<int>("smallGeoDrops")]
[AccessField<int>("mediumGeoDrops")]
[AccessField<int>("largeGeoDrops")]
[AccessField<int>("largeSmoothGeoDrops")]
[AccessField<float[]>("_shellShardMultiplierArray")]
[AccessField<bool>("megaFlingGeo")]
[AccessField<HealthManager.QueuedDropItem>("queuedDropItem")]
[AccessField<GameObject>("zeroHPEventOverride")]
[AccessField<DamageHero>("damageHero")]
[AccessField<bool>("notifiedBattleScene")]
[AccessField<AudioMixerSnapshot>("deathAudioSnapshot")]
[AccessField<GameObject>("sendKilledTo")]
[AccessField<GameObject>("corpseSplatPrefab")]
[AccessField<Vector3>("effectOrigin")]
[AccessField<EnemyDeathEffects>("enemyDeathEffects")]
[AccessField<EventRelayResponder>("corpseEventResponder")]
[AccessMethod("Invincible", typeof(HitInstance))]
[AccessMethod("TakeDamage", typeof(HitInstance))]
[AccessMethod("DieDropFling", typeof(ToolDamageFlags), typeof(bool))]
[AccessMethod("SpawnCurrency", typeof(Transform), typeof(float), typeof(float), typeof(float), typeof(float),
    typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool), typeof(int), typeof(bool))]
[AccessMethod("SpawnQueuedItemDrop", typeof(HealthManager.QueuedDropItem), typeof(Transform))]
[AccessMethod("SpawnItemDrop", typeof(SavedItem), typeof(int), typeof(CollectableItemPickup), typeof(Transform),
    typeof(int))]
[AccessMethod("NonFatalHit", typeof(bool))]
[UsedImplicitly]
public static partial class HealthManagerExtensions
{
    // extension doesn't support inside action method
    public static void BeGone(this HealthManager instance, float? attackDirection, AttackTypes attackType,
        NailElements nailElement,
        GameObject damageSource, bool ignoreEvasion = false, float corpseFlingMultiplier = 1f,
        bool overrideSpecialDeath = false, bool disallowDropFling = false)
    {
        Utils.Logger.Debug($"BeGone is called: {instance.gameObject.FullName()}");
        instance.hp = -1;
        if (instance.isDead) return;
        instance.CancelAllLagHits();
        var flingMagnitudeMult = GlobalSettings.Corpse.MinCorpseFlingMagnitudeMult;
        if (corpseFlingMultiplier < flingMagnitudeMult && Math.Abs(corpseFlingMultiplier) > Mathf.Epsilon)
            corpseFlingMultiplier = flingMagnitudeMult;
        Action<Transform>? onCorpseBegin = null;
        if (!disallowDropFling && attackType != AttackTypes.RuinsWater &&
            GameManager.instance.GetCurrentMapZoneEnum() != MapZone.MEMORY)
        {
            var smallGeoExtra = 0;
            var mediumGeoExtra = 0;
            var largeGeoExtra = 0;
            var largeSmoothGeoExtra = 0;
            var shellShardExtra = 0;
            var shellShardBase = instance.shellShardDrops;
            var thiefCharmEquipped = Gameplay.ThiefCharmTool.IsEquipped;
            if (thiefCharmEquipped)
            {
                smallGeoExtra =
                    Mathf.CeilToInt(instance.smallGeoDrops * Gameplay.ThiefCharmGeoSmallIncrease);
                mediumGeoExtra =
                    Mathf.CeilToInt(instance.mediumGeoDrops * Gameplay.ThiefCharmGeoMedIncrease);
                largeGeoExtra =
                    Mathf.CeilToInt(instance.largeGeoDrops * Gameplay.ThiefCharmGeoLargeIncrease);
                largeSmoothGeoExtra =
                    Mathf.CeilToInt(instance.largeSmoothGeoDrops * Gameplay.ThiefCharmGeoLargeIncrease);
            }

            var shellShardMultiplier =
                instance._shellShardMultiplierArray[Random.Range(0, instance._shellShardMultiplierArray.Length)];
            var num = Mathf.CeilToInt(shellShardBase * shellShardMultiplier - shellShardBase);
            if (num > 8)
                num = 8;
            shellShardBase += num;
            var boneNecklaceTool = Gameplay.BoneNecklaceTool;
            if (boneNecklaceTool && boneNecklaceTool.IsEquipped)
                shellShardExtra = Mathf.CeilToInt(shellShardBase * Gameplay.BoneNecklaceShellshardIncrease);
            var dropFlingAngleMin = instance.megaFlingGeo ? 65 : 80 /*0x50*/;
            var dropFlingAngleMax = instance.megaFlingGeo ? 115 : 100;
            var dropFlingSpeedMin = instance.megaFlingGeo ? 30 : 15;
            var dropFlingSpeedMax = instance.megaFlingGeo ? 45 : 30;
            var sourceTransform = instance.transform;
            var queuedItem = instance.queuedDropItem;
            instance.queuedDropItem.Reset();
            onCorpseBegin = spawnPoint =>
            {
                Utils.Logger.Debug(
                    $"onCorpseBegin is called: {instance.gameObject.FullName()} {spawnPoint.FullName()} {spawnPoint}");
                instance.SpawnCurrency(spawnPoint, dropFlingSpeedMin, dropFlingSpeedMax,
                    dropFlingAngleMin, dropFlingAngleMax, instance.smallGeoDrops,
                    instance.mediumGeoDrops,
                    instance.largeGeoDrops, instance.largeSmoothGeoDrops, false, shellShardBase, false);
                instance.SpawnCurrency(spawnPoint, dropFlingSpeedMin, dropFlingSpeedMax,
                    dropFlingAngleMin, dropFlingAngleMax, 0, 0, 0, 0, false, shellShardExtra, true);
                if (smallGeoExtra > 0 || mediumGeoExtra > 0 || largeGeoExtra > 0 || largeSmoothGeoExtra > 0)
                {
                    if (spawnPoint == sourceTransform)
                        SpawnExtraGeo();
                    else
                        spawnPoint.GetComponent<MonoBehaviour>().ExecuteDelayed(0.2f, SpawnExtraGeo);
                }

                instance.SpawnQueuedItemDrop(queuedItem, spawnPoint);
                foreach (var rootByProbability in from itemDropGroup in instance.itemDropGroups
                         where itemDropGroup.Drops.Count > 0 && itemDropGroup.TotalProbability >= 1.0
                         select (ItemDropProbabilityCopy)Probability
                             .GetRandomItemRootByProbability<ItemDropProbabilityCopy, SavedItem>(
                                 itemDropGroup.ToDropsCopyArray()))
                    instance.SpawnItemDrop(rootByProbability.Item, rootByProbability.Amount.GetRandomValue(),
                        rootByProbability.CustomPickupPrefab!, spawnPoint,
                        rootByProbability.LimitActiveInScene);

                return;

                void SpawnExtraGeo()
                {
                    if (thiefCharmEquipped)
                        Gameplay.ThiefCharmEnemyDeathAudio.SpawnAndPlayOneShot(spawnPoint.position);
                    instance.SpawnCurrency(spawnPoint, dropFlingSpeedMin, dropFlingSpeedMax,
                        dropFlingAngleMin, dropFlingAngleMax, smallGeoExtra, mediumGeoExtra,
                        largeGeoExtra, largeSmoothGeoExtra, true, 0, false);
                }
            };
        }

        if (!overrideSpecialDeath)
        {
            var go = instance.zeroHPEventOverride ? instance.zeroHPEventOverride : instance.gameObject;
            FSMUtility.SendEventToGameObject(go, "ZERO HP");
            if (instance.blackThreadState)
                instance.blackThreadState.CancelAttack();
            if (attackType == AttackTypes.Lava)
                FSMUtility.SendEventToGameObject(go, "LAVA DEATH");
            Utils.Logger.Debug(
                $"the obj death: {go} {go.FullName()} instance.hasSpecialDeath {instance.hasSpecialDeath}");
            if (instance.hasSpecialDeath)
            {
                instance.NonFatalHit(ignoreEvasion);
                onCorpseBegin?.Invoke(instance.transform);
                return;
            }
        }

        instance.isDead = true;
        if (instance.damageHero)
            instance.damageHero.damageDealt = 0;
        if (instance.battleScene && !instance.notifiedBattleScene)
        {
            var component = instance.battleScene.GetComponent<BattleScene>();
            if (component)
            {
                if (!instance.bigEnemyDeath)
                    component.DecrementEnemy();
                else
                    component.DecrementBigEnemy();
            }
        }

        if (instance.deathAudioSnapshot)
            instance.deathAudioSnapshot.TransitionTo(6f);
        if (instance.sendKilledTo)
            FSMUtility.SendEventToGameObject(instance.sendKilledTo, "KILLED");
        instance.SendDeathEvent();
        Utils.Logger.Debug($"the obj: {instance.gameObject} {instance.gameObject.FullName()}");

        if (Configs.RandomCorpseSplat)
            instance.corpseSplatPrefab.Spawn(instance.transform.position + instance.effectOrigin, Quaternion.identity);

        if (attackType == AttackTypes.Splatter)
        {
            if (Configs.Mode != NoEnemiesMode.BeGone)
            {
                GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
                instance.corpseSplatPrefab.Spawn(instance.transform.position + instance.effectOrigin,
                    Quaternion.identity);
                if (instance.enemyDeathEffects)
                    instance.enemyDeathEffects.EmitSound();
            }

            instance.gameObject.SetActive(false);
        }
        else
        {
            bool didCallCorpseBegin;
            GameObject? corpseObj;
            if (instance.enemyDeathEffects)
            {
                instance.enemyDeathEffects.SkipKillFreeze = true;
                instance.enemyDeathEffects.ReceiveBeGoneEvent(attackDirection, attackType, nailElement, damageSource,
                    corpseFlingMultiplier, instance.deathReset, onCorpseBegin, out didCallCorpseBegin,
                    out corpseObj);
            }
            else
            {
                didCallCorpseBegin = false;
                corpseObj = null;
            }

            Utils.Logger.Debug(
                $"didCallCorpseBegin: {didCallCorpseBegin} {corpseObj} {corpseObj?.FullName()} {onCorpseBegin}");

            if (Configs.Mode == NoEnemiesMode.BeGone && corpseObj &&
                corpseObj.TryGetComponent<ActiveCorpse>(out var activeCorpse))
            {
                Utils.Logger.Debug($"corpseObj change color: {corpseObj} {corpseObj.FullName()}");
                var color = activeCorpse.sprite.color;
                color.a = 0.0f;
                activeCorpse.sprite.color = color;
                activeCorpse.SetBlockAudio(true);
            }

            if (!didCallCorpseBegin && onCorpseBegin != null)
                onCorpseBegin(corpseObj ? corpseObj.transform : instance.transform);
            if (!corpseObj || !instance.corpseEventResponder)
                return;
            corpseObj.GetComponent<EventRelay>().TemporaryEvent += instance.corpseEventResponder.ReceiveEvent;
            Utils.Logger.Debug($"corpseObj: {corpseObj} {corpseObj.FullName()}");
        }
    }

    extension(HealthManager instance)
    {
        // ReSharper disable once InconsistentNaming
        [UsedImplicitly]
        public List<HealthManagerItemDropGroupProxy> itemDropGroups
        {
            get => (instance.GetFieldValue<List<object>>("itemDropGroups") ?? [])
                .Select(x => new HealthManagerItemDropGroupProxy(x)).ToList();
            set => instance.SetFieldValue("itemDropGroups",
                value.Select(x => x.InternalObject).ToList());
        }
    }
}