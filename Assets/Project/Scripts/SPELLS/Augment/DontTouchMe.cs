using System.Collections.Generic;
using UnityEngine;

public class DontTouchMe : MonoBehaviour, ISpell
{
    [Header("Stun")]
    [SerializeField] private float _stunRadius = 5f;
    [SerializeField] private float _stunDuration = 3f;

    [Header("Detection")]
    [SerializeField] private LayerMask _enemyLayer;

    public void Initialiser(SpellCastContext context)
    {
        Debug.Log("dontotuchemoue");
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

            enemy.Etourdir(_stunDuration);
        }

        Destroy(gameObject);
    }
}
