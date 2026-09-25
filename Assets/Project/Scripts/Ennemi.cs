using System;
using UnityEngine;

public class Ennemi : MonoBehaviour
{
    [SerializeField] private int _health;

    public void TakeDamage(int damage)
    {
        _health -= damage;

        Debug.Log(gameObject.name + " reçoit " +  damage + " dégats. PV restants : " + _health);

        if(_health <= 0 )
        {
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
