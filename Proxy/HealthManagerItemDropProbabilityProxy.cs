using BepInExUtils.Proxy;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;

namespace HKSS.NoEnemies.Proxy;

[UsedImplicitly]
public class HealthManagerItemDropProbabilityProxy : ClassProxy
{
    private const string ClassName = "HealthManager+ItemDropProbability";

    /// <summary>
    ///     <see cref="HealthManager" />.ItemDropProbability
    /// </summary>
    public HealthManagerItemDropProbabilityProxy() : base(ClassName)
    {
    }

    /// <summary>
    ///     <see cref="HealthManager" />.ItemDropProbability
    /// </summary>
    public HealthManagerItemDropProbabilityProxy(object obj) : base(obj, ClassName)
    {
    }

    // ReSharper disable once InconsistentNaming
    public SavedItem? item
    {
        get => Instance.GetFieldValue<SavedItem>("item");
        set => Instance.SetFieldValue("item", value);
    }

    public MinMaxInt Amount
    {
        get => Instance.GetFieldValue<MinMaxInt>("amount");
        set => Instance.SetFieldValue("amount", value);
    }

    public CollectableItemPickup? CustomPickupPrefab
    {
        get => Instance.GetFieldValue<CollectableItemPickup>("customPickupPrefab");
        set => Instance.SetFieldValue("customPickupPrefab", value);
    }

    public int LimitActiveInScene
    {
        get => Instance.GetFieldValue<int>("limitActiveInScene");
        set => Instance.SetFieldValue("limitActiveInScene", value);
    }

    public object? InternalObject => Instance;
}