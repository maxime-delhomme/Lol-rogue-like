using UnityEngine;

[CreateAssetMenu(fileName = "AugmentDoubleSort", menuName = "Game/Augments/Double Sort")]
public class AugmentDoubleSort : AugmentEffect
{
    private SpellCaster _spellCaster;

    public override void Apply(Character player)
    {
        _spellCaster = player.GetComponent<SpellCaster>();

        if (_spellCaster == null)
        {
            Debug.LogWarning("SpellCaster introuvable sur le joueur.");
            return;
        }

        _spellCaster.OnSpellCast += HandleSpellCast;
    }

    private void HandleSpellCast(SpellCastContext context, Vector3 launchPosition)
    {

        if (context._data == null || context._data._prefab == null)
            return;

        GameObject spellObject = Instantiate(
            context._data._prefab,
            launchPosition,
            Quaternion.identity
        );

        ISpell spell = spellObject.GetComponent<ISpell>();

        if (spell != null)
            spell.Initialiser(context);

        Debug.Log($"Position du deuxième projectile : {spellObject.transform.position}");
        Debug.Log($"Direction du deuxième projectile : {context._direction}");
    }
}