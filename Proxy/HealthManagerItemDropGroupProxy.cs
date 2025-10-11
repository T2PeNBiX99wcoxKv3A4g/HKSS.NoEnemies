using BepInExUtils.Proxy;
using JetBrains.Annotations;

namespace HKSS.NoEnemies.Proxy;

/// <summary>
///     <see cref="HealthManager" />.ItemDropGroup
/// </summary>
[UsedImplicitly]
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

    [UsedImplicitly]
    public float TotalProbability
    {
        get => Instance.GetFieldValue<float>("TotalProbability");
        set => Instance.SetFieldValue("TotalProbability", value);
    }

    [UsedImplicitly]
    public List<HealthManagerItemDropProbabilityProxy> Drops
    {
        get => (Instance.GetFieldValue<List<object>>("Drops") ?? [])
            .Select(x => new HealthManagerItemDropProbabilityProxy(x)).ToList();
        set => Instance.SetFieldValue("Drops",
            value.Select(x => x.InternalObject ?? throw new NullReferenceException()).ToList());
    }

    public object? InternalObject => Instance;

    public ItemDropProbabilityCopy[] ToDropsCopyArray() => Drops.Select(x => new ItemDropProbabilityCopy
    {
        item = x.item,
        Amount = x.Amount,
        CustomPickupPrefab = x.CustomPickupPrefab,
        LimitActiveInScene = x.LimitActiveInScene
    }).ToArray();
}