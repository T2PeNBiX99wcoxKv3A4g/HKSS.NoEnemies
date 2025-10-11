using System.Diagnostics.CodeAnalysis;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HKSS.NoEnemies.Proxy;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ItemDropProbabilityCopy : Probability.ProbabilityBase<SavedItem>
{
    [SerializeField] internal SavedItem? item;
    public MinMaxInt Amount = new(1, 1);
    public CollectableItemPickup? CustomPickupPrefab;
    public int LimitActiveInScene;

    public override SavedItem Item => item!;
}