using BepInExUtils.Proxy;
using JetBrains.Annotations;

namespace HKSS.NoEnemies.Proxy;

/// <summary>
///     <see cref="HealthManager" />.ItemDropGroup
/// </summary>
[PublicAPI]
public class HealthManagerItemDropGroupProxy : ClassProxy
{
    private const string ClassName = "HealthManager+ItemDropGroup";

    /// <summary>
    ///     <see cref="HealthManager" />.ItemDropGroup
    /// </summary>
    public HealthManagerItemDropGroupProxy() : base(ClassName)
    {
    }

    /// <summary>
    ///     <see cref="HealthManager" />.ItemDropGroup
    /// </summary>
    public HealthManagerItemDropGroupProxy(object obj) : base(obj, ClassName)
    {
    }

    public float TotalProbability
    {
        get => Native.GetFieldValue<float>("TotalProbability");
        set => Native.SetFieldValue("TotalProbability", value);
    }

    public List<HealthManagerItemDropProbabilityProxy> Drops
    {
        get => Native.GetFieldValue<List<object>>("Drops").Select(x => new HealthManagerItemDropProbabilityProxy(x))
            .ToList();
        set => Native.SetFieldValue("Drops", value.Select(x => x.Native).ToList());
    }

    public ItemDropProbabilityCopy[] ToDropsCopyArray() => Drops.Select(x => new ItemDropProbabilityCopy
    {
        item = x.item,
        Amount = x.Amount,
        CustomPickupPrefab = x.CustomPickupPrefab,
        LimitActiveInScene = x.LimitActiveInScene
    }).ToArray();
}