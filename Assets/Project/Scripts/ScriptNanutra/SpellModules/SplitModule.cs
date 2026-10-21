using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Modules/Split")]
public class SplitModule : SpellModule
{
    public float splitDelay = 0.5f;  // temps avant division
    public int splitCount = 2;       // en combien de morceaux
    public float spreadAngle = 30f;  // écart angulaire entre les morceaux

    public override void OnCast(SpellContext ctx)
    {
        // On enregistre un timer dans le contexte


        //ctx.AddTimer(splitDelay, () => DoSplit(ctx));
    }

    private void DoSplit(SpellContext ctx)
    {
        /*
        // On récupère le projectile principal spawné par le ProjectileModule
        var main = ctx.MainProjectile;
        if (main == null) return;

        for (int i = 0; i < splitCount; i++)
        {
            float angle = Mathf.Lerp(-spreadAngle, spreadAngle, i / (float)(splitCount - 1));
            var dir = Quaternion.Euler(0, 0, angle) * main.Direction;
            ctx.SpawnProjectile(main.Prefab, main.Position, dir, main.Damage);
        }

        ctx.Despawn(main); // on retire le projectile original
        */
    }
}