using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class ThunderBolt : MonoBehaviour, ISpell
{
    private SpellData _data;

    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;

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

            enemy.TakeDamage(_data._damage);
        }

        Destroy(gameObject);
    }
}
