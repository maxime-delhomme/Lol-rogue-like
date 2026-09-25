using System;
using UnityEngine;

public class Sort : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private float _rayonImpact;
    [SerializeField] private LayerMask layerEnnemi;

    private float multiplicateurDegats = 1f;

    private Vector3 destination;

    public void Initialiser(Vector3 destination)
    {
        this.destination = destination;
    }

    public void Ameliorer()
    {
        multiplicateurDegats = 2f;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
           Impact();
        }
    }

    private void Impact()
    {
        Collider[] ennemisTouches = Physics.OverlapSphere(transform.position, _rayonImpact, layerEnnemi);
        
        foreach (Collider ennemi in ennemisTouches)
        {
            Ennemi cible = ennemi.GetComponent<Ennemi>();

            if (cible != null)
            {
                int degatsFinaux = Mathf.RoundToInt(_damage * multiplicateurDegats);
                cible.TakeDamage(degatsFinaux);
            }
        }

        Destroy(gameObject);
    }


}
