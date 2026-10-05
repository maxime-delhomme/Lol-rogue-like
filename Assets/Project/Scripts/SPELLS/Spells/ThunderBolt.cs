using UnityEngine;

public class ThunderBolt : MonoBehaviour, ISpell
{
    private SpellData _data;
    private Character _caster;

    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;
        _caster = context._caster.GetComponent<Character>();

        transform.position = context._targetPosition;

        Impact();
    }

    private void Impact()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, _data._impactRadius);

        foreach (Collider collider in colliders)
        {
            Ennemi enemy = collider.GetComponent<Ennemi>();

            if (enemy == null)
            {
                continue;
            }
            int damage = _caster.CalculateSpellDamage(_data._damage);

            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
