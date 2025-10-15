using System.Diagnostics.CodeAnalysis;
using BepInExUtils.Proxy;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;

namespace HKSS.NoEnemies.Proxy;

[PublicAPI]
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

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public SavedItem item
    {
        get => Native.GetFieldValue<SavedItem>("item");
        set => Native.SetFieldValue("item", value);
    }

    public MinMaxInt Amount
    {
        get => Native.GetFieldValue<MinMaxInt>("amount");
        set => Native.SetFieldValue("amount", value);
    }

    public CollectableItemPickup CustomPickupPrefab
    {
        get => Native.GetFieldValue<CollectableItemPickup>("customPickupPrefab");
        set => Native.SetFieldValue("customPickupPrefab", value);
    }

    public int LimitActiveInScene
    {
        get => Native.GetFieldValue<int>("limitActiveInScene");
        set => Native.SetFieldValue("limitActiveInScene", value);
    }
}