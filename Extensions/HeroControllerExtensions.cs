using BepInExUtils.Attributes;
using GlobalEnums;

namespace HKSS.NoEnemies.Extensions;

[AccessExtensions]
[AccessInstance<HeroController>]
[AccessMethod("SetState", typeof(ActorStates))]
public static partial class HeroControllerExtensions
{
}