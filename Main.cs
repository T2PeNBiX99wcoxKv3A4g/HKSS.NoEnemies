using BepInEx;
using BepInExUtils.Attributes;

namespace HKSS.NoEnemies;

[BepInUtils("io.github.ykysnk.HKSS.NoEnemies", "No Enemies", Version)]
[BepInProcess(Utils.GameName)]
[ConfigBind<NoEnemiesMode>("Mode", SectionOptions, NoEnemiesMode.KillWhenSpawn, "No Enemies Mode")]
public partial class Main
{
    private const string SectionOptions = "Options";
    private const string Version = "0.0.1";

    protected override void PostAwake()
    {
    }
}