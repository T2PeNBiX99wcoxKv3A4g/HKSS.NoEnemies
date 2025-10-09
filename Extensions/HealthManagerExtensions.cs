using BepInExUtils.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace HKSS.NoEnemies.Extensions;

[AccessExtensions]
[AccessInstance<HealthManager>]
[AccessField<bool>("hasBlackThreadState")]
[AccessField<BlackThreadState>("blackThreadState")]
[AccessField<GameObject>("battleScene")]
[AccessField<bool>("bigEnemyDeath")]
[AccessMethod("Invincible", typeof(HitInstance))]
[AccessMethod("TakeDamage", typeof(HitInstance))]
[AccessMethod("DieDropFling", typeof(ToolDamageFlags), typeof(bool))]
[UsedImplicitly]
public static partial class HealthManagerExtensions
{
}