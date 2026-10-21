using UnityEngine;

[CreateAssetMenu]
public class SplitAugmentation : Augmentation
{
    public float splitDelay = 0.5f;
    public int splitCount = 2;

    public override void Apply(SpellContext ctx)
    {
        ctx.Modules.Add(new SplitModule { splitDelay = splitDelay, splitCount = splitCount });
    }
}
