using System;
using UnityEngine;

public class Sort : MonoBehaviour
{
    [SerializeField] private SpellData _spellData;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _enemyLayer;

    private float _damageMultiplier = 1f;

    private Vector3 _direction;
    private float _distanceTraveled;

    public void Initialiser(Vector3 direction, bool improved)
    {
        _direction = direction.normalized;
        _distanceTraveled = 0f;

        if (improved)
        {
            InitialiserImproved();
        }
    }

    private void InitialiserImproved()
    {
        throw new NotImplementedException();
    }

    public void Ameliorer()
    {
        _damageMultiplier = 2f;
    }

    private void Update()
    {
        Vector3 anciennePosition = transform.position;

        float distance = _spellData._speed * Time.deltaTime;

        Vector3 nouvellePosition = anciennePosition + _direction * distance;

        if(Physics.Raycast(anciennePosition, _direction, out RaycastHit hit, distance, _obstacleLayer))
        {
            transform.position = hit.point;
            Impact();
            return;
        }

        transform.position = nouvellePosition;

        _distanceTraveled += distance;

        if (_distanceTraveled >= _spellData._range)
        {
            Impact();
        }
    }

    private void Impact()
    {
        Collider[] ennemisTouches = Physics.OverlapSphere(transform.position, _spellData._impactRadius, _enemyLayer);
        
        foreach (Collider ennemi in ennemisTouches)
        {
            Ennemi cible = ennemi.GetComponent<Ennemi>();

            if (cible != null)
            {
                int degatsFinaux = Mathf.RoundToInt(_spellData._damage * _damageMultiplier);
                cible.TakeDamage(degatsFinaux);
            }
        }

        Destroy(gameObject);
    }


}
