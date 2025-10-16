using BepInEx;
using BepInExUtils.Attributes;

namespace HKSS.NoEnemies;

[BepInUtils("io.github.ykysnk.HKSS.NoEnemies", "No Enemies", Version)]
[BepInDependency("io.github.ykysnk.BepinExUtils", "0.9.0")]
[BepInProcess(Utils.GameName)]
[ConfigBind<NoEnemiesMode>("Mode", SectionOptions, NoEnemiesMode.BeGone, "No enemies working modes.")]
[ConfigBind<bool>("RandomCorpseSplat", SectionOptions, false, "Random corpse splat in the maps.")]
public partial class Main
{
    private const string SectionOptions = "Options";
    private const string Version = "0.0.6";

    public void Init()
    {
    }
}