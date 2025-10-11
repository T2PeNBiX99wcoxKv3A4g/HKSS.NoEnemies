using BepInExUtils.Attributes;
using JetBrains.Annotations;

namespace HKSS.NoEnemies.Extensions;

[AccessExtensions]
[AccessInstance<ActiveCorpse>]
[AccessField<tk2dSprite>("sprite")]
[UsedImplicitly]
public static partial class ActiveCorpseExtensions
{
}