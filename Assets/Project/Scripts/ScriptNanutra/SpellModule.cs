using UnityEngine;

public abstract class SpellModule : ScriptableObject
{
    public virtual void OnCast(SpellContext ctx) { }
    public virtual void OnTick(SpellContext ctx) { }
    public virtual void OnHit(SpellContext ctx) { }
    public virtual void OnEnd(SpellContext ctx) { }
}
