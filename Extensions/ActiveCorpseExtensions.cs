using BepInExUtils.Attributes;
using JetBrains.Annotations;

namespace HKSS.NoEnemies.Extensions;

[AccessExtensions]
[AccessInstance<ActiveCorpse>]
[AccessField<tk2dSprite>("sprite")]
[PublicAPI]
public static partial class ActiveCorpseExtensions
{
}