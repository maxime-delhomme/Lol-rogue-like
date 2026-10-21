using UnityEngine;

public abstract class Augmentation : ScriptableObject
{
    public abstract void Apply(SpellContext ctx); // ajoute des modules, modifie des stats
}
