
using System.Collections.Generic;
using UnityEngine;

public class DontTouchMeImproved : MonoBehaviour, ISpell
{
    [Header("Stun")]
    [SerializeField] private float _stunRadius = 5f;
    [SerializeField] private float _stunDuration = 3f;

    [Header("Damage")]
    [SerializeField] private int _damage = 30;

    [Header("Detection")]
    [SerializeField] private LayerMask _enemyLayer;

    public void Initialiser(SpellCastContext context)
    {
        Vector3 position = context._caster.position;

        Collider[] hits = Physics.OverlapSphere(
            position,
            _stunRadius,
            _enemyLayer
        );

        HashSet<Ennemi> affectedEnemies = new HashSet<Ennemi>();

        foreach (Collider hit in hits)
        {
            Ennemi enemy = hit.GetComponentInParent<Ennemi>();

            if (enemy == null || !affectedEnemies.Add(enemy))
                continue;

            enemy.TakeDamage(_damage);
            enemy.Etourdir(_stunDuration);
        }

        Destroy(gameObject);
    }
}
